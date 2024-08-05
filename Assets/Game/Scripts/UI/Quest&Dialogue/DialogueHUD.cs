using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UIElements;

public class DialogueHUD : Menu
{
    VisualElement dialogBox;
    VisualElement dialogOptions;
    Label dialogPersonName;
    Label dialogContent;
    string currentDialogString; // Cache whole dialogue

    [SerializeField] VisualTreeAsset dialogOptionBtn;
    [SerializeField] Color dialogColor;
    [SerializeField] Color dialogHoverColor;
    [HideInInspector] public bool dialogRunning = false;

    private void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;

        dialogBox = root.Q<VisualElement>("DialogueBox");
        dialogOptions = root.Q<VisualElement>("DialogueOptions");
        dialogPersonName = dialogBox.Q<VisualElement>("Name").Q<Label>();
        dialogContent = dialogBox.Q<Label>("DialogueText");
        Hide();
    }

    // ------------------------ Dialogue Options ------------------------
    public void ClearDialogueOptions()
    {
        UIManager.ClearVisualElement(dialogOptions);
    }

    public void SpawnDialogueOptions(Dialogue dialogue, Story story)
    {
        if (story.currentChoices.Count == 0)
            return;

        // Clear the choices
        ClearDialogueOptions();

        foreach (Choice choice in story.currentChoices)
        {
            // Create a button for the choice and set the choice text
            VisualElement dialogVE = dialogOptionBtn.CloneTree();
            ButtonSH dialogButton = dialogVE.Q<ButtonSH>();
            dialogButton.text = choice.text.Trim();

            dialogButton.clicked += () => {
                story.ChooseChoiceIndex(choice.index);
                DialogueManager.Instance.ContinueStoryPublic(dialogue, story);
            };
            dialogOptions.Add(dialogVE);
        }
    }

    // ------------------------ Dialogue Box ----------------------------
    public void ClearDialogueBox()
    {
        dialogPersonName.text = "";
        dialogContent.text = "";
    }

    public void SetDialogueBox(string personName, string dialogText)
    {
        dialogPersonName.text = personName;
        dialogContent.text = "";
        currentDialogString = dialogText;
        StartCoroutine(AddDialogue(dialogText));
    }

    public void AppendDialogue(string dialogText)
    {
        currentDialogString += dialogText;
        StartCoroutine(AddDialogue(dialogText));
    }

    IEnumerator AddDialogue(string dialogText)
    {
        dialogRunning = true;
        foreach (char c in dialogText)
        {
            dialogContent.text += c;
            yield return null;
        }
        dialogRunning = false;
    }

    public void SkipDialogue()
    {
        StopAllCoroutines();
        dialogContent.text = currentDialogString;
        dialogRunning = false;
    }
}
