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
    public float endProximity;
}

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] QuestSO quest;
    [SerializeField] QuestObjectiveData[] objectives;
    [SerializeField] GameObject trackerCanvasGO;

    public UnityEvent onQuestStart;
    public List<UnityEvent> onCompleteEvent = new List<UnityEvent>();

    // Delay in starting quest if its supposed to start right after another quest
    public bool artificalDelay = false;
    public float delayTime = 2.0f;

    bool triggered = false;
    bool triggerObjectiveComplete = false;
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
            float distance = 0;
            if (quest.objectives[quest.currentObjective].target)
            {
                // Clear the quest objective when user reaches in proximity
                distance = Vector3.Distance(GameManager.Instance.PlayerRef.transform.position,
                                            quest.objectives[quest.currentObjective].target.position);
            }
            if (distance < objectives[quest.currentObjective].endProximity || triggerObjectiveComplete)
            {
                triggerObjectiveComplete = false;
                // Quest Objective complete!
                onCompleteEvent[quest.currentObjective].Invoke();
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

    public void ManualQuestObjectiveCompleteOverride()
    {
        triggerObjectiveComplete = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            StartQuest();
        }
    }

    public void StartQuest()
    {
        if (triggered) return;

        triggered = true;
        StartCoroutine(StartQuestSequence());
    }

    IEnumerator StartQuestSequence()
    {
        if (artificalDelay)
        {
            yield return new WaitForSeconds(delayTime);
        }
        yield return null;

        onQuestStart.Invoke();
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

    void AddTracker()
    {
        UIManager.Instance.StartNavigation(quest.objectives[quest.currentObjective].target.position);
        Vector3 position = new Vector3(0, objectives[quest.currentObjective].navYOffset, 0);
        _prevTracker = Instantiate(trackerCanvasGO, Vector3.zero, Quaternion.identity, quest.objectives[quest.currentObjective].target);
        _prevTracker.transform.localPosition = position;
    }

    private void OnValidate()
    {
        if (objectives == null)
            return;

        if (quest != null && objectives.Length != quest.objectives.Count)
        {
            Debug.LogError("Invalid objectives! They do not match the quest.");
        }
    }

    private void OnDrawGizmos()
    {
        if (objectives != null && objectives.Length > 0)
        {
            foreach (QuestObjectiveData objectiveData in objectives)
            {
                if (objectiveData.target != null)
                {
                    DebugExtension.DrawCircle(objectiveData.target.transform.position, Vector3.up, Color.black, objectiveData.endProximity);
                }
            }
        }
    }
}
