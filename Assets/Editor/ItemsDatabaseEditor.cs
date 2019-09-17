using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemsDatabase))]
[CanEditMultipleObjects]
class ItemsDatabaseEditor : Editor
{
    private ItemsDatabase _itemsDatabase;

    private string _itemName = string.Empty;
    private List<KeyValuePair<int, string>> _searchResults;
    private string[] _searchResultsNames;
    private int _selectedIndex;
    private bool _isEdit;
    private bool _isSettingsOpened;
    private KeyValuePair<int, string> _editingItem;

    private GUIStyle _popupStyle;

    private GUIStyle PopupStyle
    {
        get
        {
            if(_popupStyle == null)
            {
                _popupStyle = new GUIStyle(EditorStyles.popup)
                {
                    fontSize = 11,
                    fixedHeight = 18
                };
            }
            return _popupStyle;
        }
    }

    void OnEnable()
    {
        _itemsDatabase = (ItemsDatabase)serializedObject.targetObject;
        _itemsDatabase.InventoryItem = new InventoryItemObject() { Name = "Test" };

        _searchResultsNames = Array.Empty<string>();
        _searchResults = new List<KeyValuePair<int, string>>();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FindItem();
        UpdateItem();

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemPrefab"), new GUIContent("Item prefab"));
        GUILayout.Space(3);

        if(GUILayout.Button("Instantiate"))
        {
            if (_itemsDatabase.ItemPrefab == null)
            {
                Debug.LogError("Item prefab is not referenced!");
            }
            else
            {
                var item =  Instantiate(_itemsDatabase.ItemPrefab);
                var inventoryItem = item.GetComponent<InventoryItem>();
                inventoryItem.SetItem(_searchResults[_selectedIndex].Key);
            }
        }

        if(GUILayout.Button("Clear"))
        {
            _itemsDatabase.Clear();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void FindItem()
    {
        GUILayout.Space(5);
        GUILayout.Label("Find item");
        GUILayout.BeginHorizontal();

        _itemName = EditorGUILayout.TextField(_itemName);
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.margin.top = 1;
        if (GUILayout.Button(new GUIContent("Find"), style))
        {
            UpdateSearchResults();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(1);

        _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _searchResultsNames, PopupStyle);

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Edit") && _searchResults.Count > 0)
        {
            _isEdit = true;
            _editingItem = _searchResults[_selectedIndex];
            _itemsDatabase.InventoryItem = _itemsDatabase[_editingItem.Key].Copy();
        }

        if (GUILayout.Button("Copy"))
        {
            int key = _searchResults[_selectedIndex].Key;
            _itemsDatabase.InventoryItem = _itemsDatabase[key].Copy(_itemsDatabase.NextItemID);
        }

        if (GUILayout.Button("Delete") && _searchResults.Count > 0)
        {
            _itemsDatabase.Remove(_searchResults[_selectedIndex].Key);
            UpdateSearchResults();
        }

        GUILayout.EndHorizontal();
    }

    private void UpdateItem()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("InventoryItem"), new GUIContent("Item"), true);

        if (EditorGUI.EndChangeCheck())
        {
        }

        GUILayout.Space(3);

        if (_isEdit)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                if(_itemsDatabase.InventoryItem.Name != _editingItem.Value && 
                    _itemsDatabase.ContainsItemWithName(_itemsDatabase.InventoryItem.Name))
                {
                    Debug.LogError($"Items database already contains item with name: {_itemsDatabase.InventoryItem.Name}");
                }
                else
                {
                    _itemsDatabase[_itemsDatabase.InventoryItem.ItemID] = _itemsDatabase.InventoryItem;
                    _isEdit = false;
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                _isEdit = false;
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Create"))
            {
                if (!_itemsDatabase.TryAdd(_itemsDatabase.NextItemID, _itemsDatabase.InventoryItem))
                    Debug.LogError($"Items database already contains item with name: {_itemsDatabase.InventoryItem.Name}");

            }
        }

        GUILayout.EndVertical();
    }

    private void UpdateSearchResults()
    {
        _searchResults = _itemsDatabase.Values.OrderBy(x => x.Name)
              .Where(x => x.Name.IndexOf(_itemName, StringComparison.OrdinalIgnoreCase) != -1)
              .Select(x => new KeyValuePair<int, string>(x.ItemID, x.Name))
              .ToList();

        _searchResultsNames = _searchResults.Select(x => x.Value).ToArray();
    }
}
