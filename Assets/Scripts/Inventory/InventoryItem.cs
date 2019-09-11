using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private InventoryItemObject _inventoryItem = null;

    [SerializeField]
    private MonoBehaviour _effect = null;

    public InventoryItemObject InventoryItemObject => _inventoryItem;

    void Start()
    {
        _inventoryItem.Effect = _effect;
    }

    void Update()
    {
        
    }
}
