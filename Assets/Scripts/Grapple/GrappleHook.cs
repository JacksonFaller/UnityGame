using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private Grapple _grapple = null;

    public event Action<Collider2D> OnGrappleHitTarget;
    public event Action OnGrappleHitWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Configuration.Tags.GrapplePoint))
        {
            OnGrappleHitTarget?.Invoke(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Configuration.GroundLayer.ContainsLayer(collision.gameObject.layer))
        {
            OnGrappleHitWall?.Invoke();
        }
    }
}