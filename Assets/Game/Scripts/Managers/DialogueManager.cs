using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using Ink.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] DialogueHUD dialogueHUD = null;
    [SerializeField] HUDMenu hudMenu = null;

    NPCDialogue storyOwner = null;
    bool isStoryRunning = false;
    public bool IsAStoryRunning { get { return isStoryRunning; } }

    private void Start()
    {
        if (dialogueHUD == null)
        {
            dialogueHUD = FindAnyObjectByType<DialogueHUD>();
        }
    }

    public bool StartDialogues(NPCDialogue owner, Dialogue dialogue)
    {
        if (isStoryRunning)
        {
            // Can not trigger 2 dialogues concurrently
            return false;
        }

        // Disable controls
        InputManager.Instance.TogglePlayerControls(false);
        isStoryRunning = true;
        dialogueHUD.Show();
        hudMenu.Hide();
        storyOwner = owner;
        Story story = new Story(dialogue.inkStoryJSON.text);
        StartCoroutine(ContinueStory(dialogue, story));

        return true;
    }

    public void ContinueStoryPublic(Dialogue dialogue, Story story)
    {
        StartCoroutine(ContinueStory(dialogue, story));
    }

    // Main function which gets called every time the story changes
    IEnumerator ContinueStory(Dialogue dialogue, Story story)
    {
        if (isStoryRunning)
        {
            // Clear the previous dialogues & options
            dialogueHUD.ClearDialogueBox();
            dialogueHUD.ClearDialogueOptions();

            // Read the story content (returns false for choices)
            bool startingStory = true;
            while (story.canContinue)
            {
                string text = story.Continue();
                ParseDialogue(text, out string dialogText, out int idx);
                
                // Do not interrupt running dialog
                while (dialogueHUD.dialogRunning)
                {
                    yield return null;
                }

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
                    // Appending to running dialogue, no need of updating name
                    dialogueHUD.AppendDialogue(dialogText);
                }
            }

            // Do not interrupt running dialog
            while (dialogueHUD.dialogRunning)
            {
                yield return null;
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
                storyOwner.isStoryComplete = true;
                StartCoroutine(DelayedClear(2.0f));
            }
        }
    }

    public void SkipDialogue(InputAction.CallbackContext context)
    {
        dialogueHUD?.SkipDialogue();
    }

    IEnumerator DelayedClear(float timer)
    {
        yield return new WaitForSecondsRealtime(timer);
        dialogueHUD.Hide();
        hudMenu.Show();
        // Enable controls
        InputManager.Instance.TogglePlayerControls(true);
    }

    bool ParseDialogue(string dialogText, out string actualDialog, out int participantIdx)
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

            return false;
        }
        return true;
    }
}
