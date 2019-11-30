using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal _linkedPortal;

    [SerializeField, EnumFlag]
    private ExitPosition _exitPosition;

    [SerializeField]
    private bool _negativeX;

    [SerializeField]
    private bool _negativeY;

    private bool _isExiting;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Configuration.Tags.Player) || _isExiting) return;
        collision.transform.parent.position = _linkedPortal.transform.position;
        var velocity = collision.attachedRigidbody.velocity;
        collision.attachedRigidbody.velocity =
            new Vector2(_linkedPortal._negativeX ? -Mathf.Abs(velocity.x) : Mathf.Abs(velocity.x),
            _linkedPortal._negativeY ? -Mathf.Abs(velocity.y) : Mathf.Abs(velocity.y));

        _linkedPortal._isExiting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(Configuration.Tags.Player)) return;
        _isExiting = false;
    }

    [Flags]
    public enum ExitPosition
    {
        Up = 1,
        Down = 2,
        Right = 4,
        Left = 8
    }
}
