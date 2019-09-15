using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ItemsEditor : EditorWindow
{
    public InventoryItemObject InventoryItem;

    private SerializedProperty _itemProperty;
    private List<SerializedProperty> _childProperties = new List<SerializedProperty>();
    private SerializedObject _editorObject;

    private Dictionary<int, InventoryItemObject> _itemsDatabase;
    private string _itemName = string.Empty;
    private string[] _searchResults;
    private int _selectedIndex;
    private bool _isEdit;
    private bool _isSettingsOpened;
    private string _itemDatabasePath;


    [MenuItem("Window/Items editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemsEditor>();
    }

    private void OnEnable()
    {
        InventoryItem = new InventoryItemObject() { Name = "Test" };
        _itemsDatabase = new Dictionary<int, InventoryItemObject>();
        var testObj = new InventoryItemObject() { Name = "Test Shield" };
        _itemsDatabase.Add(testObj.Id, testObj);
        _searchResults = Array.Empty<string>();

        UpdateProperties();
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Find item");
        GUILayout.BeginHorizontal();
        _itemName = EditorGUILayout.TextField(_itemName);
        if (GUILayout.Button("Find"))
        {
            _searchResults = _itemsDatabase.Values.Select(x => x.Name)
                .Where(x => x.IndexOf(_itemName, StringComparison.OrdinalIgnoreCase) != -1)
                .ToArray();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _searchResults);
        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Edit") && _searchResults.Length > 0)
        {
            _isEdit = true;
            int key = _searchResults[_selectedIndex].GetHashCode();
            InventoryItem = _itemsDatabase[key].Copy();
            UpdateProperties();
            //InventoryItem = _searchResults[_selectedIndex];
        }

        if (GUILayout.Button("Delete"))
        {

        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginVertical(GUI.skin.box);


        // TODO: Replace with foreach in children properties
        //EditorGUILayout.PropertyField(_itemProperty, new GUIContent("Item"), true);

        foreach (var prop in _childProperties)
        {
            EditorGUILayout.PropertyField(prop, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
        }

        if (_isEdit)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                _itemsDatabase[InventoryItem.Id] = InventoryItem;
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
                if(!_itemsDatabase.ContainsKey(InventoryItem.Id))
                {
                    _itemsDatabase.Add(InventoryItem.Id, InventoryItem);
                    InventoryItem = InventoryItem.Copy();
                    UpdateProperties();
                }
                else
                {
                    Debug.LogError($"Items database already contains item with name: {InventoryItem.Name}");
                }
            }

            //if (GUILayout.Button("Clear"))
            //{
            //    InventoryItem = new InventoryItemObject();
            //    MainObject = new SerializedObject(this);
            //    ItemProperty = MainObject.FindProperty("InventoryItem");
            //}

        }

        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical(GUI.skin.box);


        _isSettingsOpened = EditorGUILayout.Foldout(_isSettingsOpened, new GUIContent("Settings"));
        if (_isSettingsOpened)
        {
            _itemDatabasePath = EditorGUILayout.TextField("Database path");

            if (GUILayout.Button("Load"))
            {
                Debug.Log("Item database is loaded");
            }
        }

        GUILayout.EndVertical();


        //EditorGUILayout.PropertyField(Prop.FindPropertyRelative("Size"));
        _editorObject.ApplyModifiedProperties();
    }

    private void UpdateProperties()
    {
        _childProperties.Clear();
        _editorObject = new SerializedObject(this);
        _itemProperty = _editorObject.FindProperty("InventoryItem");

        bool children = true;
        while (_itemProperty.NextVisible(children))
        {
            _childProperties.Add(_itemProperty.Copy());
            children = !_itemProperty.isArray;
        }
    }
}
