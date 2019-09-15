using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class ThrowableObject : MonoBehaviour
{
    [SerializeField]
    private float _gravityScale = 3f;

    [SerializeField]
    private float _mass = 1f;

    [SerializeField]
    private float _drag = 0f;

    private Collider2D _collider;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector2 groundCollisionPoint = new Vector2(transform.position.x, _collider.bounds.min.y);
        if (Physics2D.OverlapPoint(groundCollisionPoint, Configuration.GroundLayer))
        {
            Catch();
        }
    }

    public void Throw(Vector2 velocity)
    {
        Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.freezeRotation = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigidbody.velocity = velocity;
        rigidbody.gravityScale = _gravityScale;
        rigidbody.drag = _drag;
        rigidbody.mass = _mass;
    }

    public void Catch()
    {
        Destroy(GetComponent<Rigidbody2D>());
        this.enabled = false;
    }
}
