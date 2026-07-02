using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InventorySlot_UIAbility : InventorySlot_UIBasic
{
    public bool TESTING;
    public static bool IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked;

    [ShowInInspector] public static bool[] isUnlocked = {IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked};

    public static bool isAbilityHotbarActive;

    private Coroutine _scrollCoroutine;
    private GameObject _tempGo;
    private Vector2 _originalSpritePos = Vector2.zero;

    void Start()
    {
        if (TESTING)
        {
            IsHoeUnlocked = true;
            IsBucketUnlocked = true;
            IsShovelUnlocked = true;

            isUnlocked = new bool[] {IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked};
        }
        isAbilityHotbarActive = false;
    }

    public override void ToggleHighlight()
    {
        isAbilityHotbarActive = true;
        _slotHighlight.SetActive(!_slotHighlight.activeInHierarchy);
    }

    public override void Init(InventorySlot slot)
    {
        CleanUpLastScroll();
        base.Init(slot);
    }

    public override void UpdateUISlot(InventorySlot slot)
    {
        CleanUpLastScroll();
        base.UpdateUISlot(slot);
    }

    private void OnDisable()
    {
        CleanUpLastScroll();
    }

    public void UpdateUISlotWithScroll(int direction)
    {
        if (assignedInventorySlot == null || assignedInventorySlot.ItemData == null)
        {
            CleanUpLastScroll();
            UpdateUISlot();
            return;
        }

        if (itemSprite == null)
        {
            CleanUpLastScroll();
            UpdateUISlot();
            return;
        }

        Sprite oldSprite = itemSprite.sprite;
        Sprite newSprite = assignedInventorySlot.ItemData.Icono;

        if (oldSprite == null || newSprite == null || oldSprite == newSprite)
        {
            CleanUpLastScroll();
            UpdateUISlot();
            return;
        }

        CleanUpLastScroll();

        if (GetComponent<RectMask2D>() == null && GetComponent<Mask>() == null)
        {
            gameObject.AddComponent<RectMask2D>();
        }

        _scrollCoroutine = StartCoroutine(ScrollTransition(oldSprite, newSprite, direction));
    }

    private void CleanUpLastScroll()
    {
        if (_scrollCoroutine != null)
        {
            StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = null;
        }
        if (_tempGo != null)
        {
            Destroy(_tempGo);
            _tempGo = null;
        }
        if (itemSprite != null)
        {
            RectTransform rect = itemSprite.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = _originalSpritePos;
            }
            itemSprite.color = Color.white;
        }
    }

    private IEnumerator ScrollTransition(Sprite oldSprite, Sprite newSprite, int direction)
    {
        RectTransform itemRect = itemSprite.GetComponent<RectTransform>();
        if (itemRect == null)
        {
            itemSprite.sprite = newSprite;
            yield break;
        }

        _originalSpritePos = Vector2.zero;

        float height = 64f;
        RectTransform slotRect = transform as RectTransform;
        if (slotRect != null)
        {
            height = slotRect.rect.height;
        }

        _tempGo = new GameObject("TempScrollImage", typeof(Image));
        _tempGo.transform.SetParent(itemSprite.transform.parent, false);
        Image tempImage = _tempGo.GetComponent<Image>();
        
        RectTransform tempRect = _tempGo.GetComponent<RectTransform>();
        tempRect.anchorMin = itemRect.anchorMin;
        tempRect.anchorMax = itemRect.anchorMax;
        tempRect.anchoredPosition = _originalSpritePos;
        tempRect.sizeDelta = itemRect.sizeDelta;
        tempRect.pivot = itemRect.pivot;
        tempRect.localScale = itemRect.localScale;
        
        tempImage.sprite = oldSprite;
        tempImage.preserveAspect = itemSprite.preserveAspect;
        tempImage.color = Color.white;

        itemSprite.sprite = newSprite;
        itemSprite.color = Color.white;

        float startY = direction * height;
        float endY = -direction * height;

        itemRect.anchoredPosition = new Vector2(_originalSpritePos.x, startY);

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            if (tempRect != null)
            {
                tempRect.anchoredPosition = new Vector2(_originalSpritePos.x, Mathf.Lerp(_originalSpritePos.y, endY, t));
            }

            if (itemRect != null)
            {
                itemRect.anchoredPosition = new Vector2(_originalSpritePos.x, Mathf.Lerp(startY, _originalSpritePos.y, t));
            }

            yield return null;
        }

        CleanUpLastScroll();
    }
}