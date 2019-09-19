using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(GridLayout))]
[RequireComponent(typeof(RectTransform))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    [Range(1, 20)]
    private uint _rowCount = 1;

    [SerializeField]
    [Range(1, 20)]
    private uint _columnCount = 10;

    [SerializeField]
    private float _cellSize = 20f;

    [SerializeField]
    private float _textSize = 10f;

    [SerializeField]
    private Vector2 _spacing = Vector2.zero;

    [SerializeField]
    private Transform _slotPrefab = null;

    [SerializeField]
    private RectTransform _slotsContainer = null;

    [SerializeField]
    private RectTransform _panel = null;

    private float _currentTextSize;
    private GridLayoutGroup _slotsGrid;
    private List<Transform> _slotsPool = new List<Transform>();

    void Start()
    {
        _slotsGrid = _slotsContainer.GetComponent<GridLayoutGroup>();
        Repaint();
    }

#if (UNITY_EDITOR)
    void Update()
    {
        Repaint();
    }
#endif

    [ContextMenu("Refresh")]
    private void Repaint()
    {
        _slotsGrid.cellSize = new Vector3(_cellSize, _cellSize);
        _slotsGrid.spacing = _spacing;
        _slotsContainer.sizeDelta = new Vector2(_cellSize * _columnCount + _spacing.x * (_columnCount - 1),
            _cellSize * _rowCount + _spacing.y * (_rowCount - 1));

        _panel.sizeDelta = _slotsContainer.sizeDelta;

        uint cellsCount = _rowCount * _columnCount;

        int activeCellsCount = _slotsContainer.childCount - _slotsPool.Count;

        for (int i = activeCellsCount; i < cellsCount; i++)
        {
            Transform newCell;
            if (_slotsPool.Count > 0)
            {
                newCell = _slotsPool[_slotsPool.Count - 1];
                _slotsPool.Remove(newCell);
                newCell.gameObject.SetActive(true);
            }
            else
            {
                newCell = Instantiate(_slotPrefab);
                newCell.name = (_slotsContainer.childCount).ToString();
                newCell.SetParent(_slotsContainer);
            }
        }

        for (int i = activeCellsCount; i > cellsCount; i--)
        {
            var cell = _slotsContainer.GetChild(i - 1);
            cell.gameObject.SetActive(false);
            _slotsPool.Add(cell.GetComponent<RectTransform>());
        }

        if(_currentTextSize != _textSize)
        {
            UpdateTextSize();
            _currentTextSize = _textSize;
        }
    }

    [ContextMenu("Update text size")]
    public void UpdateTextSize()
    {
        Vector2 size = new Vector2(_textSize, _textSize);
        for (int i = 0; i < _slotsContainer.childCount; i++)
        {
            _slotsContainer.GetChild(i).GetComponent<Slot>().UpdateTextSize(size);
        }
    }

    [ContextMenu("Update slots")]
    public void ClearSlots()
    {
        _slotsPool.Clear();
        List<GameObject> children = new List<GameObject>();
        foreach(Transform child in _slotsContainer.transform)
        {
            children.Add(child.gameObject);
        }

        for(int i = 0; i < children.Count; i++)
        {
            DestroyImmediate(children[i]);
        }

        Repaint();
    }
}
