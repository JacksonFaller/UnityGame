﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private InventoryItemObject _inventoryItem;

    [SerializeField]
    private ItemsDatabase _itemsDatabase = null;

    private SpriteRenderer _spriteRenderer;
    private bool _isInitialized = false;

    public int? ItemId => _inventoryItem?.ItemID;

    public InventoryItemObject InventoryItemObject => _inventoryItem;

    void Start()
    {
        if (!_itemsDatabase.TryGetValue(_inventoryItem.ItemID, out _inventoryItem))
        {
            Debug.LogError($"Item with ID {_inventoryItem.ItemID} is not found in items database!");
        }
        else
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = _inventoryItem.Image;
        }

        _isInitialized = true;
    }

    void Update()
    {
        
    }

    public void SetItem(InventoryItemObject item)
    {
        if(!_isInitialized)
            _inventoryItem = item;
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
