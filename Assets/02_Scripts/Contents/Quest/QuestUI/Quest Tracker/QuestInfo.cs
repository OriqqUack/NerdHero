using System.Linq;
using UnityEngine;

public class QuestInfo : MonoBehaviour
{
    [SerializeField]
    private QuestTracker questTrackerPrefab;
    [SerializeField]
    private CategoryColor[] categoryColors;
    
    public void CreateQuestTracker(Quest quest)
    {
        var categoryColor = categoryColors.FirstOrDefault(x => x.category == quest.Category);
        var color = categoryColor.category == null ? Color.white : categoryColor.color;
        Instantiate(questTrackerPrefab, transform).Setup(quest, color);
    }

    [System.Serializable]
    private struct CategoryColor
    {
        public Category category;
        public Color color;
    }
}
