using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class InkDialogueController : MonoBehaviour
{
    public bool isStoryComplete = false;  // to prevent same story being run multiple times
    bool isStoryRunning = false;  // to prevent re-trigger

    [Header("Ink Story")]
    [SerializeField] TextAsset inkStoryJSON;
    Story story;

    [Header("Story UI")]
    [SerializeField] Canvas canvas;
    [SerializeField] Text textPrefab;
    [SerializeField] Button buttonPrefab;

    private void Start()
    {
        isStoryComplete = false;
    }

    // Entry point which starts a story
    public void StartStory()
    {
        if (isStoryComplete || isStoryRunning)
            return;
        isStoryRunning = true;

        story = new Story(inkStoryJSON.text);
        ContinueStory();
    }

    // Gets called when the story ends.
    // Use this to start a quest
    protected virtual void EndStory()
    {
        StartCoroutine(DelayedClear(5.0f));
    }

    // Main function which gets called every time the story changes
    void ContinueStory()
    {
        ClearCanvas();

        // Read the story content (returns false for choices)
        while (story.canContinue)
        {
            string text = story.Continue();
            CreateTextView(text.Trim());
        }

        // Display the choices, if any
        if (story.currentChoices.Count > 0)
        {
            foreach (Choice choice in story.currentChoices)
            {
                Button button = CreateChoiceView(choice.text.Trim());
                button.onClick.AddListener(delegate
                {
                    // Tell the story about this choice
                    story.ChooseChoiceIndex(choice.index);
                    ContinueStory();
                });
            }
        }
        else
        {
            isStoryComplete = true;
            EndStory();
        }
    }

    // ------------------------ Canvas ------------------------

    void CreateTextView(string text)
    {
        Text storyText = Instantiate(textPrefab);
        storyText.text = text;
        storyText.transform.SetParent(canvas.transform, false);
    }

    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(canvas.transform, false);

        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    protected void ClearCanvas()
    {
        int childCount = canvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);
        }
    }

    protected IEnumerator DelayedClear(float timer)
    {
        isStoryRunning = false;
        yield return new WaitForSeconds(timer);
        ClearCanvas();
    }
}
