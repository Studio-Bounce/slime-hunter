using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct QuestObjectiveData
{
    public Transform target;
    public bool showOverheadNavigation;
    public float navYOffset;
}

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] QuestSO quest;
    [SerializeField] QuestObjectiveData[] objectives;
    [SerializeField] GameObject trackerCanvasGO;
    [SerializeField] float endProximity = 1.0f;

    public UnityEvent onCompleteEvent;
    public bool triggered = false;

    GameObject _prevTracker;

    private void Start()
    {
        quest.currentObjective = 0;
        // Map targets to quest
        for (int i = 0; i < objectives.Length; i++)
        {
            quest.objectives[i].target = objectives[i].target;
        }
    }

    private void Update()
    {
        if (triggered)
        {
            // Clear the quest objective when user reaches in proximity
            float distance = Vector3.Distance(GameManager.Instance.PlayerRef.transform.position,
                                              quest.objectives[quest.currentObjective].target.position);
            if (distance < endProximity)
            {
                QuestManager.Instance.ClearQuestObjective(quest);
                // Clear previous tracker
                if (_prevTracker != null)
                {
                    Destroy(_prevTracker);
                }

                if (quest.currentObjective >= quest.objectives.Count)
                {
                    // Quest complete
                    UIManager.Instance.StopNavigation();
                    Destroy(gameObject);
                }
                else if (objectives[quest.currentObjective].showOverheadNavigation)
                {
                    // Add tracker if required
                    AddTracker();
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

            // Add tracker if required
            if (objectives[quest.currentObjective].showOverheadNavigation)
            {
                // Add tracker if required
                AddTracker();
            }
        }
    }

    void AddTracker()
    {
        UIManager.Instance.StartNavigation(quest.objectives[quest.currentObjective].target.position);
        Vector3 position = new Vector3(0, objectives[quest.currentObjective].navYOffset, 0);
        _prevTracker = Instantiate(trackerCanvasGO, Vector3.zero, Quaternion.identity, quest.objectives[quest.currentObjective].target);
        _prevTracker.transform.localPosition = position;
    }

    private void OnValidate()
    {
        if (quest != null && objectives.Length != quest.objectives.Count)
        {
            Debug.LogError("Invalid objectives! They do not match the quest.");
        }
    }

    private void OnDrawGizmos()
    {
        if (objectives.Length > 0)
        {
            foreach (QuestObjectiveData objectiveData in objectives)
            {
                DebugExtension.DrawCircle(objectiveData.target.transform.position, Vector3.up, Color.black, endProximity);
            }
        }
    }
}
