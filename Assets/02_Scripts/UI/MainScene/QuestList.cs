using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestList : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private Slider questProgressSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button rewardButton;
    
    public void Setup(Quest quest)
    {
        icon.sprite = quest.Icon;
        countText.text = quest.Rewards[0].Quantity.ToString();
        questNameText.text = quest.DisplayName;
        float current = quest.TaskGroups[0].Tasks[0].CurrentSuccess;
        float maxValue = quest.TaskGroups[0].Tasks[0].NeedSuccessToComplete;
        questProgressSlider.value = current / maxValue;
        progressText.text = current + " / " + maxValue;
        
        if(quest.IsComplatable)
        {
            rewardButton.interactable = true;
            rewardButton.onClick.AddListener(() => OpenCongratulation(quest));
        }
        else
            rewardButton.interactable = false;
    }

    private void OpenCongratulation(Quest quest)
    {
        quest.Complete();
        UI_MainScene.Instance.OpenCongratulationPopup(quest);
    }
}
