using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inverntory item", menuName = "Inventory item")]
public class InventoryItemObject : ScriptableObject
{
    public int Id;
    public Sprite Image;
    public string Name;
    public string Description;
    public int SellValue;
    public int StackSize = 1;
    public MonoBehaviour Effect;
    public List<StatModifier> StatModifiers;
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
