using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField] float questBannerTime = 2.0f;

    Queue<QuestNotifType> questNotificationQ = new Queue<QuestNotifType>();
    readonly object notifQLock = new object();  // thread-safe
    bool processingNotificationQ = false;

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

    private void Update()
    {
        if (!processingNotificationQ && questNotificationQ.Count > 0)
        {
            processingNotificationQ = true;
            StartCoroutine(ProcessNotificationQ());
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

        lock (notifQLock)
        {
            questNotificationQ.Enqueue(QuestNotifType.ADD);
        }
    }

    void OnQuestUpdate(string _questObjective)
    {
        Label questObjective = questUpdatedVE.Q<Label>("NewObjectiveName");
        questObjective.text = _questObjective;

        if (_questObjective != "")
        {
            lock (notifQLock)
            {
                questNotificationQ.Enqueue(QuestNotifType.UPDATE);
            }
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

        lock (notifQLock)
        {
            questNotificationQ.Enqueue(QuestNotifType.COMPLETE);
        }
    }

    IEnumerator ProcessNotificationQ()
    {
        while (true)
        {
            // Keep processing till the queue is not empty
            int qSize = 0;
            lock (notifQLock)
            {
                qSize = questNotificationQ.Count;
            }
            if (qSize == 0)
                break;

            QuestNotifType questNotifType = QuestNotifType.ADD;
            lock (notifQLock)
            {
                questNotifType = questNotificationQ.Dequeue();
            }
            if (DialogueManager.Instance.IsAStoryRunning)
            {
                yield return null;
            }
            else
            {
                yield return InitiateQuestNotification(questNotifType);
            }
        }
        
        processingNotificationQ = false;
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
            // If a story is running, exit the coroutine
            if (DialogueManager.Instance.IsAStoryRunning)
                break;

            opacity += Time.unscaledDeltaTime;
            canvas.style.opacity = opacity;
            yield return null;
        }
        canvas.style.opacity = 1f;

        int qSize = 0;
        lock (notifQLock)
        {
            qSize = questNotificationQ.Count;
        }
        // We can wait if there's no quest notification pending in queue & no story is running
        if (qSize == 0 && !DialogueManager.Instance.IsAStoryRunning)
        {
            yield return new WaitForSeconds(questBannerTime);
        }

        // If a story is running, hide the quest HUD
        if (autoFade || DialogueManager.Instance.IsAStoryRunning)
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
            opacity -= Time.unscaledDeltaTime;
            canvas.style.opacity = opacity;
            yield return null;
        }
        canvas.style.opacity = 0f;

        HUDMenu hudMenu = UIManager.Instance.HUDMenu as HUDMenu;
        // Hide the relevant visual element
        switch (notifType)
        {
            case QuestNotifType.ADD:
                questStartVE.AddToClassList(panelHideStyle);
                hudMenu.HighlightCompass(true);
                break;
            case QuestNotifType.UPDATE:
                questUpdatedVE.AddToClassList(panelHideStyle);
                hudMenu.HighlightCompass(false);
                break;
            case QuestNotifType.COMPLETE:
                questCompletedVE.AddToClassList(panelHideStyle);
                hudMenu.HighlightCompass(false);
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
