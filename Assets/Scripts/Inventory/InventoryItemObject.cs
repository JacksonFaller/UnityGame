using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(fileName = "Inverntory item", menuName = "Inventory item")]
[Serializable]
public class InventoryItemObject
{
    [SerializeField, ReadOnly]
    private int _itemID;
    public int ItemID { get => _itemID; private set => _itemID = value; }
    public int Id => Name.GetHashCode();
    public Sprite Image;
    public string Name;
    public string Description;
    public int SellValue;
    public int StackSize = 1;
    public MonoBehaviour Effect;
    public List<StatModifier> StatModifiers;

    public InventoryItemObject()
    {
        StatModifiers = new List<StatModifier>();
    }

    public InventoryItemObject Copy(int itemID)
    {
        var copy = (InventoryItemObject)this.MemberwiseClone();
        copy.StatModifiers = StatModifiers.ToList();
        copy.ItemID = itemID;
        return copy;
    }

    public InventoryItemObject Copy()
    {
        return Copy(ItemID);
    }
}


[Serializable]
public struct StatModifier
{
    public Stat Stat;
    public float Modifier;
    public bool IsMultiplier;
}

public enum Stat
{
    Health,
    Mana,
    Stamina,
    Attack,
    Defence
}
