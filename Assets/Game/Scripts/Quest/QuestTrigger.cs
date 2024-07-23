using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] QuestSO quest;
    [SerializeField] Transform[] objectiveTargets;
    [SerializeField] float endProximity = 1.0f;

    public UnityEvent onCompleteEvent;
    public bool triggered = false;

    private void Start()
    {
        quest.currentObjective = 0;
        // Map targets to quest
        for (int i = 0; i < objectiveTargets.Length; i++)
        {
            quest.objectives[i].target = objectiveTargets[i];
        }
    }

    private void Update()
    {
        if (triggered)
        {
            // REMOVE
            return;
            // Clear the quest objective when user reaches in proximity
            float distance = Vector3.Distance(GameManager.Instance.PlayerRef.transform.position,
                                              objectiveTargets[quest.currentObjective].position);
            if (distance < endProximity)
            {
                QuestManager.Instance.ClearQuestObjective(quest);
                if (quest.currentObjective >= quest.objectives.Count)
                {
                    // Quest complete
                    Destroy(gameObject);
                }
            }
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
