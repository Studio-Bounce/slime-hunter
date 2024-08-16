using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputGlyph : VisualElement
{
    public string ActionName { get; set; }

    private bool _initialized = false;

    public new class UxmlFactory : UxmlFactory<InputGlyph, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription actionNameAttribute = new UxmlStringAttributeDescription {
            name = "action-name",
            defaultValue = ""
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var inputGlyph = ve as InputGlyph;
            inputGlyph.ActionName = actionNameAttribute.GetValueFromBag(bag, cc);
        }
    }

    public InputGlyph()
    {
        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
    }

    private void OnAttachToPanel(AttachToPanelEvent evt)
    {
        if (!_initialized)
        {
            _initialized = true;
            SetAction(ActionName);
        }
    }

    private void SetAction(string actionName)
    {
        Sprite sprite = InputManager.Instance.FindSpriteByAction(actionName);
        style.backgroundImage = sprite?.texture;
    }
}
