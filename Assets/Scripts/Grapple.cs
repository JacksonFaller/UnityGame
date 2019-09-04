using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Grapple : MonoBehaviour
{
    private const string _grappleButton = "Grapple";
    private const string _jumpButton = "Jump";

    #region Inspector values

    [SerializeField]
    private float _throwSpeed = 2f;

    [SerializeField]
    private float _swingSpeed = 5f;

    [SerializeField]
    private float _pushDelay = 1f;

    [SerializeField]
    private float _maxFlyDistance = 20f;

    [SerializeField]
    private LayerMask _grappleObjectsLayer;

    [SerializeField]
    private Rigidbody2D _playerRigidbody2D;

    [SerializeField]
    private Rigidbody2D _grappleRigidbody2D;

    [SerializeField]
    private LayerMask _groundLayerMask;

    #endregion

    private DistanceJoint2D _playerDistanceJoint2D;
    private Collision _playerCollision;
    private Movement _playerMovement;

    private LineRenderer _lineRenderer;

    private bool _isGrappled;
    private bool _isReturning;
    private bool _isInUse;
    private float _pushTimer;

    // TODO: Consider using configuration object with layer names and stuff
    public LayerMask GroundLayer => _groundLayerMask;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _playerCollision = _playerRigidbody2D.GetComponent<Collision>();
        _playerDistanceJoint2D = _playerRigidbody2D.GetComponent<DistanceJoint2D>();
        _playerMovement = _playerRigidbody2D.GetComponent<Movement>();
    }

    void Update()
    {
        if (_isGrappled)
        {
            if (_playerCollision.OnWall || _playerCollision.OnGround)
            {
                _isGrappled = false;
                DetachGrapple();
                return;
            }

            // Release grapple
            if (Input.GetButtonDown(_jumpButton))
            {
                DetachGrapple();
            }
            else
            {
                _pushTimer -= Time.deltaTime;
                _lineRenderer.SetPosition(1, new Vector3(_playerRigidbody2D.position.x, _playerRigidbody2D.position.y));
                float x = Input.GetAxis("Horizontal");
                if (Mathf.Abs(x) > 0.1)
                {
                    // TODO: add restrictions on how much velocity you can gain per swing
                    if (_pushTimer <= 0)
                    {
                        if ((_playerRigidbody2D.velocity.x > 0 && x > 0) || (_playerRigidbody2D.velocity.x < 0 && x < 0))
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
                if (!_isInUse)
                {
                    _isInUse = true;
                    _grappleRigidbody2D.transform.position = _playerRigidbody2D.position;
                    StartCoroutine(WaitForEndOfFrameAndThrow());
                }
            }
            else
            {
                if (_isInUse)
                {
                    _lineRenderer.SetPosition(0, new Vector3(_grappleRigidbody2D.position.x, _grappleRigidbody2D.position.y));
                    _lineRenderer.SetPosition(1, new Vector3(_playerRigidbody2D.position.x, _playerRigidbody2D.position.y));

                    float distance = Vector2.Distance(_grappleRigidbody2D.position, _playerRigidbody2D.position);
                    if (_isReturning)
                    {
                        if (distance <= 0.5f)
                        {
                            DisableGrapple();
                        }
                        else
                        {
                            Vector2 direction = _playerRigidbody2D.position - _grappleRigidbody2D.position;
                            _grappleRigidbody2D.velocity = direction.normalized * _throwSpeed;
                        }
                    }
                    else
                    {
                        if (distance >= _maxFlyDistance)
                            _isReturning = true;
                    }
                }
            }
        }
    }

    private void DetachGrapple()
    {
        _grappleRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _playerMovement.enabled = true;
        _playerDistanceJoint2D.enabled = false;
        _isGrappled = false;
        _isReturning = true;
    }

    public void GrappleHitTraget(Collider2D collision)
    {
        if (_isReturning) return;

        _playerMovement.enabled = false;
        _isGrappled = _isInUse = true;
        _grappleRigidbody2D.position = collision.transform.position;
        _playerDistanceJoint2D.enabled = true;
        _grappleRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _grappleRigidbody2D.velocity = Vector2.zero;

        _lineRenderer.SetPosition(0, new Vector3(_grappleRigidbody2D.position.x, _grappleRigidbody2D.position.y));
        _lineRenderer.SetPosition(1, new Vector3(_playerRigidbody2D.position.x, _playerRigidbody2D.position.y));
    }

    public void GrappleHitWall()
    {
        var hitResults = new RaycastHit2D[1];
        var filter = new ContactFilter2D() { layerMask = _groundLayerMask };
        int hitCount = _grappleRigidbody2D.Cast(_playerRigidbody2D.position - _grappleRigidbody2D.position, filter, hitResults);

        if(hitCount > 0 && _isReturning)
        {
            DisableGrapple();
            return;
        }
        _isReturning = true;
    }

    private void DisableGrapple()
    {
        _lineRenderer.enabled = false;
        _grappleRigidbody2D.gameObject.SetActive(false);
        _isInUse = false;
        _isReturning = false;
    }

    private IEnumerator WaitForEndOfFrameAndThrow()
    {
        yield return new WaitForEndOfFrame();
        _lineRenderer.enabled = true;
        _grappleRigidbody2D.gameObject.SetActive(true);
        //yield return new WaitForFixedUpdate();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directinon = new Vector2(mousePosition.x - _grappleRigidbody2D.position.x,
                                        mousePosition.y - _grappleRigidbody2D.position.y);
        _grappleRigidbody2D.velocity = directinon.normalized * _throwSpeed;
    }
}
