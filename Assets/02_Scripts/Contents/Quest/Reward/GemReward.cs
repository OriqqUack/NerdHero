using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Gem", fileName = "GemReward_")]
public class GemReward : Reward
{
    public override void Give(Quest quest)
    {
        GameManager.Instance.Gem += Quantity;
    }
}