using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

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

    private RectTransform rect;
    private Canvas canvas;

    private Transform originalParent;
    private Vector2 originalPos;
    private int originalIndex;
    private bool canDrag = true;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        HoverContext.OnHoverChanged += UpdateHighlight;
    }

    void OnDestroy()
    {
        HoverContext.OnHoverChanged -= UpdateHighlight;
    }

    public void Bind(ModificationViewModel vm)
    {
        this.vm = vm;

        NameText.text = vm.Model.Name;
        TypeText.text = vm.Model.Type.ToString();

        var visual = TypeVisuals.Find(v => v.Type == vm.Model.Type);

        if (visual != null)
        {
            Background.sprite = visual.Background;
            Background.color = visual.BackgroundColor;
            TypeIcon.sprite = visual.Icon;
        }

        canDrag = !vm.Model.IsUsed;

        UpdateUsedState();
        UpdateHighlight();
    }

    void UpdateUsedState()
    {
        if (CanvasGroup == null) return;

        CanvasGroup.alpha = vm.Model.IsUsed ? 0.4f : 1f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        DragContext.DraggedMod = vm;
        DragContext.WasDropped = false;

        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex(); // чтобы потом вернуться в список с соранением иерархии
        originalPos = rect.anchoredPosition;

        transform.SetParent(canvas.transform, true);

        if (CanvasGroup != null)
            CanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        DragContext.DraggedMod = null;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverContext.SetMod(vm);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverContext.Clear();
    }

    void UpdateHighlight()
    {
        if (vm == null) return;

        if (Outline != null)
            Outline.enabled = false;

        if (HoverContext.HoveredAbility != null)
        {
            var ability = HoverContext.HoveredAbility.Model;
            var assigned = ability.AssignedModification;

            if (assigned != null)
            {
                if (assigned == vm.Model)
                {
                    if (Outline != null)
                    {
                        Outline.enabled = true;
                        //Outline.effectColor = Color.black;
                        Outline.effectColor = Color.white;
                    }
                    return;
                }

                bool compatible =
                    ability.SupportedTypes.Contains(vm.Model.Type);

                if (compatible && Outline != null)
                {
                    Outline.enabled = true;
                    Outline.effectColor = Color.green;
                }

                return;
            }

            bool isCompatible =
                ability.SupportedTypes.Contains(vm.Model.Type);

            if (Outline != null)
            {
                Outline.enabled = true;
                Outline.effectColor = isCompatible ? Color.green : Color.red;
            }
        }
    }
}