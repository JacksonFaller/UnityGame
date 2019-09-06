using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class SwapTarget : MonoBehaviour
{
    [SerializeField]
    private Color _outlineColor = default;

    [SerializeField]
    private float _outlineToOriginalScale = 1.1f;

    private SpriteRenderer _spriteRenderer;
    private GameObject _outlineObject;

    private string _tag;


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
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Configuration.Tags.SwapField)
        {
            _tag = transform.tag;
            transform.tag = Configuration.Tags.SwapTarget;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Configuration.Tags.SwapField)
        {
            transform.tag = _tag;
        }
    }

    void OnMouseDown()
    {
    }

    void OnMouseEnter()
    {
        //OutlineObject.SetActive(true);
    }

    void OnMouseExit()
    {
        //OutlineObject.SetActive(false);
    }
}
