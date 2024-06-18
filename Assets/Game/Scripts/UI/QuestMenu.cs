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

    VisualElement questListContent;
    [SerializeField] VisualTreeAsset questListAsset;
    [SerializeField] Texture2D activeQuestIcon;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        VisualElement questTypes = root.Q<VisualElement>("SubOptions");
        mainQuestBtn = questTypes.Q<Button>("MainQuestBtn");
        sideQuestBtn = questTypes.Q<Button>("SideQuestBtn");
        huntingQuestBtn = questTypes.Q<Button>("HuntingQuestBtn");

        VisualElement questList = root.Q<VisualElement>("QuestList");
        questListContent = questList.Q<VisualElement>("Content");
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
        Debug.Log("Clicked " + questType.ToString());
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

        // Highlight the button
        activeBtn.style.unityFontStyleAndWeight = FontStyle.Bold;

        // Update quest list
        ClearQuestContentList();
        List<QuestSO> quests = QuestManager.Instance.GetQuestsByType(questType);
        bool isFirst = true;
        foreach (QuestSO quest in quests)
        {
            VisualElement questElement = questListAsset.CloneTree();
            PopulateQuestDetails(questElement, quest, isFirst);
            isFirst = false;
            questListContent.Add(questElement);
        }
    }

    void ClearQuestContentList()
    {
        // Safe-deletion
        List<VisualElement> questItems = new();
        foreach (VisualElement questItem in questListContent.Children())
        {
            questItems.Add(questItem);
        }
        foreach (VisualElement questItem in questItems)
        {
            questListContent.Remove(questItem);
        }
    }

    void PopulateQuestDetails(VisualElement questItemVE, QuestSO quest, bool isSelected)
    {
        string veName = (isSelected) ? "QuestListItemSelected" : "QuestListItem";
        VisualElement questItemParent = questItemVE.Q<VisualElement>(veName);
        questItemParent.style.display = DisplayStyle.Flex;
        // Quest status: Active or Inactive
        VisualElement questStatus = questItemParent.Q<VisualElement>("QuestStatus");
        VisualElement activeStatus    = questStatus.Q<VisualElement>("Active");
        VisualElement notActiveStatus = questStatus.Q<VisualElement>("NotActive");
        activeStatus.style.display    = (quest.isActive) ? DisplayStyle.Flex : DisplayStyle.None;
        notActiveStatus.style.display = (quest.isActive) ? DisplayStyle.None : DisplayStyle.Flex;

        // Task name
        VisualElement taskName = questItemParent.Q<VisualElement>("TaskNameSlot");
        Label taskNameLbl = taskName.Q<Label>("TaskNameLabel");
        taskNameLbl.text = quest.questName;

        // Task level
        VisualElement taskLevel = questItemParent.Q<VisualElement>("TaskLevelSlot");
        Label taskLevelLbl = taskLevel.Q<Label>("TaskLevelLabel");
        taskLevelLbl.text = quest.levelName;
    }
}
