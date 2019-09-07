using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private Grapple _grapple = null;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Configuration.Tags.GrapplePoint))
        {
            GameObject grappleJoint = collision.transform.GetChild(0).gameObject;
            grappleJoint.transform.rotation = Quaternion.identity;
            _grapple.GrappleHitTraget(grappleJoint.GetComponent<Rigidbody2D>());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Configuration.GroundLayer.ContainsLayer(collision.gameObject.layer))
        {
            _grapple.GrappleHitWall();
        }
    }
}