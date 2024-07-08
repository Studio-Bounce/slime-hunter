using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    MAIN,
    SIDE,
    HUNTING
};

public enum QuestStatus
{
    NATIVATE,
    ONGOING
};

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestSO : ScriptableObject
{
    public bool isComplete = false;
    public bool isActive = false;

    // Basic data
    public string questName;     // Go to ABC and do DEF
    public string levelName;     // Glade Village
    public string description;   // ...
    public QuestType questType;  // MainQuest

    // Quest objectives
    public List<QuestObjective> objectives;
    public int currentObjective = 0;

    // Quest rewards
    public List<QuestReward> rewards;
}
