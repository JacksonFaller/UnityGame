using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridLayout))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Transform _gridCellPrefab = null;

    [SerializeField]
    private int _rowCount = 1;

    [SerializeField]
    private int _columnCount = 10;

    private GridLayout _gridLayout = null;

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
