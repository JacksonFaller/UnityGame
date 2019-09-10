using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New inverntory item", menuName = "Inventory item")]
public class InventoryItemObject : ScriptableObject
{
    public Sprite Image;
    public string Name;
    public string Description;
    public int SellValue;
    public AbilityType AbilityType;
    public float StatModifier;
    public MonoBehaviour Effect;
}

public enum AbilityType
{
    StatModifier,
    StatMultiplier,
    Effect
}
