using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemsDatabase))]
[CanEditMultipleObjects]
class TestEditor : Editor
{
    private ItemsDatabase _itemsDatabase;

    private string _itemName = string.Empty;
    private string[] _searchResults;
    private int _selectedIndex;
    private bool _isEdit;
    private bool _isSettingsOpened;
    private int _editingItemId;

    void OnEnable()
    {
        _itemsDatabase = (ItemsDatabase)serializedObject.targetObject;
        _itemsDatabase.InventoryItem = new InventoryItemObject() { Name = "Test" };
        _searchResults = Array.Empty<string>();

        //Debug.Log(string.Join(",", _itemsDatabase.Items.Select(x => x.Key)));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FindItem();
        UpdateItem();
        serializedObject.ApplyModifiedProperties();
    }

    private void FindItem()
    {
        GUILayout.Space(10);
        GUILayout.Label("Find item");
        GUILayout.BeginHorizontal();
        _itemName = EditorGUILayout.TextField(_itemName);
        if (GUILayout.Button("Find"))
        {
            UpdateSearchResults();
        }

        GUILayout.EndHorizontal();

        _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _searchResults);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Edit") && _searchResults.Length > 0)
        {
            _isEdit = true;
            int key = _searchResults[_selectedIndex].GetHashCode();
            _editingItemId = key;
            _itemsDatabase.InventoryItem = _itemsDatabase.Items[key].Copy();
        }

        if (GUILayout.Button("Copy"))
        {
            int key = _searchResults[_selectedIndex].GetHashCode();
            _itemsDatabase.InventoryItem = _itemsDatabase.Items[key].Copy();
        }

        if (GUILayout.Button("Delete") && _searchResults.Length > 0)
        {
            int key = _searchResults[_selectedIndex].GetHashCode();
            _itemsDatabase.Items.Remove(key);
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

        if (_isEdit)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                int itemKey = _itemsDatabase.InventoryItem.Id;
                if (_editingItemId != itemKey)
                {
                    if (_itemsDatabase.Items.ContainsKey(itemKey))
                    {
                        Debug.LogError($"Items database already contains item with name: {_itemsDatabase.InventoryItem.Name}");
                    }
                    else
                    {
                        _itemsDatabase.Items.Remove(_editingItemId);
                        _itemsDatabase.Items.Add(itemKey, _itemsDatabase.InventoryItem);
                    }
                }



                _itemsDatabase.Items[itemKey] = _itemsDatabase.InventoryItem;
                _isEdit = false;
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
                if (!_itemsDatabase.Items.ContainsKey(_itemsDatabase.InventoryItem.Id))
                {
                    _itemsDatabase.Items.Add(_itemsDatabase.InventoryItem.Id, _itemsDatabase.InventoryItem);
                    _itemsDatabase.InventoryItem = _itemsDatabase.InventoryItem.Copy();
                }
                else
                {
                    Debug.LogError($"Items database already contains item with name: {_itemsDatabase.InventoryItem.Name}");
                }
            }
        }

        GUILayout.EndVertical();
    }

    private void UpdateSearchResults()
    {
        _searchResults = _itemsDatabase.Items.Values.Select(x => x.Name)
              .Where(x => x.IndexOf(_itemName, StringComparison.OrdinalIgnoreCase) != -1)
              .ToArray();
    }
}
