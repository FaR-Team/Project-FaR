using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class InventorySlot_UIAbility : InventorySlot_UIBasic
{
    public static bool IsHoeUnlocked = true;
    public static bool IsBucketUnlocked = true;
    public static bool IsShovelUnlocked = true;
    public static bool IsBlank2Unlocked = false;
    public static bool IsBlank3Unlocked = false;

    [ShowInInspector]
    public static bool[] isUnlocked => new bool[] { IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked };

    public static bool IsAbilityUnlocked(int index)
    {
        bool[] unlocked = isUnlocked;
        return unlocked != null && index >= 0 && index < unlocked.Length && unlocked[index];
    }

    public static bool isAbilityHotbarActive;

    [Header("Slot Background Assets")]
    [SerializeField] private Sprite middleSlotSprite;
    [SerializeField] private Sprite mediumSlotSprite;
    [SerializeField] private Sprite smallSlotSprite;

    [Header("Carousel Sub-Slot Configuration")]
    [SerializeField] private Vector2 middleSlotSize = new Vector2(64f, 64f);
    [SerializeField] private Vector2 mediumSlotSize = new Vector2(48f, 48f);
    [SerializeField] private Vector2 smallSlotSize = new Vector2(36f, 36f);
    [SerializeField] private float mediumVerticalOffset = 45f;
    [SerializeField] private float smallVerticalOffset = 80f;

    [Header("Icon Scaling & Padding")]
    [SerializeField] private Vector2 iconPaddingMiddle = new Vector2(-24f, -24f);
    [SerializeField] private Vector2 iconPaddingMedium = new Vector2(-20f, -20f);

    private RectTransform _subSlotsContainer;
    private AbilitySubSlotUI[] _subSlots; // 0: Top (-2), 1: Upper (-1), 2: Middle (0), 3: Lower (+1), 4: Bottom (+2)
    private Coroutine _scrollCoroutine;

    [System.Serializable]
    private class AbilitySubSlotUI
    {
        public RectTransform rectTransform;
        public Image bgImage;
        public Image iconImage;
        public int offset;
    }

    private struct SubSlotConfig
    {
        public Vector2 position;
        public Vector2 size;
        public Sprite bgSprite;
    }

    private struct SlotAnimState
    {
        public Vector2 startPos;
        public Vector2 targetPos;
        public Vector2 startSize;
        public Vector2 targetSize;
        public Vector2 startIconPadding;
        public Vector2 targetIconPadding;
        public Sprite startBg;
        public Sprite targetBg;
        public Sprite startIcon;
        public Sprite targetIcon;
        public bool isExpanding;
    }

    private SubSlotConfig GetSubSlotConfig(int offset)
    {
        int abs = Mathf.Abs(offset);
        Vector2 size = abs == 0 ? middleSlotSize : (abs == 1 ? mediumSlotSize : smallSlotSize);
        Sprite sprite = abs == 0 ? middleSlotSprite : (abs == 1 ? (mediumSlotSprite != null ? mediumSlotSprite : middleSlotSprite) : (smallSlotSprite != null ? smallSlotSprite : mediumSlotSprite));

        float y = offset switch
        {
            -2 => smallVerticalOffset,
            -1 => mediumVerticalOffset,
            0 => 0f,
            1 => -mediumVerticalOffset,
            2 => -smallVerticalOffset,
            < -2 => smallVerticalOffset + mediumVerticalOffset,
            _ => -(smallVerticalOffset + mediumVerticalOffset)
        };

        return new SubSlotConfig { position = new Vector2(0f, y), size = size, bgSprite = sprite };
    }

    protected override void Awake()
    {
        base.Awake();
        EnsureSpritesLoaded();
        SetupSubSlotsContainer();
    }

    private void Start()
    {
        isAbilityHotbarActive = false;
    }

    private void Update()
    {
        UpdateSurroundingSlotsVisibility();
    }

    private void UpdateSurroundingSlotsVisibility()
    {
        if (_subSlots == null) return;

        bool showSurrounding = HotbarDisplayBase.CurrentIndexIsSpecialSlotAndYouAreHoldingCtrl();

        for (int i = 0; i < _subSlots.Length; i++)
        {
            if (_subSlots[i]?.rectTransform != null)
            {
                _subSlots[i].rectTransform.gameObject.SetActive(_subSlots[i].offset == 0 || showSurrounding);
            }
        }
    }

    public void ApplySubSlotConfigurations()
    {
        if (_subSlots == null || _subSlots.Length < 5) return;

        for (int i = 0; i < _subSlots.Length; i++)
        {
            AbilitySubSlotUI slotUI = _subSlots[i];
            if (slotUI?.rectTransform == null) continue;

            SubSlotConfig config = GetSubSlotConfig(slotUI.offset);
            slotUI.rectTransform.sizeDelta = config.size;

            if (_scrollCoroutine == null)
            {
                slotUI.rectTransform.anchoredPosition = config.position;
            }

            if (slotUI.bgImage != null)
            {
                slotUI.bgImage.sprite = config.bgSprite;
                slotUI.bgImage.color = config.bgSprite != null ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }

            if (slotUI.iconImage != null)
            {
                RectTransform iconRect = slotUI.iconImage.rectTransform;
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.sizeDelta = (slotUI.offset == 0) ? iconPaddingMiddle : iconPaddingMedium;
                iconRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    private void EnsureSpritesLoaded()
    {
#if UNITY_EDITOR
        if (middleSlotSprite == null)
            middleSlotSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arte/GUI/HotbarSlot-1.png");
        if (mediumSlotSprite == null)
            mediumSlotSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arte/GUI/HotbarSlot-2.png");
        if (smallSlotSprite == null)
            smallSlotSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arte/GUI/HotbarSlot-3.png");
#endif
    }

    private void SetupSubSlotsContainer()
    {
        if (itemSprite != null)
        {
            itemSprite.color = Color.clear;
        }

        Transform existingContainer = transform.Find("SubSlotsContainer");
        if (existingContainer != null)
        {
            _subSlotsContainer = existingContainer.GetComponent<RectTransform>();
        }
        else
        {
            GameObject containerGo = new GameObject("SubSlotsContainer", typeof(RectTransform));
            containerGo.transform.SetParent(transform, false);
            _subSlotsContainer = containerGo.GetComponent<RectTransform>();
            _subSlotsContainer.anchorMin = new Vector2(0.5f, 0.5f);
            _subSlotsContainer.anchorMax = new Vector2(0.5f, 0.5f);
            _subSlotsContainer.sizeDelta = Vector2.zero;
            _subSlotsContainer.anchoredPosition = Vector2.zero;
        }

        int[] offsets = { -2, -1, 0, 1, 2 };
        _subSlots = new AbilitySubSlotUI[5];

        for (int i = 0; i < 5; i++)
        {
            int offset = offsets[i];
            Transform existingSlot = _subSlotsContainer.Find($"SubSlot_{offset}");
            GameObject slotGo;
            RectTransform slotRect;
            Image bgImg;
            Image iconImg;

            if (existingSlot != null)
            {
                slotGo = existingSlot.gameObject;
                slotRect = slotGo.GetComponent<RectTransform>();
                bgImg = slotGo.GetComponent<Image>();
                iconImg = slotGo.transform.Find("Icon")?.GetComponent<Image>();
            }
            else
            {
                slotGo = new GameObject($"SubSlot_{offset}", typeof(RectTransform), typeof(Image));
                slotGo.transform.SetParent(_subSlotsContainer, false);
                slotRect = slotGo.GetComponent<RectTransform>();
                bgImg = slotGo.GetComponent<Image>();

                GameObject iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                iconGo.transform.SetParent(slotGo.transform, false);
                iconImg = iconGo.GetComponent<Image>();
            }

            SubSlotConfig config = GetSubSlotConfig(offset);

            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.sizeDelta = config.size;
            slotRect.anchoredPosition = config.position;
            slotRect.pivot = new Vector2(0.5f, 0.5f);

            bgImg.sprite = config.bgSprite;
            bgImg.preserveAspect = true;
            bgImg.color = config.bgSprite != null ? Color.white : new Color(1f, 1f, 1f, 0.5f);

            if (iconImg != null)
            {
                RectTransform iconRect = iconImg.rectTransform;
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.sizeDelta = (offset == 0) ? iconPaddingMiddle : iconPaddingMedium;
                iconRect.anchoredPosition = Vector2.zero;
                iconImg.preserveAspect = true;
                iconImg.color = Color.clear;
            }

            _subSlots[i] = new AbilitySubSlotUI
            {
                rectTransform = slotRect,
                bgImage = bgImg,
                iconImage = iconImg,
                offset = offset
            };
        }
    }

    public override void ToggleHighlight()
    {
        isAbilityHotbarActive = true;
        if (_slotHighlight != null)
        {
            _slotHighlight.SetActive(!_slotHighlight.activeInHierarchy);
        }
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
        if (itemSprite != null)
        {
            itemSprite.color = Color.clear;
        }
    }

    private void OnDisable()
    {
        CleanUpLastScroll();
    }

    public void UpdateAbilityCarousel(ToolItemData[] abilityTools, int currentAbilityIndex, int direction = 0)
    {
        if (_subSlots == null || _subSlots.Length < 5)
        {
            SetupSubSlotsContainer();
        }

        CleanUpLastScroll();

        if (direction != 0)
        {
            _scrollCoroutine = StartCoroutine(CarouselScrollTransition(abilityTools, currentAbilityIndex, direction));
        }
        else
        {
            SetCarouselIcons(abilityTools, currentAbilityIndex);
            ResetSubSlotPositions();
        }
    }

    private List<int> GetUnlockedToolIndices(ToolItemData[] abilityTools)
    {
        List<int> unlockedIndices = new List<int>();
        if (abilityTools == null) return unlockedIndices;

        for (int i = 0; i < abilityTools.Length; i++)
        {
            if (IsAbilityUnlocked(i))
            {
                unlockedIndices.Add(i);
            }
        }
        return unlockedIndices;
    }

    private void SetCarouselIcons(ToolItemData[] abilityTools, int currentAbilityIndex)
    {
        if (abilityTools == null || abilityTools.Length == 0) return;

        List<int> unlockedIndices = GetUnlockedToolIndices(abilityTools);

        if (unlockedIndices.Count == 0)
        {
            for (int i = 0; i < _subSlots.Length; i++)
            {
                SetImageSprite(_subSlots[i].iconImage, null);
            }
            return;
        }

        int posInUnlocked = unlockedIndices.IndexOf(currentAbilityIndex);
        if (posInUnlocked < 0) posInUnlocked = 0;

        int count = unlockedIndices.Count;

        for (int i = 0; i < _subSlots.Length; i++)
        {
            int offset = _subSlots[i].offset;
            bool isSmallestSlot = Mathf.Abs(offset) >= 2;
            int cyclicPos = ((posInUnlocked + offset) % count + count) % count;
            ToolItemData tool = abilityTools[unlockedIndices[cyclicPos]];

            Sprite iconSprite = (tool != null && !isSmallestSlot) ? tool.Icono : null;
            SetImageSprite(_subSlots[i].iconImage, iconSprite);
        }
    }

    private void ResetSubSlotPositions()
    {
        if (_subSlots == null) return;
        for (int i = 0; i < _subSlots.Length; i++)
        {
            if (_subSlots[i]?.rectTransform != null)
            {
                SubSlotConfig config = GetSubSlotConfig(_subSlots[i].offset);
                _subSlots[i].rectTransform.anchoredPosition = config.position;
                _subSlots[i].rectTransform.sizeDelta = config.size;
                if (_subSlots[i].bgImage != null)
                {
                    _subSlots[i].bgImage.sprite = config.bgSprite;
                }
            }
        }
    }

    private IEnumerator CarouselScrollTransition(ToolItemData[] abilityTools, int currentAbilityIndex, int direction)
    {
        if (_subSlots == null || _subSlots.Length < 5) yield break;

        List<int> unlockedIndices = GetUnlockedToolIndices(abilityTools);
        if (unlockedIndices.Count == 0) yield break;

        int targetPosInUnlocked = unlockedIndices.IndexOf(currentAbilityIndex);
        if (targetPosInUnlocked < 0) targetPosInUnlocked = 0;
        int count = unlockedIndices.Count;

        int prevPosInUnlocked = ((targetPosInUnlocked - direction) % count + count) % count;

        SlotAnimState[] animStates = new SlotAnimState[5];

        for (int i = 0; i < 5; i++)
        {
            int targetOffset = _subSlots[i].offset;
            int startOffset = targetOffset + direction;

            SubSlotConfig startCfg = GetSubSlotConfig(startOffset);
            SubSlotConfig targetCfg = GetSubSlotConfig(targetOffset);

            int targetCyclicPos = ((targetPosInUnlocked + targetOffset) % count + count) % count;
            ToolItemData targetTool = abilityTools[unlockedIndices[targetCyclicPos]];

            int startCyclicPos = ((prevPosInUnlocked + startOffset) % count + count) % count;
            ToolItemData startTool = abilityTools[unlockedIndices[startCyclicPos]];

            animStates[i] = new SlotAnimState
            {
                startPos = startCfg.position,
                targetPos = targetCfg.position,
                startSize = startCfg.size,
                targetSize = targetCfg.size,
                startIconPadding = (startOffset == 0) ? iconPaddingMiddle : iconPaddingMedium,
                targetIconPadding = (targetOffset == 0) ? iconPaddingMiddle : iconPaddingMedium,
                startBg = startCfg.bgSprite,
                targetBg = targetCfg.bgSprite,
                startIcon = (startTool != null && Mathf.Abs(startOffset) < 2) ? startTool.Icono : null,
                targetIcon = (targetTool != null && Mathf.Abs(targetOffset) < 2) ? targetTool.Icono : null,
                isExpanding = targetCfg.size.sqrMagnitude > startCfg.size.sqrMagnitude
            };
        }

        const float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / duration);
            float t = Mathf.SmoothStep(0f, 1f, rawT);

            for (int i = 0; i < 5; i++)
            {
                var slot = _subSlots[i];
                if (slot?.rectTransform == null) continue;

                var state = animStates[i];

                slot.rectTransform.anchoredPosition = Vector2.Lerp(state.startPos, state.targetPos, t);
                slot.rectTransform.sizeDelta = Vector2.Lerp(state.startSize, state.targetSize, t);

                float swapThreshold = state.isExpanding ? 0.25f : 0.70f;
                Sprite currentBg = (rawT >= swapThreshold) ? state.targetBg : state.startBg;

                if (slot.bgImage != null)
                {
                    slot.bgImage.sprite = currentBg;
                    slot.bgImage.color = currentBg != null ? Color.white : new Color(1f, 1f, 1f, 0.5f);
                }

                if (slot.iconImage != null)
                {
                    slot.iconImage.rectTransform.sizeDelta = Vector2.Lerp(state.startIconPadding, state.targetIconPadding, t);
                }

                Sprite currentIcon = (rawT >= 0.5f) ? state.targetIcon : state.startIcon;
                SetImageSprite(slot.iconImage, currentIcon);
            }

            yield return null;
        }

        SetCarouselIcons(abilityTools, currentAbilityIndex);
        ResetSubSlotPositions();
        _scrollCoroutine = null;
    }

    private static void SetImageSprite(Image img, Sprite sprite)
    {
        if (img == null) return;
        img.sprite = sprite;
        img.color = sprite != null ? Color.white : Color.clear;
    }

    private void CleanUpLastScroll()
    {
        if (_scrollCoroutine != null)
        {
            StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = null;
        }
        ResetSubSlotPositions();
    }
}