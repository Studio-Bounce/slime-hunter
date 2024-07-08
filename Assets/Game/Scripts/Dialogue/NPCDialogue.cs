using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Character
{
    MYLO,
    BLACKSMITH,
    ALCHEMIST,
    GUARD
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

    public bool isStoryComplete = false;

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
            // Start the quest
            if (questGameObject != null)
                questGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStoryComplete && other.gameObject.layer == GameConstants.PlayerLayer)
        {
            DialogueManager.Instance.StartDialogues(this, dialogue);
        }
    }
}
