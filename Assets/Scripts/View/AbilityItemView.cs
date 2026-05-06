using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AbilityItemView : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("UI")]
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Name;

    [Header("Outline")]
    public Outline Outline;

    [Header("Модификатор в способности")]
    public GameObject AssignedModRoot;
    public Image ModBackground;
    public Image ModIcon;

    [System.Serializable]
    public class TypeVisual
    {
        public ModificationType Type;
        public Sprite Background;
        public Sprite Icon;
        public Color BackgroundColor = Color.white;
    }

    public List<TypeVisual> TypeVisuals;

    private AbilityViewModel vm;
    private CharacterViewModel owner;
    private RectTransform assignedModRect;
    private Canvas rootCanvas;
    private CanvasGroup assignedModCanvasGroup;
    private GameObject dragPreviewInstance;
    private RectTransform dragPreviewRect;
    private bool isDraggingAssignedMod;

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();

        if (AssignedModRoot != null)
        {
            assignedModRect = AssignedModRoot.GetComponent<RectTransform>();
            assignedModCanvasGroup = AssignedModRoot.GetComponent<CanvasGroup>();

            if (assignedModCanvasGroup == null)
            {
                assignedModCanvasGroup = AssignedModRoot.AddComponent<CanvasGroup>();
            }
        }
    }

    void OnDisable()
    {
        CleanupAssignedModDragState(clearGlobalContext: isDraggingAssignedMod);
    }

    void OnDestroy()
    {
        CleanupAssignedModDragState(clearGlobalContext: isDraggingAssignedMod);
    }

    public void Bind(AbilityViewModel vm, CharacterViewModel owner)
    {
        this.vm = vm;
        this.owner = owner;

        Name.text = vm.Model.Name;
        Icon.sprite = vm.Model.Icon;

        vm.Highlight.OnChanged += OnHighlightChanged;

        UpdateAssignedMod();
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

    void UpdateAssignedMod()
    {
        var mod = vm.Model.AssignedModification;

        if (mod == null)
        {
            AssignedModRoot.SetActive(false);
            return;
        }

        AssignedModRoot.SetActive(true);

        var visual = TypeVisuals.Find(v => v.Type == mod.Type);

        if (visual != null)
        {
            ModBackground.sprite = visual.Background;
            ModBackground.color = visual.BackgroundColor;
            ModIcon.sprite = visual.Icon;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragContext.DraggedMod != null)
            return;

        owner.SetHoveredAbility(vm);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragContext.DraggedMod != null)
            return;

        owner.ClearHover();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragContext.DraggedMod == null)
            return;

        var mod = DragContext.DraggedMod.Model;

        if (!vm.Model.SupportedTypes.Contains(mod.Type))
            return;

        var command = new AssignModificationCommand(mod, vm.Model);
        command.Execute();

        DragContext.WasDropped = true;

        owner.NotifyDataChanged();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (vm.Model.AssignedModification == null)
            return;

        var command = new RemoveModificationCommand(vm.Model);
        command.Execute();

        owner.NotifyDataChanged();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraggingAssignedMod = false;

        if (vm == null || vm.Model == null || vm.Model.AssignedModification == null)
            return;

        if (assignedModRect == null || rootCanvas == null)
            return;

        bool startedOnAssignedMod = RectTransformUtility.RectangleContainsScreenPoint(
            assignedModRect,
            eventData.position,
            eventData.pressEventCamera);

        if (!startedOnAssignedMod)
            return;

        var draggedModVm = owner.Mods.Find(m => m.Model == vm.Model.AssignedModification);
        if (draggedModVm == null)
            return;

        DragContext.DraggedMod = draggedModVm;
        DragContext.WasDropped = false;
        isDraggingAssignedMod = true;
        owner.BeginAssignedModDragHighlight(draggedModVm);

        dragPreviewInstance = Instantiate(AssignedModRoot, rootCanvas.transform);
        dragPreviewRect = dragPreviewInstance.GetComponent<RectTransform>();

        var previewCanvasGroup = dragPreviewInstance.GetComponent<CanvasGroup>();
        if (previewCanvasGroup == null)
            previewCanvasGroup = dragPreviewInstance.AddComponent<CanvasGroup>();
        previewCanvasGroup.blocksRaycasts = false;
        previewCanvasGroup.interactable = false;

        dragPreviewRect.position = eventData.position;

        if (assignedModCanvasGroup != null)
            assignedModCanvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingAssignedMod || dragPreviewRect == null)
            return;

        dragPreviewRect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingAssignedMod)
            return;
        CleanupAssignedModDragState(clearGlobalContext: true);
    }

    void CleanupAssignedModDragState(bool clearGlobalContext)
    {
        if (dragPreviewInstance != null)
            Destroy(dragPreviewInstance);
        dragPreviewInstance = null;
        dragPreviewRect = null;

        if (assignedModCanvasGroup != null)
            assignedModCanvasGroup.alpha = 1f;

        if (clearGlobalContext)
        {
            DragContext.DraggedMod = null;
            isDraggingAssignedMod = false;

            if (owner != null)
                owner.EndAssignedModDragHighlight();
        }
    }
}