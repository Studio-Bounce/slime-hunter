using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestMenu : MonoBehaviour
{
    UIDocument uiDocument;

    // Buttons
    Button activeBtn;
    Button mainQuestBtn;
    Button sideQuestBtn;
    Button huntingQuestBtn;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        VisualElement questTypes = root.Q<VisualElement>("SubOptions");

        mainQuestBtn = questTypes.Q<Button>("MainQuestBtn");
        sideQuestBtn = questTypes.Q<Button>("SideQuestBtn");
        huntingQuestBtn = questTypes.Q<Button>("HuntingQuestBtn");
    }

    private void Start()
    {
        activeBtn = mainQuestBtn;  // Main quest is active by default
        mainQuestBtn.clicked += () => QuestTypeSelected(QuestType.MAIN);
        sideQuestBtn.clicked += () => QuestTypeSelected(QuestType.SIDE);
        huntingQuestBtn.clicked += () => QuestTypeSelected(QuestType.HUNTING);
    }

    void QuestTypeSelected(QuestType questType)
    {
        activeBtn.style.unityFontStyleAndWeight = FontStyle.Normal;
        switch (questType)
        {
            case QuestType.MAIN:
                activeBtn = mainQuestBtn;
                break;

            case QuestType.SIDE:
                activeBtn = sideQuestBtn;
                break;

            case QuestType.HUNTING:
                activeBtn = huntingQuestBtn;
                break;

        }

        activeBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
    }
}
