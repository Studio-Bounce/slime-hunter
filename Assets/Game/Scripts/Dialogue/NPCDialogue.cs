using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Character
{
    MYLO,
    BLACKSMITH,
    ALCHEMIST
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
    public bool isStoryComplete = false;

    private void Start()
    {
        isStoryComplete = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStoryComplete && other.gameObject.layer == GameConstants.PlayerLayer)
        {
            DialogueManager.Instance.StartDialogues(this, dialogue);
        }
    }
}
