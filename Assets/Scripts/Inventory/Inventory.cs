using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private float _pickupRadius = 5f;

    [SerializeField]
    private LayerMask _mask;

    public MonoBehaviour Script;

    private HashSet<InventoryItemObject> _items;

    private List<ItemStack> _itemStacks;

    void Start()
    {
        _items = new HashSet<InventoryItemObject>();
        _itemStacks = new List<ItemStack>();
    }

    void Update()
    {
        if(Input.GetButtonDown(Configuration.Input.Interact))
        {
            PickUpItem();
        }
        
    }

    private void PickUpItem()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        var filter = new ContactFilter2D() { layerMask = _mask, useLayerMask = true };
        if(Physics2D.OverlapCircle(transform.position, _pickupRadius, filter, colliders) > 0)
        {
            Debug.Log(colliders.Count);
            Transform closestItem = null;
            float minDistance = Mathf.Infinity;
            foreach(var collider in colliders)
            {
                float distance = (collider.transform.position - transform.position).sqrMagnitude;
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = collider.transform;
                }
            }
            var inventoryItem = closestItem.GetComponent<InventoryItem>();
            _items.Add(inventoryItem.InventoryItemObject);
            closestItem.SetParent(transform);
            closestItem.localPosition = Vector3.zero;
            closestItem.gameObject.SetActive(false);
        }
    }

}

public class ItemStack
{
    public int Count { get; private set; }

    public int StackSize => ItemObject.StackSize;

    public InventoryItemObject ItemObject { get; }

    public ItemStack(InventoryItemObject itemObject)
    {
        ItemObject = itemObject;
    }

    public int Add(int count = 1)
    {
        Count += count;
        if(Count > ItemObject.StackSize)
        {
            count = Count - ItemObject.StackSize;
            Count = ItemObject.StackSize;
        }
        return count;
    }

    public int Remove(int count = 1)
    {
        if (count > Count)
            count = Count;

        Count-= count;
        return count;
    }
}
