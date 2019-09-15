using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Inverntory item", menuName = "Inventory item")]
[Serializable]
public class InventoryItemObject
{
    public int Id => Name.GetHashCode();
    public Sprite Image;
    public string Name;
    public string Description;
    public int SellValue;
    public int StackSize = 1;
    public MonoBehaviour Effect;
    public List<StatModifier> StatModifiers;

    public InventoryItemObject Copy()
    {
        var copy = (InventoryItemObject)this.MemberwiseClone();
        copy.StatModifiers = new List<StatModifier>();
        return copy;
    }
}


[Serializable]
public class StatModifier
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
