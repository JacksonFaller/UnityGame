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

    [SerializeField]
    private Collision _collision;

    private bool _isJumping;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_isJumping)
        {
            if (_collision.OnGround && _rigidbody2D.velocity.y == 0)
            {
                _isJumping = false;
                return;
            }

            if (_rigidbody2D.velocity.y < 0)
            {
                _rigidbody2D.velocity += GetVelocity(_fallMultiplier);
            }
            else if (_rigidbody2D.velocity.y > 0 && !Input.GetButton(Configuration.Input.JumpButton))
            {
                _rigidbody2D.velocity += GetVelocity(_lowJumpMultiplier);
            }
        }
        else if(Input.GetButtonDown(Configuration.Input.JumpButton) && _collision.OnGround)
        {
            _isJumping = true;
        }
    }

    private Vector2 GetVelocity(float multiplier)
    {
        return Vector2.up * Physics2D.gravity.y * (multiplier - 1) * Time.deltaTime;
    }
}
