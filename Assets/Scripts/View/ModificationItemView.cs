using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class ModificationItemView : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("UI")]
    public Image Background;
    public Image TypeIcon;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI TypeText;

    [Header("Outline")]
    public Outline Outline;

    [Header("CanvasGroup")]
    public CanvasGroup CanvasGroup;

    [Serializable]
    public class TypeVisual
    {
        public ModificationType Type;
        public Sprite Background;
        public Sprite Icon;
        public Color BackgroundColor = Color.white;
    }

    public List<TypeVisual> TypeVisuals;

    private ModificationViewModel vm;
    private CharacterViewModel owner;

    private RectTransform rect;
    private Canvas canvas;

    private Transform originalParent;
    private Vector2 originalPos;
    private int originalIndex;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Bind(ModificationViewModel vm, CharacterViewModel owner)
    {
        this.vm = vm;
        this.owner = owner;

        NameText.text = vm.Model.Name;
        TypeText.text = vm.Model.Type.ToString();

        var visual = TypeVisuals.Find(v => v.Type == vm.Model.Type);

        if (visual != null)
        {
            Background.sprite = visual.Background;
            Background.color = visual.BackgroundColor;
            TypeIcon.sprite = visual.Icon;
        }

        vm.Highlight.OnChanged += OnHighlightChanged;

        UpdateUsedState();
        OnHighlightChanged(vm.Highlight.Value);
    }

    void OnHighlightChanged(HighlightState state)
    {
        if (Outline == null) return;

        Outline.enabled = state != HighlightState.None;

        switch (state)
        {
            case HighlightState.Compatible:
                Outline.effectColor = Color.green;
                break;

            case HighlightState.Incompatible:
                Outline.effectColor = Color.red;
                break;

            case HighlightState.Selected:
                Outline.effectColor = Color.white;
                break;
        }
    }

    void UpdateUsedState()
    {
        if (CanvasGroup != null)
        {
            bool isUsed = vm.Model.IsUsed;

            CanvasGroup.alpha = isUsed ? 0.4f : 1f;
            CanvasGroup.blocksRaycasts = !isUsed;
            CanvasGroup.interactable = !isUsed;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragContext.DraggedMod != null)
            return;

        owner.SetHoveredMod(vm);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragContext.DraggedMod != null)
            return;

        owner.ClearHover();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (vm.Model.IsUsed) return;

        DragContext.DraggedMod = vm;
        DragContext.WasDropped = false;
        owner.BeginAssignedModDragHighlight(vm);

        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex(); // чтобы потом вернуться в список с соранением иерархии
        originalPos = rect.anchoredPosition;

        transform.SetParent(canvas.transform, true);

        if (CanvasGroup != null)
            CanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragContext.DraggedMod = null;
        owner.EndAssignedModDragHighlight();

        if (CanvasGroup != null)
            CanvasGroup.blocksRaycasts = true;

        if (DragContext.WasDropped)
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalIndex);
        rect.anchoredPosition = originalPos;
    }
}