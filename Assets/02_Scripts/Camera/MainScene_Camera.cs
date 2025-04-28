using System.Collections;
using UnityEngine;

public class MainScene_Camera : MonoBehaviour
{
    [SerializeField] private Transform targetedItem = null;
    [SerializeField] private IslandController demoController = null;
    [SerializeField] private float speed = 0;
    private Vector3 offset;

    void Awake()
    {
        offset = transform.position - targetedItem.position;
        StartCoroutine(SetCamAfterStart());
    }

    IEnumerator SetCamAfterStart()
    {
        yield return null;
        transform.position = targetedItem.position + offset;
    }
}
