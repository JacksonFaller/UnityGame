using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField]
    private float _bounceVelocity;

    [SerializeField]
    private float _minVelocity;

    [SerializeField]
    private float _maxVelocity;

    [SerializeField]
    private Vector2 _direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Configuration.Tags.Player)) return;

        collision.attachedRigidbody.velocity = _direction * 
            Mathf.Clamp(collision.attachedRigidbody.velocity.magnitude + _bounceVelocity, _minVelocity, _maxVelocity);
    }
}
