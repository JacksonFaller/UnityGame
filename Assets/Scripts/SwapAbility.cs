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
    private Transform _markPrefab = null;

    [SerializeField]
    private Collision _playerCollision;

    private Collider2D _playerCollider2D;
    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;
    private float _cooldownTimer;
    private float _useTimer;

    private bool _onCooldown;
    private bool _inUse;

    private Dictionary<Transform, Transform> _targets = new Dictionary<Transform, Transform>(2);
    private HashSet<Transform> _availableTargets;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerTransform = transform.parent;
        _playerCollider2D = _playerCollision.GetComponent<Collider2D>();
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
                    _spriteRenderer.transform.DOScale(new Vector3(_maxSize, _maxSize, 1f), _growTime).SetUpdate(true);
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
                    _useTimer -= Time.deltaTime;
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
                                    Destroy(_targets[hitTransform].gameObject);
                                    _targets.Remove(hitTransform);
                                }
                                else
                                {
                                    var mark = Instantiate(_markPrefab);
                                    mark.SetParent(hitTransform.transform);
                                    mark.localPosition = new Vector3(0, _markOffsetY);
                                    _targets.Add(hitTransform.transform, mark);

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
            if(_targets.ContainsKey(targetTransform))
            {
                Destroy(_targets[targetTransform].gameObject);
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
            SwapTargetsPositions2(target, _playerTransform, target.GetComponent<Collider2D>(), _playerCollider2D);
        }
        else if (_targets.Count == 2)
        {
            Transform target = _targets.Keys.ElementAt(0);
            Transform target2 = _targets.Keys.ElementAt(1);

            SwapTargetsPositions2(target, target2, target.GetComponent<Collider2D>(), target2.GetComponent<Collider2D>());
        }
        RemoveSwapMarks();
        _targets.Clear();
    }

    private float GetNewTargetPosition(Collider2D targetCollider, Transform targetTransform, 
        Collider2D target2Collider, Transform target2Transform)
    {
        var goundFilter = new ContactFilter2D() { layerMask = Configuration.GroundLayer };
        var hitResults = new RaycastHit2D[1];
        float traget2NewYPos = targetTransform.position.y;

        if (targetCollider.Cast(Vector2.down, goundFilter, hitResults) > 0)
        {
            float targetGroundY = hitResults[0].point.y;
            if (Mathf.Abs(targetGroundY - targetCollider.bounds.min.y) < 0.05f)
            {
                traget2NewYPos = ApplyOffset(targetGroundY, target2Collider, target2Transform);
                if (targetTransform.position.y > traget2NewYPos)
                    traget2NewYPos = targetTransform.position.y;
            }
        }

        return traget2NewYPos;
    }

    private float ApplyOffset(float basePosition, Collider2D collider2D, Transform target)
    {
        return basePosition + collider2D.bounds.size.y / 2 - collider2D.offset.y * target.localScale.y;
    }

    private void RemoveSwapMarks()
    {
        foreach (var marks in _targets.Values)
        {
            Destroy(marks.gameObject);
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

    private void SwapTargetsPositions2(Transform target, Transform target2, Collider2D targetCollider, Collider2D target2Collider)
    {
        float targetNewYPos;
        float target2NewYPos = GetNewTargetPosition(targetCollider, target, target2Collider, target2);

        if (_playerCollision.OnGround)
        {
            float target2GroundY = target2Collider.bounds.min.y;
            targetNewYPos = ApplyOffset(target2GroundY, targetCollider, target);

            if (target2.position.y > targetNewYPos)
                targetNewYPos = target2.position.y;
        }
        else
        {
            targetNewYPos = target2.position.y;
        }

        float position = target.position.x;
        target.position = new Vector2(target2.position.x, targetNewYPos);
        target2.position = new Vector3(position, target2NewYPos);

        var rigidbody = target.GetComponent<Rigidbody2D>();
        var rigidbody2 = target2.GetComponent<Rigidbody2D>();

        var velocity = rigidbody.velocity;
        rigidbody.velocity = rigidbody2.velocity;
        rigidbody2.velocity = velocity;
    }
}