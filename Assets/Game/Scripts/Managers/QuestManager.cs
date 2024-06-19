using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    List<QuestSO> allQuests = new();
    QuestSO activeQuest = null;

    public event Action<string, string> OnActiveQuestChange;

    [Header("For Testing")]
    public QuestSO[] testAllQuests;

    private void Start()
    {
        // FOR TESTING ONLY
        foreach (QuestSO q in testAllQuests)
        {
            allQuests.Add(q);
        }
    }

    private void Update()
    {
        // Track the active quest
        if (activeQuest != null && activeQuest.isActive)
        {
            TrackActiveQuest();
        }

        // FOR TESTING ONLY
        if (activeQuest == null)
        {
            foreach (QuestSO quest in allQuests)
            {
                if (quest.isActive)
                {
                    SetQuestAsActive(quest);
                    return;
                }
            }
        }
    }

    void TrackActiveQuest()
    {
        if (activeQuest == null)
        {
            return;
        }

        // TODO: Tracking logic
    }

    void GiveRewards(QuestReward questReward)
    {
        // TODO: Implement reward mechanism
        Debug.Log("Rewarding player with " + questReward.rewardName);
    }

    // -------------------------- Quest HUD Handler -----------------------------------



    // -------------------------- Exposed (public) functions --------------------------

    // Add a new quest to player's game
    public void AddQuest(QuestSO quest)
    {
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
