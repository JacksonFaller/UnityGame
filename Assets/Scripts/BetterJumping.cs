using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BetterJumping : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    [SerializeField]
    private float _fallMultiplier = 3f;

    [SerializeField]
    private float _lowJumpMultiplier = 8f;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(_rigidbody2D.velocity.y < 0)
        {
            _rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if(_rigidbody2D.velocity.y > 0 && !Input.GetButton(Configuration.Input.JumpButton))
        {
            _rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
