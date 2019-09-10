using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridLayout))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Transform _gridCellPrefab;

    [SerializeField]
    private float _rowCount;

    [SerializeField]
    private float _columnCount;

    private GridLayout _gridLayout;

    void Start()
    {
        _gridLayout = GetComponent<GridLayout>();
        for (int i = 0; i < _rowCount * _columnCount; i++)
        {
            Instantiate(_gridCellPrefab).SetParent(transform);
        }
    }

    void Update()
    {
    }
}
