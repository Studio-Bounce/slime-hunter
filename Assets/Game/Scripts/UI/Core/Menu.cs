using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Menu : MonoBehaviour
{
    protected UIDocument uiDocument;
    protected VisualElement root;

    public bool showOnStart = true;
    protected FocusController focusController;

    public bool IsVisible { get { return uiDocument.rootVisualElement.style.display == DisplayStyle.Flex; } }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.enabled = true;
        root = uiDocument.rootVisualElement;
        focusController = root.focusController;
        SetVisible(showOnStart);
    }

    public void OnSelect()
    {
        if (!IsVisible) return;

        VisualElement focusedElement = focusController.focusedElement as VisualElement;
        // Debug.Log($"Select: {focusedElement?.name}");
        if (focusedElement != null && focusedElement is Button button)
        {
            using (var clickEvent = ClickEvent.GetPooled())
            {
                clickEvent.target = button;
                button.SendEvent(clickEvent);
            }
        }
    }

    public void SetVisible(bool visible)
    {
        uiDocument.rootVisualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public virtual void ToggleVisible()
    {
        if (IsVisible) Hide(); else Show();
    }

    public virtual void Show()
    {
        root.Q<VisualElement>().Focus();
        SetVisible(true);
    }

    public virtual void Hide()
    {
        root.Blur();
        SetVisible(false);
    }

    public virtual IEnumerator FadeIn(float duration)
    {
        SetVisible(true);
        float timer = 0;
        VisualElement root = uiDocument.rootVisualElement;
        while (timer < duration)
        {
            root.style.opacity = timer;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        root.style.opacity = 1.0f;
    }

    public virtual IEnumerator FadeOut(float duration)
    {
        float timer = duration;
        VisualElement root = uiDocument.rootVisualElement;
        while (timer > 0)
        {
            root.style.opacity = timer;
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }
        root.style.opacity = 0f;
        SetVisible(false);
    }
}
