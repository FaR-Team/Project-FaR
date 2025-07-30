using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ShopGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int _columns = 3;
    [SerializeField] private int _rows = 2;
    [SerializeField] private Vector2 _cellSize = new Vector2(150, 150);
    [SerializeField] private Vector2 _spacing = new Vector2(10, 10);
    [SerializeField] private RectOffset _padding = new RectOffset(10, 10, 10, 10);
    
    private GridLayoutGroup _gridLayout;
    private RectTransform _rectTransform;
    
    private void Awake()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        _rectTransform = GetComponent<RectTransform>();
        SetupGrid();
    }
    
    private void SetupGrid()
    {
        if (_gridLayout == null) return;
        
        _gridLayout.cellSize = _cellSize;
        _gridLayout.spacing = _spacing;
        _gridLayout.padding = _padding;
        _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayout.constraintCount = _columns;
        _gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        _gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        _gridLayout.childAlignment = TextAnchor.UpperCenter;
        
        float totalWidth = (_columns * _cellSize.x) + ((_columns - 1) * _spacing.x) + _padding.left + _padding.right;
        float totalHeight = (_rows * _cellSize.y) + ((_rows - 1) * _spacing.y) + _padding.top + _padding.bottom;
        
        _rectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
    }
    
    public void SetGridDimensions(int columns, int rows)
    {
        _columns = columns;
        _rows = rows;
        SetupGrid();
    }
    
    public void SetCellSize(Vector2 cellSize)
    {
        _cellSize = cellSize;
        SetupGrid();
    }
    
    public int GetItemsPerPage()
    {
        return _columns * _rows;
    }
}