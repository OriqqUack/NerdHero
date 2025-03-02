using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class FloatingTextView : MonoSingleton<FloatingTextView>
{
    private class FloatingElementData
    {
        public TextMeshProUGUI TextMesh { get; private set; }
        public Image Icon { get; private set; }
        public float CurrentDuration { get; set; }

        public FloatingElementData(TextMeshProUGUI textMesh, Image icon)
        {
            TextMesh = textMesh;
            Icon = icon;
        }
    }

    private class FloatingElementGroup
    {
        public List<FloatingElementData> elementDatas = new();
        public Transform TraceTarget { get; private set; }
        public RectTransform GroupTransform { get; private set; }
        public IReadOnlyList<FloatingElementData> ElementDatas => elementDatas;

        public FloatingElementGroup(Transform traceTarget, RectTransform groupTransform)
            => (TraceTarget, GroupTransform) = (traceTarget, groupTransform);

        public void AddData(FloatingElementData elementData)
            => elementDatas.Add(elementData);

        public void RemoveData(FloatingElementData elementData)
            => elementDatas.Remove(elementData);
    }

    [SerializeField]
    private RectTransform canvasTransform;

    [Space]
    [SerializeField]
    private GameObject elementGroupPrefab;
    [SerializeField]
    private GameObject floatingTextPrefab;
    [SerializeField]
    private GameObject floatingImagePrefab;

    [Space]
    [SerializeField]
    private float floatingDuration;

    private readonly Dictionary<Transform, FloatingElementGroup> elementGroupsByTarget = new();
    private readonly Queue<Transform> removeTargetQueue = new();
    private readonly Queue<FloatingElementData> removeElementDataQueue = new();

    private void LateUpdate()
    {
        foreach ((var traceTarget, var elementGroup) in elementGroupsByTarget)
        {
            UpdatePosition(elementGroup);

            foreach (var elementData in elementGroup.ElementDatas)
            {
                elementData.CurrentDuration += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elementData.CurrentDuration / floatingDuration);

                if (elementData.TextMesh != null)
                {
                    var textColor = elementData.TextMesh.color;
                    textColor.a = alpha;
                    elementData.TextMesh.color = textColor;
                }

                if (elementData.Icon != null)
                {
                    var iconColor = elementData.Icon.color;
                    iconColor.a = alpha;
                    elementData.Icon.color = iconColor;
                }

                if (elementData.CurrentDuration >= floatingDuration)
                    removeElementDataQueue.Enqueue(elementData);
            }

            while (removeElementDataQueue.Count > 0)
            {
                var targetElementData = removeElementDataQueue.Dequeue();
                if (targetElementData.TextMesh != null)
                    Destroy(targetElementData.TextMesh.gameObject);
                if (targetElementData.Icon != null)
                    Destroy(targetElementData.Icon.gameObject);
                
                elementGroup.RemoveData(targetElementData);
            }

            if (elementGroup.elementDatas.Count == 0)
                removeTargetQueue.Enqueue(traceTarget);
        }

        while (removeTargetQueue.Count > 0)
        {
            var removeTarget = removeTargetQueue.Dequeue();
            Destroy(elementGroupsByTarget[removeTarget].GroupTransform.gameObject);
            elementGroupsByTarget.Remove(removeTarget);
        }
    }

    private void UpdatePosition(FloatingElementGroup group)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(group.TraceTarget.position);
        Vector2 uiPosition = (viewportPosition * canvasTransform.sizeDelta) - (canvasTransform.sizeDelta * 0.5f);

        group.GroupTransform.anchoredPosition = uiPosition;
    }

    public void Show(Transform traceTarget, string text = null, Color? textColor = null, Sprite iconSprite = null)
    {
        var elementGroup = CreateCachedGroup(traceTarget);

        TextMeshProUGUI textMesh = null;
        if (!string.IsNullOrEmpty(text))
        {
            textMesh = Instantiate(floatingTextPrefab, elementGroup.GroupTransform).GetComponent<TextMeshProUGUI>();
            textMesh.text = text;
            textMesh.color = textColor ?? Color.white;
        }

        Image icon = null;
        if (iconSprite != null)
        {
            icon = Instantiate(floatingImagePrefab, elementGroup.GroupTransform).GetComponent<Image>();
            icon.sprite = iconSprite;
            icon.color = textColor ?? Color.white;
        }

        var newElementData = new FloatingElementData(textMesh, icon);
        elementGroup.AddData(newElementData);
    }

    private FloatingElementGroup CreateCachedGroup(Transform traceTarget)
    {
        if (!elementGroupsByTarget.ContainsKey(traceTarget))
        {
            var group = Instantiate(elementGroupPrefab, transform);
            var newElementGroup = new FloatingElementGroup(traceTarget, group.GetComponent<RectTransform>());
            elementGroupsByTarget[traceTarget] = newElementGroup;

            UpdatePosition(newElementGroup);
        }

        return elementGroupsByTarget[traceTarget];
    }
}