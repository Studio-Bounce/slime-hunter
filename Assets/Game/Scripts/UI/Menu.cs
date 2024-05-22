using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Menu : MonoBehaviour
{
    protected UIDocument uiDocument;
    public bool showOnStart = true;

    // Start is called before the first frame update
    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        SetVisible(showOnStart);
    }

    public void Show()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void SetVisible(bool visible)
    {
        uiDocument.rootVisualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
