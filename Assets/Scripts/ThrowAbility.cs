using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThrowAbility : MonoBehaviour
{
    [SerializeField]
    private Collision _collision = null;

    [SerializeField]
    private SpriteRenderer _playerSpriterRenderer;
    private Transform _pickedUpItem;
    private HashSet<Transform> _collectableItems;

    private bool _flip = false;

    void Start()
    {
        _collectableItems = new HashSet<Transform>();
        _collision.OnCollisionEnter += CollisionEnter2D;
        _collision.OnCollisionExit += CollisionExit2D;
    }

    void Update()
    {
        if(_pickedUpItem != null && _playerSpriterRenderer.flipX != _flip)
        {
            _pickedUpItem.localPosition = new Vector3(_pickedUpItem.localPosition.x * -1, 0);
            _flip = _playerSpriterRenderer.flipX;
        }

        if(Input.GetButtonDown(Configuration.Input.ActionButton))
        {
            if (_collectableItems.Count > 0)
            {
                CollectItems();
                Transform item = _collectableItems.FirstOrDefault();
                item.SetParent(transform);
                _flip = _playerSpriterRenderer.flipX;
                item.localPosition = new Vector3(_flip? -1f: 1f, 0);
                _pickedUpItem = item;
                _collectableItems.Clear();
            }
            else if(_pickedUpItem != null)
            {
                _pickedUpItem.SetParent(null);
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                Rigidbody2D rigidbody = _pickedUpItem.gameObject.AddComponent<Rigidbody2D>();
                rigidbody.velocity = mousePosition.normalized * 20f;
            }
        }
    }

    private void CollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectables"))
        {
            _collectableItems.Add(collision.transform);
        }
    }

    private void CollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectables"))
        {
            _collectableItems.Remove(collision.transform);
        }
    }

    private void CollectItems()
    {
        Debug.Log($"Collected {_collectableItems.Count} items");
    }
}
