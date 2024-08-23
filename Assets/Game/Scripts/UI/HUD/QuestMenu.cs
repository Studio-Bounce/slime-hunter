using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestMenu : Menu
{
    // Buttons
    const int numTabs = 3;
    int currentTab = 0;  // For left/right shift functionality
    Button mainQuestBtn;
    Button sideQuestBtn;
    Button huntingQuestBtn;
    Button selectedQuestTypeBtn;
    Button selectedQuestBtn = null;

    VisualElement questHeader;
    VisualElement questListContent;
    VisualElement questContent;
    VisualElement questComplete;

    Button mainPagination;
    Button sidePagination;
    Button huntingPagination;

    [SerializeField] VisualTreeAsset questListAsset;
    [SerializeField] Texture2D activeQuestIcon;

    [SerializeField] Color selectedQuestBorderColor;
    [SerializeField] int selectedQuestBorderWidth = 5;
    [SerializeField] int questBorderWidth = 2;
    [SerializeField] Font questObjectiveFont;

    private void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;

        // Top-level button on the pause menu ----------------
        questHeader = root.Q<VisualElement>("QuestTab");
        questHeader.RegisterCallback<ClickEvent>(evt => {
            // Open main quests by default
            QuestTypeSelected(QuestType.MAIN);
        });
        currentTab = 0;

        // Left side options on Quest page -------------------
        VisualElement questTypes = root.Q<VisualElement>("SubOptions");
        mainQuestBtn = questTypes.Q<Button>("MainQuestBtn");
        sideQuestBtn = questTypes.Q<Button>("SideQuestBtn");
        huntingQuestBtn = questTypes.Q<Button>("HuntingQuestBtn");

        selectedQuestTypeBtn = mainQuestBtn;  // Main quest is active by default
        mainQuestBtn.RegisterCallback<ClickEvent>(evt => QuestTypeSelected(QuestType.MAIN));
        sideQuestBtn.RegisterCallback<ClickEvent>(evt => QuestTypeSelected(QuestType.SIDE));
        huntingQuestBtn.RegisterCallback<ClickEvent>(evt => QuestTypeSelected(QuestType.HUNTING));

        VisualElement questList = root.Q<VisualElement>("QuestList");
        VisualElement questListLeft = questList.Q<VisualElement>("LeftArrow");
        VisualElement questListRight = questList.Q<VisualElement>("RightArrow");
        questListLeft.Q<Button>().RegisterCallback<ClickEvent>(evt => {
            currentTab = (numTabs + currentTab - 1) % numTabs;
            QuestTypeSelectById();
        });
        questListRight.Q<Button>().RegisterCallback<ClickEvent>(evt => {
            currentTab = (currentTab + 1) % numTabs;
            QuestTypeSelectById();
        });

        questListContent = questList.Q<VisualElement>("Content");

        // Right side options on Quest page ------------------
        VisualElement questExplanation = root.Q<VisualElement>("QuestExplanation");
        questContent = questExplanation.Q<VisualElement>("Content");
        questComplete = questExplanation.Q<VisualElement>("Complete");
        questComplete.style.display = DisplayStyle.None;

        // Bottom pagination ---------------------------------
        VisualElement questTypesPage = root.Q<VisualElement>("QuestPage");
        mainPagination = questTypesPage.Q<Button>("Main");
        sidePagination = questTypesPage.Q<Button>("Side");
        huntingPagination = questTypesPage.Q<Button>("Hunting");
    }

    void QuestTypeSelectById()
    {
        if (currentTab == 0)
        {
            QuestTypeSelected(QuestType.MAIN);
        }
        else if (currentTab == 1)
        {
            QuestTypeSelected(QuestType.SIDE);
        }
        else if (currentTab == 2)
        {
            QuestTypeSelected(QuestType.HUNTING);
        }
    }

    void QuestTypeSelected(QuestType questType)
    {
        // Un-highlight previous button
        selectedQuestTypeBtn.style.unityFontStyleAndWeight = FontStyle.Normal;
        selectedQuestTypeBtn.style.opacity = 0.5f;

        mainPagination.style.opacity = 0.25f;
        sidePagination.style.opacity = 0.25f;
        huntingPagination.style.opacity = 0.25f;
        switch (questType)
        {
            case QuestType.MAIN:
                selectedQuestTypeBtn = mainQuestBtn;
                currentTab = 0;
                mainPagination.style.opacity = 1.0f;
                break;

            case QuestType.SIDE:
                selectedQuestTypeBtn = sideQuestBtn;
                currentTab = 1;
                sidePagination.style.opacity = 1.0f;
                break;

            case QuestType.HUNTING:
                selectedQuestTypeBtn = huntingQuestBtn;
                currentTab = 2;
                huntingPagination.style.opacity = 1.0f;
                break;
        }

        // Highlight the button
        selectedQuestTypeBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
        selectedQuestTypeBtn.style.opacity = 1.0f;

        // Update quest list
        UIManager.ClearVisualElement(questListContent);
        selectedQuestBtn = null;
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

    void PopulateQuestDetails(VisualElement questItemVE, QuestSO quest, bool isFirst)
    {
        Button questItemParent = questItemVE.Q<Button>("QuestListItem");
        questItemParent.style.display = DisplayStyle.Flex;
        questItemParent.RegisterCallback<ClickEvent>(evt => QuestSelected(questItemParent, quest));
        // Auto-select the first quest
        if (isFirst)
        {
            QuestSelected(questItemParent, quest);
        }

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

    void QuestSelected(Button questBtn, QuestSO quest)
    {
        if (selectedQuestBtn != null)
        {
            // Remove highlight of previously selected button
            SetButtonBorderColor(selectedQuestBtn, Color.white);
            SetButtonBorderWidth(selectedQuestBtn, questBorderWidth);
        }
        // Highlight newly selected button
        SetButtonBorderColor(questBtn, selectedQuestBorderColor);
        SetButtonBorderWidth(questBtn, selectedQuestBorderWidth);

        // Display quest content
        VisualElement questNameVE = questContent.Q<VisualElement>("QuestName");
        questNameVE.Q<Label>().text = quest.questName;
        VisualElement questPositionVE = questContent.Q<VisualElement>("Position");
        questPositionVE.Q<Label>().text = quest.levelName;

        VisualElement questDescription = questContent.Q<VisualElement>("TrackInformation");
        questDescription.Q<Label>().text = quest.description;
        VisualElement questObjectives = questDescription.Q<VisualElement>("ToDoList");
        UIManager.ClearVisualElement(questObjectives);
        for (int i = 0; i < quest.objectives.Count; i++)
        {
            Label objectiveLabel = new()
            {
                name = $"Item{i + 1}",
                text = $"- {quest.objectives[i].objective}"
            };
            objectiveLabel.style.color = Color.white;
            objectiveLabel.style.fontSize = 20;
            if (questObjectiveFont != null)
            {
                objectiveLabel.style.unityFont = questObjectiveFont;
            }
            if (i != quest.currentObjective)
            {
                // Don't highlight
                objectiveLabel.style.opacity = 0.5f;
                objectiveLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
            }
            else
            {
                // Highlight
                objectiveLabel.style.opacity = 1.0f;
                objectiveLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            }
            questObjectives.Add(objectiveLabel);
        }

        VisualElement questRewards = questContent.Q<VisualElement>("Rewards");
        VisualElement questWeapon = questRewards.Q<VisualElement>("Weapon");
        VisualElement questSpell = questRewards.Q<VisualElement>("Spell");
        VisualElement questCash = questRewards.Q<VisualElement>("Cash");
        questWeapon.style.display = DisplayStyle.None;
        questSpell.style.display = DisplayStyle.None;
        questCash.style.display = DisplayStyle.None;
        foreach (QuestReward questReward in quest.rewards)
        {
            if (questReward.rewardType == RewardType.WEAPON)
            {
                questWeapon.style.display = DisplayStyle.Flex;
                questWeapon.Q<Label>().text = questReward.rewardName;
            }
            else if (questReward.rewardType == RewardType.SPELL)
            {
                questSpell.style.display = DisplayStyle.Flex;
                questSpell.Q<Label>().text = questReward.rewardName;
            }
            else if (questReward.rewardType == RewardType.CASH)
            {
                questCash.style.display = DisplayStyle.Flex;
                questCash.Q<Label>().text = questReward.quantity.ToString();
            }
        }

        selectedQuestBtn = questBtn;
    }

    void SetButtonBorderColor(Button _button, Color _color)
    {
        _button.style.borderBottomColor = _color;
        _button.style.borderTopColor = _color;
        _button.style.borderLeftColor = _color;
        _button.style.borderRightColor = _color;
    }

    void SetButtonBorderWidth(Button _button, int _width)
    {
        _button.style.borderBottomWidth = _width;
        _button.style.borderTopWidth = _width;
        _button.style.borderLeftWidth = _width;
        _button.style.borderRightWidth = _width;
    }
}
