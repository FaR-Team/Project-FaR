using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class ShopListLayout : MonoBehaviour
{
    [Header("List Settings")]
    [SerializeField] private int _itemsPerPage = 5;
    [SerializeField] private float _itemHeight = 80f;
    [SerializeField] private float _spacing = 10f;
    [SerializeField] private RectOffset _padding;
    
    private VerticalLayoutGroup _verticalLayout;
    private RectTransform _rectTransform;
    
    private void Awake()
    {
        if (_padding == null)
        {
            _padding = new RectOffset(10, 10, 10, 10);
        }
        
        _verticalLayout = GetComponent<VerticalLayoutGroup>();
        _rectTransform = GetComponent<RectTransform>();
        SetupList();
    }
    
    private void SetupList()
    {
        if (_verticalLayout == null) return;
        
        _verticalLayout.spacing = _spacing;
        _verticalLayout.padding = _padding;
        _verticalLayout.childAlignment = TextAnchor.UpperCenter;
        _verticalLayout.childControlHeight = false;
        _verticalLayout.childControlWidth = true;
        _verticalLayout.childForceExpandHeight = false;
        _verticalLayout.childForceExpandWidth = true;
        _verticalLayout.childScaleHeight = false;
        _verticalLayout.childScaleWidth = false;
        
        float totalHeight = (_itemsPerPage * _itemHeight) + ((_itemsPerPage - 1) * _spacing) + _padding.top + _padding.bottom;
        
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, totalHeight);
    }
    
    public void SetItemsPerPage(int itemsPerPage)
    {
        _itemsPerPage = itemsPerPage;
        SetupList();
    }
    
    public void SetItemHeight(float itemHeight)
    {
        _itemHeight = itemHeight;
        SetupList();
    }
    
    public void SetSpacing(float spacing)
    {
        _spacing = spacing;
        SetupList();
    }
    
    public int GetItemsPerPage()
    {
        return _itemsPerPage;
    }
    
    public float GetItemHeight()
    {
        return _itemHeight;
    }
    
    public void RefreshLayout()
    {
        SetupList();
        
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                LayoutElement layoutElement = child.GetComponent<LayoutElement>();
                if (layoutElement == null)
                {
                    layoutElement = child.gameObject.AddComponent<LayoutElement>();
                }
                
                layoutElement.preferredHeight = _itemHeight;
                layoutElement.flexibleHeight = 0;
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }
    
    private void Start()
    {
        Invoke(nameof(RefreshLayout), 0.1f);
    }
}