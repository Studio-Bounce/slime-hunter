using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

public class QuestHUD : Menu
{
    enum QuestNotifType
    {
        ADD,
        UPDATE,
        COMPLETE
    };

    // Cached as its getting called in OnDestroy
    QuestManager questManager;

    readonly string panelHideStyle = "unshowned-content";

    VisualElement canvas;
    VisualElement questStartVE;
    VisualElement questUpdatedVE;
    VisualElement questCompletedVE;
    Button questCompleteClearBtn;

    [SerializeField] float questBannerTime = 2.0f;

    void Start()
    {
        questManager = QuestManager.Instance;
        questManager.OnAddQuest += OnNewQuestAdded;
        questManager.OnQuestObjectiveComplete += OnQuestUpdate;
        questManager.OnQuestComplete += OnQuestComplete;

        VisualElement root = uiDocument.rootVisualElement;
        canvas = root.Q<VisualElement>("Canvas");
        questStartVE = canvas.Q<VisualElement>("QuestStart");
        questUpdatedVE = canvas.Q<VisualElement>("QuestUpdated");
        questCompletedVE = canvas.Q<VisualElement>("QuestCompleted");
        questCompleteClearBtn = questCompletedVE.Q<Button>("ClearButton");
        questCompleteClearBtn.clicked += () => StartCoroutine(HideQuestNotification(QuestNotifType.COMPLETE));

        // Hide all
        canvas.style.opacity = 1f;
        if (!questStartVE.ClassListContains(panelHideStyle))
        {
            questStartVE.AddToClassList(panelHideStyle);
        }
        if (!questUpdatedVE.ClassListContains(panelHideStyle))
        {
            questUpdatedVE.AddToClassList(panelHideStyle);
        }
        if (!questCompletedVE.ClassListContains(panelHideStyle))
        {
            questCompletedVE.AddToClassList(panelHideStyle);
        }
    }

    void OnNewQuestAdded(QuestSO quest)
    {
        Label questName = questStartVE.Q<Label>("QuestName");
        Label questType = questStartVE.Q<Label>("QuestType");
        switch (quest.questType)
        {
            case QuestType.MAIN:
                questType.text = "Main Journey";
                break;
            case QuestType.SIDE:
                questType.text = "Side Quest";
                break;
            case QuestType.HUNTING:
                questType.text = "Hunting Quest";
                break;
        }
        questName.text = quest.questName;
        StartCoroutine(InitiateQuestNotification(QuestNotifType.ADD));
    }

    void OnQuestUpdate(string _questObjective)
    {
        if (_questObjective != "")
        {
            Label questObjective = questUpdatedVE.Q<Label>("NewObjectiveName");
            questObjective.text = _questObjective;
            StartCoroutine(InitiateQuestNotification(QuestNotifType.UPDATE));
        }
    }

    void OnQuestComplete(QuestSO quest)
    {
        Label questName = questCompletedVE.Q<Label>("QuestName");
        questName.text = quest.questName;

        VisualElement weapon = questCompletedVE.Q<VisualElement>("Weapon");
        VisualElement spell = questCompletedVE.Q<VisualElement>("Spell");
        VisualElement cash = questCompletedVE.Q<VisualElement>("Cash");
        bool hasWeapon = false, hasSpell = false, hasCash = false;
        foreach (QuestReward questReward in quest.rewards)
        {
            switch (questReward.rewardType)
            {
                case RewardType.WEAPON:
                    hasWeapon = true;
                    weapon.Q<Label>().text = questReward.rewardName;
                    break;

                case RewardType.SPELL:
                    hasSpell = true;
                    spell.Q<Label>().text = questReward.rewardName;
                    break;

                case RewardType.CASH:
                    hasWeapon = true;
                    cash.Q<Label>().text = questReward.quantity.ToString();
                    break;

            }
        }
        weapon.style.display = (hasWeapon) ? DisplayStyle.Flex : DisplayStyle.None;
        spell.style.display = (hasSpell) ? DisplayStyle.Flex : DisplayStyle.None;
        cash.style.display = (hasCash) ? DisplayStyle.Flex : DisplayStyle.None;

        StartCoroutine(InitiateQuestNotification(QuestNotifType.COMPLETE, false));
    }

    IEnumerator InitiateQuestNotification(QuestNotifType notifType, bool autoFade = true)
    {
        // Show the relevant visual element
        switch (notifType)
        {
            case QuestNotifType.ADD:
                questStartVE.RemoveFromClassList(panelHideStyle);
                break;
            case QuestNotifType.UPDATE:
                questUpdatedVE.RemoveFromClassList(panelHideStyle);
                break;
            case QuestNotifType.COMPLETE:
                questCompletedVE.RemoveFromClassList(panelHideStyle);
                break;
        }

        // Fade in
        float opacity = 0f;
        canvas.style.opacity = opacity;
        while (opacity < 1.0f)
        {
            opacity += Time.deltaTime;
            canvas.style.opacity = opacity;
            yield return null;
        }
        canvas.style.opacity = 1f;

        yield return new WaitForSeconds(questBannerTime);

        if (autoFade)
        {
            yield return HideQuestNotification(notifType);
        }
    }

    IEnumerator HideQuestNotification(QuestNotifType notifType)
    {
        // Fade out
        float opacity = 1f;
        canvas.style.opacity = opacity;
        while (opacity > 0f)
        {
            opacity -= Time.deltaTime;
            canvas.style.opacity = opacity;
            yield return null;
        }
        canvas.style.opacity = 0f;

        // Hide the relevant visual element
        switch (notifType)
        {
            case QuestNotifType.ADD:
                questStartVE.AddToClassList(panelHideStyle);
                break;
            case QuestNotifType.UPDATE:
                questUpdatedVE.AddToClassList(panelHideStyle);
                break;
            case QuestNotifType.COMPLETE:
                questCompletedVE.AddToClassList(panelHideStyle);
                break;
        }
    }

    void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.OnAddQuest -= OnNewQuestAdded;
            questManager.OnQuestObjectiveComplete -= OnQuestUpdate;
            questManager.OnQuestComplete -= OnQuestComplete;
        }
    }
}
