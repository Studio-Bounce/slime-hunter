using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ink.Runtime;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] DialogueHUD dialogueHUD = null;

    bool isStoryRunning = false;

    private void Start()
    {
        if (dialogueHUD == null)
        {
            dialogueHUD = FindAnyObjectByType<DialogueHUD>();
        }
    }

    public bool StartDialogues(Dialogue dialogue)
    {
        if (isStoryRunning)
        {
            // Can not trigger 2 dialogues concurrently
            return false;
        }

        isStoryRunning = true;
        dialogueHUD.Show();
        Story story = new Story(dialogue.inkStoryJSON.text);
        ContinueStory(dialogue, story);

        return true;
    }

    // Main function which gets called every time the story changes
    public void ContinueStory(Dialogue dialogue, Story story)
    {
        if (!isStoryRunning)
            return;

        // Clear the previous dialogues & options
        dialogueHUD.ClearDialogueBox();
        dialogueHUD.ClearDialogueOptions();

        // Read the story content (returns false for choices)
        bool startingStory = true;
        while (story.canContinue)
        {
            string text = story.Continue();
            ParseDialogue(text, out string dialogText, out int idx);
            if (startingStory)
            {
                // Starting new dialogue so update both person name and the dialogue
                startingStory = false;
                string participant = dialogue.participants[idx].ToString();
                participant = Utils.ToCapitalizedString(participant);
                dialogueHUD.SetDialogueBox(participant, dialogText);
            }
            else
            {
                // Appending to running dialog, no need of updating name
                dialogueHUD.AppendDialogue(dialogText);
            }
        }

        // Display the choices, if any
        if (story.currentChoices.Count > 0)
        {
            dialogueHUD.SpawnDialogueOptions(dialogue, story);
        }
        else
        {
            // Story complete
            isStoryRunning = false;
            StartCoroutine(DelayedClear(0.5f));
        }
    }

    IEnumerator DelayedClear(float timer)
    {
        yield return new WaitForSeconds(timer);
        dialogueHUD.Hide();
    }

    void ParseDialogue(string dialogText, out string actualDialog, out int participantIdx)
    {
        // {number}: "{dialogue}"
        string pattern = @"(\d+): ""(.*)""";
        Match match = Regex.Match(dialogText, pattern);
        if (match.Success)
        {
            participantIdx = int.Parse(match.Groups[1].Value);
            actualDialog = match.Groups[2].Value;
        }
        else
        {
            actualDialog = "";
            participantIdx = 0;
            Debug.LogError($"Following dialogue could not be parsed: {dialogText}");
        }
    }
}
