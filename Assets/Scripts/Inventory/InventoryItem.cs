using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private InventoryItemObject _inventoryItem = null;

    [SerializeField]
    private int _itemID;

    [SerializeField]
    private ItemsDatabase _itemsDatabase = null;


    private SpriteRenderer _spriteRenderer;
    private bool _isInitialized = false;

    public InventoryItemObject InventoryItemObject => _inventoryItem;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (!_itemsDatabase.TryGetValue(_itemID, out _inventoryItem))
        {
            Debug.LogError($"Item with ID {_itemID} is not found in items database!");
        }
        _spriteRenderer.sprite = _inventoryItem.Image;
        _isInitialized = true;
    }

    void Update()
    {
        
    }

    public void SetItem(int itemID)
    {
        if(!_isInitialized) _itemID = itemID;
    }
}
