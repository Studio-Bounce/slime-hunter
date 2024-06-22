using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestAdded : Menu
{
    // Cached as its getting called in OnDestroy
    QuestManager questManager;

    VisualElement canvas;
    Label questType;
    Label questName;

    [SerializeField] float questBannerTime = 2.0f;

    void Start()
    {
        questManager = QuestManager.Instance;
        questManager.OnAddQuest += ShowQuestNotification;

        VisualElement root = uiDocument.rootVisualElement;
        canvas = root.Q<VisualElement>("Canvas");
        questType = canvas.Q<Label>("QuestType");
        questName = canvas.Q<Label>("QuestName");

        canvas.style.opacity = 0f;
    }

    void ShowQuestNotification(QuestSO quest)
    {
        StartCoroutine(ShowQuestNotification(quest.questName, quest.questType));
    }

    IEnumerator ShowQuestNotification(string _questName, QuestType _questType)
    {
        if (_questType == QuestType.MAIN)
        {
            questType.text = "Main Journey";
        }
        else if (_questType == QuestType.SIDE)
        {
            questType.text = "Side Quest";
        }
        else if (_questType == QuestType.HUNTING)
        {
            questType.text = "Hunting Quest";
        }
        questName.text = _questName;

        // Fade in
        float opacity = 0f;
        canvas.style.opacity = opacity;
        while (opacity < 1.0f)
        {
            opacity += Time.deltaTime;
            canvas.style.opacity = opacity;
            yield return null;
        }
        canvas.style.opacity = 1f;

        yield return new WaitForSeconds(questBannerTime);

        // Fade out
        opacity = 1f;
        canvas.style.opacity = opacity;
        while (opacity > 0f)
        {
            opacity -= Time.deltaTime;
            canvas.style.opacity = opacity;
            yield return null;
        }
        canvas.style.opacity = 0f;
    }

    private void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.OnAddQuest -= ShowQuestNotification;
        }
    }
}
