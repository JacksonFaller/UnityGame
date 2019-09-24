using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Slot : MonoBehaviour
{
    [SerializeField]
    private InventoryItemObject _inventoryItem;

    [SerializeField]
    private uint _itemAmount = 1;

    [SerializeField]
    private bool _hasItem = false;

    [Space]
    [SerializeField]
    private ItemsDatabase _itemsDatabase = null;

    [SerializeField]
    private Image _itemImage = null;

    [SerializeField]
    private TextMeshProUGUI _amountText = null;

    public uint ItemsAmount => _itemAmount;

    void Start()
    {
        if (!_hasItem)
        {
            _itemImage.enabled = false;
            _amountText.enabled = false;
            return;
        }

        if (!_itemsDatabase.TryGetValue(_inventoryItem.ItemID, out _inventoryItem))
        {
            Debug.LogError($"Item with ID {_inventoryItem.ItemID} is not found in items database!");
        }
        else
        {
            _itemImage.sprite = _inventoryItem.Image;
            if (_inventoryItem.StackSize == 1)
            {
                _amountText.enabled = false;
            }
            else
            {
                _amountText.enabled = true;
                _amountText.text = _itemAmount.ToString();
            }
        }
    }

    void Update()
    {

    }

    public void SetItem(InventoryItemObject item)
    {
        _inventoryItem = item;
        _itemImage.sprite = _inventoryItem.Image;
        _itemImage.enabled = true;
        _hasItem = true;
        if(_inventoryItem.StackSize > 1)
        {
            _amountText.enabled = true;
            _amountText.text = "1";
        }
    }

    public void AddItem(InventoryItemObject item)
    {
        if (_itemAmount == item.StackSize)
        {
            Debug.LogError("Can't add item to this slot. It has reached it's max capacity");
        }
        else
        {
            if (_inventoryItem.ItemID != item.ItemID)
            {
                Debug.LogError($"Can't add item with ID {item.ItemID} to this slot. It already contains item with different ID: {_inventoryItem.ItemID}");
            }
            else
            {
                _itemAmount++;
                _amountText.text = _itemAmount.ToString();
            }
        }
    }

    public void UpdateTextSize(Vector2 size)
    {
        _amountText.rectTransform.sizeDelta = size;
    }

    [ContextMenu("Update item in database")]
    private void UpdateItemInDatabase()
    {
        if (!_itemsDatabase.TryGetValue(_inventoryItem.ItemID, out InventoryItemObject item))
        {
            Debug.Log($"Item with ID {_inventoryItem.ItemID} is not found in items database");
            return;
        }
        if (_inventoryItem.Name != item.Name && _itemsDatabase.ContainsItemWithName(_inventoryItem.Name))
        {
            Debug.LogError($"Items database already contains item with name {_inventoryItem.Name}");
            return;
        }
        _itemsDatabase[_inventoryItem.ItemID] = _inventoryItem;
    }
}
