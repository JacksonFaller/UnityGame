using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Inverntory item", menuName = "Inventory item")]
public class ItemsDatabase : ScriptableObject
{
    public Dictionary<int, InventoryItemObject> Items;
    public InventoryItemObject InventoryItem;

    public ItemsDatabase()
    {
        Items = new Dictionary<int, InventoryItemObject>();
    }
}
