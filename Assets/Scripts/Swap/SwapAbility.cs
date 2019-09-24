using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SwapAbility : MonoBehaviour
{
    [SerializeField]
    private float _maxSize = 30f;

    [SerializeField]
    private float _growTime = 1f;

    [SerializeField]
    private float _shrinkTime = 0.5f;

    [SerializeField]
    private float _cooldownTime = 5f;

    [SerializeField]
    private float _useTime = 5f;

    [SerializeField]
    private float _timeScale = 0.5f;

    [SerializeField]
    private float _markOffsetY = 0.6f;

    [SerializeField]
    private GameObject _markPrefab = null;

    [SerializeField]
    private Collision _playerCollision = null;

    private SwapTarget _playerSwapTarget;
    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;
    private float _cooldownTimer;
    private float _useTimer;

    private bool _onCooldown;
    private bool _inUse;

    private Dictionary<Transform, SwapTarget> _targets = new Dictionary<Transform, SwapTarget>(2);
    private HashSet<Transform> _availableTargets;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerTransform = transform.parent;
        _playerSwapTarget = _playerCollision.GetComponent<SwapTarget>();
        _availableTargets = new HashSet<Transform>();
    }

    void Update()
    {
        if (_onCooldown)
        {
            if (_cooldownTimer <= 0)
            {
                _onCooldown = false;
            }
            else
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetButtonDown(Configuration.Input.SwapButton))
            {
                if (_inUse)
                {
                    SwapFinished(true);
                }
                else
                {
                    _spriteRenderer.enabled = true;
                    _spriteRenderer.transform.DOScale(new Vector3(_maxSize, _maxSize), _growTime).SetUpdate(true);
                    Time.timeScale = _timeScale;
                    _useTimer = _useTime;
                    _inUse = true;
                }

            }
            if (_inUse)
            {
                if (_useTimer <= 0)
                {
                    SwapFinished();
                }
                else
                {
                    _useTimer -= Time.deltaTime / Time.timeScale;
                    if (Input.GetButtonDown(Configuration.Input.CancelButton))
                    {
                        RemoveSwapMarks();
                        _targets.Clear();
                        SwapFinished();
                    }
                    else if (Input.GetButtonDown(Configuration.Input.ActionButton))
                    {
                        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var hitTransform = Physics2D.Raycast(mousePosition, Vector2.zero).transform;
                        if (hitTransform != null)
                        {
                            if (_availableTargets.Contains(hitTransform))
                            {
                                if (_targets.ContainsKey(hitTransform))
                                {
                                    _targets[hitTransform].RemoveMark();
                                    _targets.Remove(hitTransform);
                                }
                                else
                                {
                                    SwapTarget swapTarget = hitTransform.GetComponent<SwapTarget>();
                                    swapTarget.AddMark(_markPrefab, _markOffsetY);
                                    _targets.Add(hitTransform, swapTarget);

                                    if (_targets.Count == 2)
                                        SwapFinished();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Configuration.Tags.SwapTarget))
        {
            _availableTargets.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Configuration.Tags.SwapTarget))
        {
            Transform targetTransform = collision.transform;
            if (_targets.ContainsKey(targetTransform))
            {
                _targets[targetTransform].RemoveMark();
                _targets.Remove(targetTransform);
            }
            _availableTargets.Remove(targetTransform);
        }
    }

    private void SwapFinished(bool isConfirmed = false)
    {
        DOTween.Kill(_spriteRenderer.transform);
        _spriteRenderer.transform.DOScale(Vector3.one, _shrinkTime)
            .OnComplete(() => _spriteRenderer.enabled = false); // TODO: Maybe also a good idea to disable a collider

        Time.timeScale = 1f;
        _cooldownTimer = _cooldownTime;
        _onCooldown = true;
        _inUse = false;

        SwapTargets(isConfirmed);
    }

    private void SwapTargets(bool isConfirmed)
    {
        if (_targets.Count == 0) return;

        if (_targets.Count == 1 && isConfirmed)
        {
            Transform target = _targets.Keys.ElementAt(0);
            SwapTargetsPositions2(target, _playerTransform, _targets[target], _playerSwapTarget);
        }
        else if (_targets.Count == 2)
        {
            Transform target = _targets.Keys.ElementAt(0);
            Transform target2 = _targets.Keys.ElementAt(1);

            SwapTargetsPositions2(target, target2, _targets[target], _targets[target2]);
        }
        RemoveSwapMarks();
        _targets.Clear();
    }

    private void RemoveSwapMarks()
    {
        foreach (var swapTarget in _targets.Values)
        {
            swapTarget.RemoveMark();
        }
    }


    private void SwapTargetsPositions(Transform target1, Transform target2)
    {
        var position = target1.position;
        target1.position = target2.position;
        target2.position = position;

        var rigidbody1 = target1.GetComponent<Rigidbody2D>();
        var rigidbody2 = target2.GetComponent<Rigidbody2D>();

        var velocity = rigidbody1.velocity;
        rigidbody1.velocity = rigidbody2.velocity;
        rigidbody2.velocity = velocity;
    }

    private void SwapTargetsPositions2(Transform target, Transform target2, SwapTarget targetCollider, SwapTarget target2Collider)
    {
        var targetNewPosition = GetNewTargetPosition(target2Collider, target2, targetCollider);
        target2.position = GetNewTargetPosition(targetCollider, target, target2Collider);
        target.position = targetNewPosition;

        var rigidbody = target.GetComponent<Rigidbody2D>();
        var rigidbody2 = target2.GetComponent<Rigidbody2D>();

        var velocity = rigidbody.velocity;
        rigidbody.velocity = rigidbody2.velocity;
        rigidbody2.velocity = velocity;
    }

    private Vector2 GetNewTargetPosition(SwapTarget swapTarget, Transform targetTransform,
        SwapTarget swapTarget2)
    {
        var goundFilter = new ContactFilter2D() { layerMask = Configuration.GroundLayer, useLayerMask = true };
        var hitResults = new RaycastHit2D[1];

        float x = targetTransform.position.x;
        float y = targetTransform.position.y;

        if (swapTarget.Collider.Cast(Vector2.down, goundFilter, hitResults, swapTarget2.CollisionPointsDistances.Bottom) > 0)
        {
            y = hitResults[0].point.y + swapTarget2.CollisionPointsDistances.Bottom;
        }
        else if (swapTarget.Collider.Cast(Vector2.up, goundFilter, hitResults, swapTarget2.CollisionPointsDistances.Top) > 0)
        {
            y = hitResults[0].point.y - swapTarget2.CollisionPointsDistances.Top;
        }

        if (swapTarget.Collider.Cast(Vector2.right, goundFilter, hitResults, swapTarget2.CollisionPointsDistances.Right) > 0)
        {
            x = hitResults[0].point.x - swapTarget2.CollisionPointsDistances.Right;
        }
        else if (swapTarget.Collider.Cast(Vector2.left, goundFilter, hitResults, swapTarget2.CollisionPointsDistances.Left) > 0)
        {
            x = hitResults[0].point.x + swapTarget2.CollisionPointsDistances.Left;
        }

        return new Vector2(x, y);
    }

    private float ApplyOffset(float basePosition, Collider2D collider2D, Transform target)
    {
        return basePosition + collider2D.bounds.size.y / 2 - collider2D.offset.y * target.localScale.y;
    }
}