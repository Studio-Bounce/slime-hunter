using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Menu : MonoBehaviour
{
    protected UIDocument uiDocument;
    public bool showOnStart = true;

    public bool IsVisible { get { return uiDocument.rootVisualElement.style.display == DisplayStyle.Flex; } }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.enabled = true;
        SetVisible(showOnStart);
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
        SetVisible(true);
    }

    public virtual void Hide()
    {
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
            timer += Time.deltaTime;
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
            timer -= Time.deltaTime;
            yield return null;
        }
        root.style.opacity = 0f;
        SetVisible(false);
    }
}
