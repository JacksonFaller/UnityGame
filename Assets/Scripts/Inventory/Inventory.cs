using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private float _pickupRadius = 5f;

    [SerializeField]
    private LayerMask _mask = new LayerMask();

    [SerializeField]
    private Transform _slotsContainer;

    [SerializeField]
    private GameObject _inventory;


    // all slots objects
    List<Slot> _slots;

    // Empty slots indexes sorted to make search of first empty slot easier
    SortedSet<int> _emptySlots;

    // Dictionary with ItemID as a key and slot index as a value, to find slots with stackable items
    Dictionary<int, int> _itemsSlots;

    // Items that player picked up. Storing them so player cat drop them later
    Dictionary<int, Transform> _inventoryitems;


    void Start()
    {
        _inventoryitems = new Dictionary<int, Transform>();
        _itemsSlots = new Dictionary<int, int>();
        _slots = new List<Slot>();
        _emptySlots = new SortedSet<int>();

        for (int i = 0; i < _slotsContainer.childCount; i++)
        {
            _slots.Add(_slotsContainer.GetChild(i).GetComponent<Slot>());
            _emptySlots.Add(i);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown(Configuration.Input.Interact))
        {
            PickUpItem();
        }
        else if (Input.GetButtonDown(Configuration.Input.Inventory))
        {
            _inventory.SetActive(!_inventory.activeSelf);
        }
    }


    private void PickUpItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _pickupRadius, _mask);
        if (colliders.Length > 0)
        {
            Transform closestItem = null;
            float minDistance = Mathf.Infinity;
            foreach (var collider in colliders)
            {
                float distance = (collider.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = collider.transform;
                }
            }
            var inventoryItem = closestItem.GetComponent<InventoryItem>();
            AddItemToInventorySlot(inventoryItem.InventoryItemObject);
            closestItem.SetParent(transform);
            closestItem.localPosition = Vector3.zero;
            closestItem.gameObject.SetActive(false);
            _inventoryitems.Add(inventoryItem.InventoryItemObject.ItemID, closestItem);
        }
    }

    private void AddItemToInventorySlot(InventoryItemObject item)
    {
        // Non stackable items
        if (item.StackSize == 1)
        {
            var slotIndex = _emptySlots.Min;
            _slots[slotIndex].SetItem(item);
            // Remove from the empty slots list
            _emptySlots.Remove(slotIndex);
        }
        else
        {
            if (_itemsSlots.ContainsKey(item.ItemID))
            {
                var slot = _slots[_itemsSlots[item.ItemID]];
                // This is just adds +1 to the stack
                slot.AddItem(item);
                // Reached max stack size, remove slot from dictionary
                if (slot.ItemsAmount == item.StackSize)
                    _itemsSlots.Remove(item.ItemID);

            }
        }
    }
}
