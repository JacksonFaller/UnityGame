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

    /// <summary>
    /// Returns a copy of an item
    /// </summary>
    /// <param name="key">Item ID</param>
    /// <returns>Item</returns>
    public InventoryItemObject this[int key]
    {
        get => Items[key].Copy();
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (key != value.ItemID)
                throw new ArgumentException("ItemID should be the same as the key in items database", nameof(value));

            if (ContainsKey(key))
            {
                if (Items[key].Name != value.Name && _itemsNames.Contains(value.Name))
                    throw new ArgumentException($"Items database already contains item with name: {value.Name}");
                Items[key] = value;
            }
            else Add(key, value);
            _isCached = false;
        }
    }

    public InventoryItemObject GetItemCopy(int key, bool useNextItemID = false)
    {
        if(!useNextItemID)
            return Items[key].Copy(NextItemID);

        return Items[key].Copy();
    }

    public bool TryAdd(int key, InventoryItemObject value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

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
    /// <param name="key">Item's copy ID</param>
    /// <param name="value">Reference item</param>
    public bool AddCopy(int key, InventoryItemObject value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (ItemsNames.Contains(value.Name))
            throw new ArgumentException($"Items database already contains item with name: {value.Name}");

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

    /// <summary>
    /// Returns a copy of an item if exists
    /// </summary>
    /// <param name="key">Item ID</param>
    /// <param name="value">Item if found, otherwise default value</param>
    /// <returns>True if item with the key exists, otherwise false</returns>
    public bool TryGetValue(int key, out InventoryItemObject value)
    {
        bool result = Items.TryGetValue(key, out value);
        if (result) value = value.Copy();
        return result;
    }

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