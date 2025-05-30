using UnityEngine;

public class UI_Congratue : UiWindow
{
    [Space][UnderlineTitle("Congratulations")]
    [SerializeField] private Transform rewardParent;
    [SerializeField] private RewardList rewardList;
    
    public void Setup(Quest quest)
    {
        foreach (RectTransform child in rewardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Reward reward in quest.Rewards)
        {
            Instantiate(rewardList, rewardParent).Setup(reward.Icon, reward.Quantity);
        }
    }
}
