using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SwapTarget : MonoBehaviour
{
    [SerializeField]
    private Color _outlineColor = default;

    [SerializeField]
    private float _outlineToOriginalScale = 1.1f;

    private SpriteRenderer _spriteRenderer = null;
    private GameObject _outlineObject;

    // Distance from the object position to the ground (when gounded)
    public CollisionPointsDistances CollisionPointsDistances { get; private set; }

    public Collider2D Collider { get; private set; }

    private GameObject _mark;

    protected GameObject OutlineObject
    {
        get
        {
            if (_outlineObject == null)
            {
                _outlineObject = new GameObject("Outline");
                var spriteRenderer = _outlineObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = _spriteRenderer.sprite;
                spriteRenderer.material = _spriteRenderer.material;
                spriteRenderer.material.shader = Shader.Find("GUI/Text Shader");
                spriteRenderer.material.color = _outlineColor;
                _outlineObject.transform.localScale = transform.localScale * _outlineToOriginalScale;
                _outlineObject.transform.SetParent(transform.parent);
                _outlineObject.transform.localPosition = Vector3.zero;
            }
            return _outlineObject;
        }

    }

    void Start()
    {
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
        CollisionPointsDistances = new CollisionPointsDistances
        {
            Top = Collider.bounds.max.y - transform.position.y,
            Bottom = transform.position.y - Collider.bounds.min.y,
            Left = transform.position.x - Collider.bounds.min.x,
            Right = Collider.bounds.max.x - transform.position.x
        };
    }

    void Update()
    {
    }

    public void AddMark(GameObject mark, float offsetY)
    {
        if(_mark == null)
        {
            _mark = Instantiate(mark);
            _mark.transform.SetParent(transform);
            _mark.transform.localPosition = new Vector3(0, offsetY);
        }
        _mark.SetActive(true);
    }

    public void RemoveMark()
    {
        _mark.SetActive(false);
    }
}

public class CollisionPointsDistances
{
    public float Top { get; set; }
    public float Bottom { get; set; }
    public float Left { get; set; }
    public float Right { get; set; }
}