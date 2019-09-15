using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThrowAbility : MonoBehaviour
{
    [SerializeField]
    private Collision _collision = null;

    [SerializeField]
    private SpriteRenderer _playerSpriterRenderer = null;

    [SerializeField]
    private float _throwSpeed = 30f;

    [SerializeField]
    private float _holdDistanceX = 0.7f;

    [SerializeField]
    private float _holdDistanceY = 0f;

    private Transform _throwableObject;

    private bool _isCarrying;

    private bool _flip = false;

    void Start()
    {
        _collision.OnTriggerEnter += CollisionEnter2D;
        _collision.OnTriggerExit += CollisionExit2D;
    }

    void Update()
    {
        if(_isCarrying && _throwableObject != null && _playerSpriterRenderer.flipX != _flip)
        {
            _throwableObject.localPosition = new Vector3(_throwableObject.localPosition.x * -1, _throwableObject.localPosition.y);
            _flip = _playerSpriterRenderer.flipX;
        }

        if(Input.GetButtonDown(Configuration.Input.ActionButton))
        {
            // Pick up object
            if (_throwableObject != null && !_isCarrying)
            {
                CollectItems();
                _throwableObject.SetParent(transform);
                _flip = _playerSpriterRenderer.flipX;
                _throwableObject.localPosition = new Vector3(_flip? _holdDistanceX * -1: _holdDistanceX, _holdDistanceY);
                _isCarrying = true;

                var throwable = _throwableObject.GetComponent<ThrowableObject>();
                throwable.Catch();
            }
            // Throw object
            else if(_isCarrying)
            {
                _throwableObject.SetParent(null);
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                var throwable = _throwableObject.GetComponent<ThrowableObject>();
                throwable.enabled = true;
                throwable.Throw(mousePosition.normalized * _throwSpeed);
                _isCarrying = false;
                _throwableObject = null;
            }
        }
    }

    private void CollisionEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Configuration.Tags.ThrowableObject))
        {
            _throwableObject = collision.transform.parent;
        }
    }

    private void CollisionExit2D(Collider2D collision)
    {
        if (!_isCarrying && collision.CompareTag(Configuration.Tags.ThrowableObject))
        {
            _throwableObject = null;
        }
    }

    private void CollectItems()
    {
    }
}
