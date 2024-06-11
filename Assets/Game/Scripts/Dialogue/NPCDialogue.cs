using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : InkDialogueController
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameConstants.PlayerLayer)
        {
            StartStory();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == GameConstants.PlayerLayer)
        {
            StartCoroutine(DelayedClear(0.5f));
        }
    }
}
