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
        if(_inventoryItem.AbilityType == AbilityType.Effect)
        {
            if (_effect == null) Debug.LogError("Ability has type of Effect but Effect script is not referenced");
            _inventoryItem.Effect = _effect;
        }
    }

    void Update()
    {
        
    }
}
