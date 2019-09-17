using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Inverntory item", menuName = "Inventory item")]
[Serializable]
public class ItemsDatabase : ScriptableObject, IDictionary<int, InventoryItemObject>, ISerializationCallbackReceiver
{
    public int NextItemID { get; private set; }

    public InventoryItemObject InventoryItem;
    public GameObject ItemPrefab;


    [SerializeField]
    private List<InventoryItemObject> _inventoryItems;

    private Dictionary<int, InventoryItemObject> _items;

    private Dictionary<int, InventoryItemObject> Items
    {
        get
        {
            if(_items == null)
            {
                _items = _inventoryItems.ToDictionary(x => x.ItemID);
            }
            return _items;
        }
    }

    private HashSet<string> _itemsNames;

    private HashSet<string> ItemsNames
    {
        get
        {
            if (_itemsNames == null)
            {
                _itemsNames = new HashSet<string>(Items.Values.Select(x => x.Name));
            }
            return _itemsNames;
        }
    }

    private bool _isCached;

    public bool ContainsItemWithName(string name) => ItemsNames.Contains(name);


    #region Dictionary 

    public InventoryItemObject this[int key]
    {
        get => Items[key];
        set
        {
            if (key != value.ItemID)
                throw new ArgumentException("ItemID should be the same as the key in items database", nameof(value));

            if (ContainsKey(key)) Items[key] = value;
            else Add(key, value);
            _isCached = false;
        }
    }


    public bool TryAdd(int key, InventoryItemObject value)
    {
        if (value.ItemID != NextItemID)
            return AddCopy(key, value);

        if (ItemsNames.Contains(value.Name)) return false;

        ItemsNames.Add(value.Name);
        Items.Add(key, value);
        _isCached = false;
        NextItemID++;
        return true;
    }


    /// <summary>
    /// Adds an inventory item to the database. 
    /// If itemID is not equal to a NextItemID, a copy will be added to the database.
    /// </summary>
    /// <param name="key">Item key</param>
    /// <param name="value">Item value</param>
    public void Add(int key, InventoryItemObject value)
    {
        if (!TryAdd(key, value))
            throw new ArgumentException($"Items database already have an item with name {value.Name}", nameof(value));
    }

    /// <summary>
    /// Add a copy of an item to the database
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public bool AddCopy(int key, InventoryItemObject value)
    {
        if (ItemsNames.Contains(value.Name))
            return false;

        ItemsNames.Add(value.Name);
        Items.Add(key, value.Copy(NextItemID));
        _isCached = false;
        NextItemID++;
        return true;
    }

    public ICollection<int> Keys => Items.Keys;

    public ICollection<InventoryItemObject> Values => Items.Values;

    public int Count => Items.Count;

    public bool IsReadOnly => false;

    public bool ContainsKey(int key) => Items.ContainsKey(key);

    public bool TryGetValue(int key, out InventoryItemObject value) => Items.TryGetValue(key, out value);

    public void Add(KeyValuePair<int, InventoryItemObject> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        Items.Clear();
        _isCached = false;
        ItemsNames.Clear();
    }

    public bool Contains(KeyValuePair<int, InventoryItemObject> item) => Items.Contains(item);

    public bool Remove(KeyValuePair<int, InventoryItemObject> item) => Remove(item.Key);
    bool IDictionary<int, InventoryItemObject>.Remove(int key) => Remove(key);

    public bool Remove(int key)
    {
        if(Items.TryGetValue(key, out InventoryItemObject value))
        {
            ItemsNames.Remove(value.Name);
            _isCached = false;
            return Items.Remove(key);
        } 
        return false;
    }

    public IEnumerator<KeyValuePair<int, InventoryItemObject>> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void CopyTo(KeyValuePair<int, InventoryItemObject>[] array, int arrayIndex) => throw new NotImplementedException();

    #endregion

    public void OnBeforeSerialize()
    {
        if (!_isCached)
        {
            _inventoryItems = Items.Values.ToList();
            _isCached = true;
        }
    }

    public void OnAfterDeserialize()
    {
    }
}