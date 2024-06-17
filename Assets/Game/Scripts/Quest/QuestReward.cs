using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    WEAPON,
    SPELL,
    CASH
};

[System.Serializable]
public class QuestReward
{
    public string rewardName;
    public RewardType rewardType;
    public int quantity;
}
