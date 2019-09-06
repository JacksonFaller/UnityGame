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
    private Transform _markPrefab = null;

    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;
    private float _cooldownTimer;
    private float _useTimer;

    private bool _onCooldown;
    private bool _inUse;

    private Dictionary<Transform, Transform> _targets = new Dictionary<Transform, Transform>(2);

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerTransform = transform.parent;
    }

    void Update()
    {
        if (_onCooldown)
        {
            if(_cooldownTimer <= 0)
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
                if(_inUse)
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
            if(_inUse)
            {
                if(_useTimer <= 0)
                {
                    SwapFinished();
                }
                else
                {
                    _useTimer -= Time.deltaTime;
                    if(Input.GetButtonDown(Configuration.Input.CancelButton))
                    {
                        RemoveSwapMarks();
                        _targets.Clear();
                        SwapFinished();
                    }
                    else if(Input.GetButtonDown(Configuration.Input.ActionButton))
                    {
                        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                        if (hit.transform != null)
                        {
                            if(hit.transform.tag == Configuration.Tags.SwapTarget && !_targets.ContainsKey(hit.transform))
                            {
                                var mark = Instantiate(_markPrefab);
                                mark.SetParent(hit.transform);
                                mark.localPosition = Vector3.zero;
                                _targets.Add(hit.transform, mark);

                                if (_targets.Count == 2)
                                    SwapFinished();
                            }
                        }
                    }
                }
            }
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
        if(_targets.Count == 1 && isConfirmed)
        {
            SwapTargetsPositions(_targets.Keys.ElementAt(0), _playerTransform);
        }
        else if (_targets.Count == 2)
        {
            SwapTargetsPositions(_targets.Keys.ElementAt(0), _targets.Keys.ElementAt(1));
        }
        RemoveSwapMarks();
        _targets.Clear();
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
}