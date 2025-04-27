using System.Collections;
using UnityEngine;

public class MainScene_Camera : MonoBehaviour
{
    [SerializeField] private Transform targetedItem = null;
    [SerializeField] private IslandController demoController = null;
    [SerializeField] private float speed = 0;
    private Vector3 offset;
    private bool canUpdate = false;

    void Awake()
    {
        offset = transform.position - targetedItem.position;
        StartCoroutine(SetCamAfterStart());
    }

    private void Update()
    {
        if (!canUpdate) return;
    }

    IEnumerator SetCamAfterStart()
    {
        yield return null;
        transform.position = targetedItem.position + offset;
        canUpdate = true;
    }
}
