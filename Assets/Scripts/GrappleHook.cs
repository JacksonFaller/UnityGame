using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private Grapple _grapple = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _grapple.GrappleHitTraget(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Configuration.GroundLayer.ContainsLayer(collision.gameObject.layer))
        {
            _grapple.GrappleHitWall();
        }
    }
}