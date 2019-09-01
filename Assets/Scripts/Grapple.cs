using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private const string _grappleButton = "Grapple";

    [SerializeField]
    private float _throwSpeed = 2f;

    [SerializeField]
    private float _swingSpeed = 5f;

    [SerializeField]
    private float _pushDelay = 1f;

    [SerializeField]
    private LayerMask _grappleObjectsLayer;

    [SerializeField]
    private Movement _movement;

    [SerializeField]
    private Rigidbody2D _playerRigidbody2D;

    [SerializeField]
    private DistanceJoint2D _distanceJoint2D;

    [SerializeField]
    private Rigidbody2D _grappleRigidbody2D;

    private bool _isGrappled;

    private float _pushTimer;


    void Start()
    { 
    }

    void Update()
    {
        if (_isGrappled)
        {
            if (Input.GetButtonUp(_grappleButton))
            {
                _grappleRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                _grappleRigidbody2D.gameObject.SetActive(false);
                _movement.enabled = true;
                _distanceJoint2D.enabled = false;
                _isGrappled = false;
            }
            else
            {
                _pushTimer -= Time.deltaTime;
                float x = Input.GetAxis("Horizontal");
                if(Mathf.Abs(x) > 0.1)
                {
                    // TODO: add restrictions on how much velocity you can gain per swing
                    if (_pushTimer <= 0)
                    {
                        if((_playerRigidbody2D.velocity.x > 0 && x > 0) || (_playerRigidbody2D.velocity.x < 0 && x < 0))
                        {
                            _playerRigidbody2D.velocity = new Vector2(x, 0).normalized * _swingSpeed;
                            _pushTimer = _pushDelay;
                        }
                    }
                }
            }
          
        }
        else
        {
            // Throw grapple
            if (Input.GetButtonDown(_grappleButton))
            {
                _grappleRigidbody2D.gameObject.SetActive(true);
                _grappleRigidbody2D.transform.position = _movement.transform.position;

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 directinon = new Vector2(mousePosition.x - _grappleRigidbody2D.transform.position.x, 
                                                mousePosition.y - _grappleRigidbody2D.transform.position.y);
                _grappleRigidbody2D.velocity = directinon.normalized * _throwSpeed;
            }
        }
    }

    public void GrappleHit(Collider2D collision)
    {
        _movement.enabled = false;
        _isGrappled = true;
        _grappleRigidbody2D.transform.position = collision.gameObject.transform.position;
        _distanceJoint2D.enabled = true;
        _grappleRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _grappleRigidbody2D.velocity = Vector2.zero;
    }
}
