using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Grapple : MonoBehaviour
{
    #region Inspector values

    [SerializeField]
    private float _throwSpeed = 2f;

    [SerializeField]
    private float _swingSpeed = 5f;

    [SerializeField]
    private float _swingSpeedLerp = 10f;

    [SerializeField]
    private float _maxFlyDistance = 20f;

    [SerializeField]
    private LayerMask _grappleObjectsLayer;

    [SerializeField]
    private Rigidbody2D _playerRigidbody2D = null;

    [SerializeField]
    private Rigidbody2D _grappleRigidbody2D = null;

    [SerializeField]
    private Collision _playerCollision = null;

    #endregion

    private HingeJoint2D _playerHingeJoint2D;
    private Movement _playerMovement;
    private BetterJumping _betterJumping;
    private GameObject _grapplePoint;

    private LineRenderer _lineRenderer;

    private bool _isGrappled;
    private bool _isReturning;
    private bool _isInUse;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _playerHingeJoint2D = _playerRigidbody2D.GetComponent<HingeJoint2D>();
        _playerMovement = _playerRigidbody2D.GetComponent<Movement>();
        _betterJumping = _playerRigidbody2D.GetComponent<BetterJumping>();
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
            if (Input.GetButtonDown(Configuration.Input.JumpButton))
            {
                DetachGrapple();
            }
            else
            {
                _lineRenderer.SetPosition(1, new Vector3(_playerRigidbody2D.position.x, _playerRigidbody2D.position.y));
                float x = Input.GetAxis(Configuration.Input.HorizontalAxis);

                if (x > 0 && _playerRigidbody2D.velocity.x >= 0 || x < 0 && _playerRigidbody2D.velocity.x <= 0)
                {
                    _playerRigidbody2D.velocity = Vector2.Lerp(_playerRigidbody2D.velocity,
                        new Vector2(_playerRigidbody2D.velocity.x + x * _swingSpeed, _playerRigidbody2D.velocity.y), _swingSpeedLerp * Time.deltaTime);
                }
            }
        }
        else if (_isInUse)
        {
            DrawRope();
            ReturnGrapple();
        }
        else if (Input.GetButtonDown(Configuration.Input.GrappleButton))
        {
            // Throw grapple
            _isInUse = true;
            _grappleRigidbody2D.transform.position = _playerRigidbody2D.position;
            StartCoroutine(WaitForEndOfFrameAndThrow());
        }
    }

    private void DrawRope()
    {
        _lineRenderer.SetPosition(0, new Vector3(_grappleRigidbody2D.position.x, _grappleRigidbody2D.position.y));
        _lineRenderer.SetPosition(1, new Vector3(_playerRigidbody2D.position.x, _playerRigidbody2D.position.y));
    }

    private void ReturnGrapple()
    {
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

    private void DetachGrapple()
    {
        _grapplePoint.SetActive(false);
        _grappleRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _playerMovement.enabled = true;
        _betterJumping.enabled = true;
        _playerHingeJoint2D.enabled = false;
        _isGrappled = false;
        _isReturning = true;
    }

    public void GrappleHitTraget(Rigidbody2D grappleJoint)
    {
        if (_isReturning) return;

        _grapplePoint = grappleJoint.gameObject;
        _grapplePoint.SetActive(true);

        _playerMovement.enabled = false;
        _betterJumping.enabled = false;
        _isGrappled = _isInUse = true;
        _grappleRigidbody2D.bodyType = RigidbodyType2D.Static;
        _grappleRigidbody2D.position = grappleJoint.position;
        _playerHingeJoint2D.enabled = true;
        _playerHingeJoint2D.connectedBody = grappleJoint;

        DrawRope();
    }

    public void GrappleHitWall()
    {
        var hitResults = new RaycastHit2D[1];
        var filter = new ContactFilter2D() { layerMask = Configuration.GroundLayer };
        int hitCount = _grappleRigidbody2D.Cast(_playerRigidbody2D.position - _grappleRigidbody2D.position, filter, hitResults);

        // Player is behind a wall and grapple is stuck, so have to disable it
        if (hitCount > 0 && _isReturning)
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
