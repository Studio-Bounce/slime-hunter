using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] QuestSO quest;
    [SerializeField] Transform[] objectiveTargets;
    
    bool triggered = false;

    private void Start()
    {
        // Map targets to quest
        for (int i = 0; i < objectiveTargets.Length; i++)
        {
            quest.objectives[i].target = objectiveTargets[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            QuestManager.Instance.AddQuest(quest);

            // TEMPORARY for testing
            QuestManager.Instance.SetQuestAsActive(quest);
        }
    }

    private void OnValidate()
    {
        if (quest != null && objectiveTargets.Length != quest.objectives.Count)
        {
            objectiveTargets = new Transform[quest.objectives.Count];
        }
    }
}
