using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AutoSave : Menu
{
    [SerializeField] float rotationSpeed = 100.0f;
    VisualElement spinner;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        spinner = root.Q<VisualElement>("loadSpinner");
    }

    private void Update()
    {
        if (spinner != null)
        {
            Vector3 rotation = spinner.transform.rotation.eulerAngles + new Vector3(0, 0, rotationSpeed * Time.unscaledDeltaTime);
            spinner.transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
