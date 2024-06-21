using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using static UnityEngine.UIElements.VisualElement;

public class DialogueHUD : Menu
{
    VisualElement dialogOptions;
    Label dialogPersonName;
    Label dialogContent;

    [SerializeField] VisualTreeAsset dialogOptionBtn;
    [SerializeField] Color dialogColor;
    [SerializeField] Color dialogHoverColor;

    private void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;

        dialogOptions = root.Q<VisualElement>("DialogOptions");
        VisualElement dialogBox = root.Q<VisualElement>("DialogBox");
        dialogPersonName = dialogBox.Q<VisualElement>("Name").Q<Label>();
        dialogContent = dialogBox.Q<VisualElement>("Content").Q<VisualElement>("ContentBox").Q<Label>();
        
        Hide();
    }

    // ------------------------ Dialogue Options ------------------------
    public void ClearDialogueOptions()
    {
        UIManager.Instance.ClearVisualElement(dialogOptions);
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
            Button dialogButton = dialogVE.Q<Button>();
            dialogButton.text = choice.text.Trim();

            dialogButton.clicked += () => {
                story.ChooseChoiceIndex(choice.index);
                DialogueManager.Instance.ContinueStory(dialogue, story);
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
        dialogContent.text = dialogText;
    }

    public void AppendDialogue(string dialogText)
    {
        dialogContent.text += dialogText;
    }
}
