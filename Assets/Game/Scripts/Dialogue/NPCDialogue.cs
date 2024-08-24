using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Character
{
    MYLO,
    BLACKSMITH,
    ALCHEMIST,
    GUARD,
    STRANGER,
    KURO
}


[System.Serializable]
public class Dialogue
{
    public TextAsset inkStoryJSON;
    [Tooltip("People who are part of the conversation")]
    public List<Character> participants;
}


public class NPCDialogue : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject questGameObject;

    [Tooltip("Start the dialogues using box trigger")]
    public bool useBoxTrigger = true;
    public bool isStoryComplete = false;
    bool didStoryStart = false;

    public UnityEvent OnDialoguesStart;
    public UnityEvent OnDialoguesFinish;

    private void Start()
    {
        isStoryComplete = false;
        if (questGameObject != null)
            questGameObject.SetActive(false);
    }

    private void Update()
    {
        if (isStoryComplete)
        {
            OnDialoguesFinish.Invoke();
            // Start the quest
            if (questGameObject != null)
                questGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useBoxTrigger && other.gameObject.layer == GameConstants.PlayerLayer)
        {
            StartDialogues();
        }
    }

    public void StartDialogues()
    {
        if (!isStoryComplete && !didStoryStart)
        {
            didStoryStart = true;
            OnDialoguesStart.Invoke();
            DialogueManager.Instance.StartDialogues(this, dialogue);
        }
    }

    private void OnValidate()
    {
        if (useBoxTrigger)
        {
            if (!gameObject.TryGetComponent<BoxCollider>(out var _))
            {
                useBoxTrigger = false;
                Debug.LogError("Need box collider to set Use Box Trigger!");
            }
        }
    }
}
