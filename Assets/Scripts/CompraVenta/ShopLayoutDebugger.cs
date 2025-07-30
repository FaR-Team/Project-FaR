using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopLayoutDebugger : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private bool _showDebugInfo = true;
    
    private VerticalLayoutGroup _verticalLayout;
    private ShopListLayout _shopListLayout;
    
    private void Start()
    {
        _verticalLayout = GetComponent<VerticalLayoutGroup>();
        _shopListLayout = GetComponent<ShopListLayout>();
    }
    
    private void Update()
    {
        if (_showDebugInfo && _verticalLayout != null)
        {
            // This will help you see the current spacing values in the inspector
            Debug.Log($"Spacing: {_verticalLayout.spacing}, " +
                     $"Child Count: {transform.childCount}, " +
                     $"Padding: {_verticalLayout.padding.top}/{_verticalLayout.padding.bottom}");
        }
    }
    
    [ContextMenu("Force Refresh Layout")]
    public void ForceRefreshLayout()
    {
        if (_shopListLayout != null)
        {
            _shopListLayout.RefreshLayout();
        }
        
        if (_verticalLayout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
    
    [ContextMenu("Add Layout Elements to Children")]
    public void AddLayoutElementsToChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<LayoutElement>() == null)
            {
                var layoutElement = child.gameObject.AddComponent<LayoutElement>();
                layoutElement.preferredHeight = _shopListLayout != null ? _shopListLayout.GetItemHeight() : 80f;
                layoutElement.flexibleHeight = 0;
            }
        }
    }
}