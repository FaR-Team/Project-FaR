using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPagination : MonoBehaviour
{
    [Header("Pagination Settings")]
    [SerializeField] private int _itemsPerPage = 6;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _pageText;
    
    private int _currentPage = 0;
    private int _totalPages = 0;
    private List<GameObject> _allItems = new List<GameObject>();
    
    public System.Action<int, int> OnPageChanged; // currentPage, itemsPerPage
    
    private void Awake()
    {
        if (_previousButton != null)
            _previousButton.onClick.AddListener(PreviousPage);
        
        if (_nextButton != null)
            _nextButton.onClick.AddListener(NextPage);
    }
    
    public void Initialize(List<GameObject> items)
    {
        _allItems = new List<GameObject>(items);
        _currentPage = 0;
        CalculateTotalPages();
        UpdatePageDisplay();
        UpdateButtonStates();
    }
    
    public void SetItemsPerPage(int itemsPerPage)
    {
        _itemsPerPage = itemsPerPage;
        _currentPage = 0;
        CalculateTotalPages();
        UpdatePageDisplay();
        UpdateButtonStates();
    }
    
    private void CalculateTotalPages()
    {
        _totalPages = Mathf.CeilToInt((float)_allItems.Count / _itemsPerPage);
        if (_totalPages == 0) _totalPages = 1;
    }
    
    public void UpdatePageDisplay()
    {
        foreach (var item in _allItems)
        {
            if (item != null)
                item.SetActive(false);
        }
        
        int startIndex = _currentPage * _itemsPerPage;
        int endIndex = Mathf.Min(startIndex + _itemsPerPage, _allItems.Count);
        
        for (int i = startIndex; i < endIndex; i++)
        {
            if (_allItems[i] != null)
                _allItems[i].SetActive(true);
        }
        
        if (_pageText != null)
        {
            _pageText.text = $"{_currentPage + 1} / {_totalPages}";
        }
        
        OnPageChanged?.Invoke(_currentPage, _itemsPerPage);
    }
    
    private void UpdateButtonStates()
    {
        if (_previousButton != null)
            _previousButton.interactable = _currentPage > 0;
        
        if (_nextButton != null)
            _nextButton.interactable = _currentPage < _totalPages - 1;
    }
    
    public void PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            UpdatePageDisplay();
            UpdateButtonStates();
        }
    }
    
    public void NextPage()
    {
        if (_currentPage < _totalPages - 1)
        {
            _currentPage++;
            UpdatePageDisplay();
            UpdateButtonStates();
        }
    }
    
    public void GoToPage(int pageIndex)
    {
        pageIndex = Mathf.Clamp(pageIndex, 0, _totalPages - 1);
        _currentPage = pageIndex;
        UpdatePageDisplay();
        UpdateButtonStates();
    }
    
    public int GetCurrentPage() => _currentPage;
    public int GetTotalPages() => _totalPages;
    public int GetItemsPerPage() => _itemsPerPage;
}