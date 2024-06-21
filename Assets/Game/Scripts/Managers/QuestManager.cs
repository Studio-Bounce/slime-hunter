using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestManager : Singleton<QuestManager>
{
    List<QuestSO> allQuests = new();
    QuestSO activeQuest = null;

    public event Action<QuestSO> OnAddQuest;
    public event Action<string, string> OnActiveQuestChange;

    [SerializeField] RectTransform questTracker;
    [SerializeField] RectTransform questArrow;
    [SerializeField] TextMeshProUGUI questDistance;
    public float edgeBuffer = 10.0f;

    [SerializeField] QuestMenu questMenu;

    private void LateUpdate()
    {
        TrackActiveQuest();
    }

    void TrackActiveQuest()
    {
        if (activeQuest == null || !activeQuest.isActive || GameManager.Instance.PlayerRef == null)
        {
            questTracker.gameObject.SetActive(false);
            return;
        }
        if (!questTracker.gameObject.activeSelf)
        {
            questTracker.gameObject.SetActive(true);
        }

        Transform target = activeQuest.objectives[activeQuest.currentObjective].target;
        if (target == null)
        {
            return;
        }

        bool isTargetOffscreen = Utils.IsWorldPositionOffScreen(target.position + Vector3.up, out Vector3 screenPosition);
        if (isTargetOffscreen)
        {
            screenPosition.x = Mathf.Clamp(screenPosition.x, edgeBuffer, Screen.width - edgeBuffer);
            screenPosition.y = Mathf.Clamp(screenPosition.y, edgeBuffer, Screen.height - edgeBuffer);
            return; // TEMPORARY
        }

        questTracker.position = screenPosition;
        // Calculate the direction from the player to the target
        Vector3 playerPosition = GameManager.Instance.PlayerRef.transform.position;
        float angle = 0;
        if (isTargetOffscreen)
        {
            Vector3 direction = (target.position - playerPosition).normalized;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        questArrow.rotation = Quaternion.Euler(0, 0, angle - 90);
        questDistance.text = $"{(int)Vector3.Distance(playerPosition, target.position)}m";
    }

    void GiveRewards(QuestReward questReward)
    {
        // TODO: Implement reward mechanism
        Debug.Log("Rewarding player with " + questReward.rewardName);
    }

    // -------------------------- Exposed (public) functions --------------------------

    // Add a new quest to player's game
    public void AddQuest(QuestSO quest)
    {
        OnAddQuest.Invoke(quest);
        allQuests.Add(quest);
    }

    // Called when the player sets a quest as active
    public void SetQuestAsActive(QuestSO quest)
    {
        if (activeQuest != null)
        {
            activeQuest.isActive = false;
        }
        activeQuest = quest;
        activeQuest.isActive = true;
        OnActiveQuestChange.Invoke(activeQuest.questName, activeQuest.objectives[activeQuest.currentObjective].objective);
    }

    // Called when the player clears the current objective of a quest.
    // If all objectives are cleared, the quest gets completed & rewards are awarded
    public void ClearQuestObjective(QuestSO quest)
    {
        if (quest != activeQuest)
        {
            Debug.LogWarning("Trying to clear objective of inactive quest. Making it active and proceeding.");
            SetQuestAsActive(quest);
        }

        quest.currentObjective += 1;
        // Quest complete
        if (quest.currentObjective >= quest.objectives.Count)
        {
            // TODO: Notification that quest is complete
            Debug.Log("Quest is complete");

            quest.isComplete = true;
            quest.isActive = false;
            foreach (QuestReward questReward in quest.rewards)
            {
                GiveRewards(questReward);
            }
            allQuests.Remove(quest);

            if (activeQuest == quest)
            {
                activeQuest = null;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // ------------- Getters -------------

    public List<QuestSO> GetQuestsByType(QuestType questType)
    {
        List<QuestSO> _quests = new List<QuestSO>();
        foreach (QuestSO quest in allQuests)
        {
            if (quest.questType == questType)
                _quests.Add(quest);
        }
        return _quests;
    }

    public QuestSO GetActiveQuest() { return activeQuest; }

}
