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
            if(_inventoryItem.StackSize == 1)
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
    }

    public void UpdateTextSize(Vector2 size)
    {
        _amountText.rectTransform.sizeDelta = size;
    }

    [ContextMenu("Update item in database")]
    private void UpdateItemInDatabase()
    {
        if(!_itemsDatabase.TryGetValue(_inventoryItem.ItemID, out InventoryItemObject item))
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
