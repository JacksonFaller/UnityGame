using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text;

public class ItemsEditor : EditorWindow
{
    private const string _settingsPath = "Assets/Configurations/ItemsEditorSettings.json";
    private EditorSettings _settings;

    public InventoryItemObject InventoryItem;
    public ItemsDatabase ItemsDatabase;

    private SerializedObject _editorObject;
    private SerializedProperty _itemProperty;
    private SerializedProperty _database;

    private Dictionary<int, InventoryItemObject> _itemsDatabase;

    private string _itemName = string.Empty;
    private string[] _searchResults;
    private int _selectedIndex;
    private bool _isEdit;
    private bool _isSettingsOpened;


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

        LoadSettings();
        UpdateItemProperties();
        _database = _editorObject.FindProperty("ItemsDatabase");
    }

    void OnGUI()
    {
        FindItem();
        UpdateItem();
        Settings();

        //EditorGUILayout.PropertyField(Prop.FindPropertyRelative("Size"));
        _editorObject.ApplyModifiedProperties();
    }

    private void FindItem()
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
            UpdateItemProperties();
            //InventoryItem = _searchResults[_selectedIndex];
        }

        if (GUILayout.Button("Delete"))
        {

        }
        GUILayout.EndHorizontal();
    }

    private void UpdateItem()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical(GUI.skin.box);


        EditorGUILayout.PropertyField(_itemProperty, new GUIContent("Item"), true);

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
                if (!_itemsDatabase.ContainsKey(InventoryItem.Id))
                {
                    _itemsDatabase.Add(InventoryItem.Id, InventoryItem);
                    InventoryItem = InventoryItem.Copy();
                    UpdateItemProperties();
                }
                else
                {
                    Debug.LogError($"Items database already contains item with name: {InventoryItem.Name}");
                }
            }
        }

        GUILayout.EndVertical();
    }

    private void Settings()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical(GUI.skin.box);


        _isSettingsOpened = EditorGUILayout.Foldout(_isSettingsOpened, new GUIContent("Settings"));
        if (_isSettingsOpened)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_database, false);
            if(EditorGUI.EndChangeCheck())
            {

            }

        }

        GUILayout.EndVertical();
    }

    private void UpdateItemProperties()
    {
        _editorObject = new SerializedObject(this);
        _itemProperty = _editorObject.FindProperty("InventoryItem");

        //bool children = true;
        //while (_itemProperty.NextVisible(children))
        //{
        //    _childProperties.Add(_itemProperty.Copy());
        //    children = !_itemProperty.isArray;
        //}
    }

    private void LoadSettings()
    {
        using (var file = new FileStream(_settingsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        using (var streamReader = new StreamReader(file))
        {
            _settings = JsonConvert.DeserializeObject<EditorSettings>(streamReader.ReadToEnd()) ?? new EditorSettings();

            if (string.IsNullOrEmpty(_settings.ItemsDatabasePath))
            {
                _settings.ItemsDatabasePath = EditorSettings.DefaultItemsDatabasePath;
                file.SetLength(0);
                var streamWriter = new StreamWriter(file);
                streamWriter.Write(JsonConvert.SerializeObject(_settings));
                streamWriter.Flush();
            }
        }
        ItemsDatabase = AssetDatabase.LoadAssetAtPath(_settings.ItemsDatabasePath, typeof(ItemsDatabase)) as ItemsDatabase;
        if (ItemsDatabase == null)
            Debug.LogWarning("Items database is not found! Please add reference to it in settings.");
    }

    class EditorSettings
    {
        [JsonIgnore]
        public const string DefaultItemsDatabasePath = "Assets/ScriptableObjects/ItemsDatabase";

        public string ItemsDatabasePath { get; set; }
    }
}
