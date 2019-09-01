using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private Grapple _grapple;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _grapple.GrappleHit(collision);
    }
}
