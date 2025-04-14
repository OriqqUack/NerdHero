using UnityEngine;

public class WallAligner : MonoBehaviour
{
    [Header("Wall GameObjects")]
    public GameObject wallFront;
    public GameObject wallBack;
    public GameObject wallLeft;
    public GameObject wallRight;

    [Header("Wall Settings")]
    public float wallHeight = 2f;
    public float wallThickness = 0.1f;

    void Start()
    {
        AlignWalls();
    }

    [ContextMenu("Align Walls")]
    void AlignWalls()
    {
        Transform ground = transform;

        // Unity Plane 기본 크기: 10x10 units → Scale을 반영해야 실제 크기 나옴
        float groundWidth = 10f * ground.localScale.x;
        float groundDepth = 10f * ground.localScale.z;
        Vector3 groundCenter = ground.position;

        // ───────────── FRONT & BACK ─────────────
        Vector3 wallScaleFB = new Vector3(groundWidth, wallHeight, wallThickness);
        wallFront.transform.localScale = wallScaleFB;
        wallBack.transform.localScale = wallScaleFB;

        wallFront.transform.position = groundCenter + new Vector3(0, wallHeight / 2f, groundDepth / 2f);
        wallBack.transform.position = groundCenter + new Vector3(0, wallHeight / 2f, -groundDepth / 2f);

        wallFront.transform.rotation = Quaternion.identity;
        wallBack.transform.rotation = Quaternion.identity;

        // ───────────── LEFT & RIGHT ─────────────
        Vector3 wallScaleLR = new Vector3(groundDepth, wallHeight, wallThickness);
        wallLeft.transform.localScale = wallScaleLR;
        wallRight.transform.localScale = wallScaleLR;

        wallLeft.transform.position = groundCenter + new Vector3(-groundWidth / 2f, wallHeight / 2f, 0);
        wallRight.transform.position = groundCenter + new Vector3(groundWidth / 2f, wallHeight / 2f, 0);

        wallBack.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        wallFront.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        wallLeft.transform.rotation = Quaternion.Euler(90, 90f, 0);
        wallRight.transform.rotation = Quaternion.Euler(-90, 90f, 0);
    }
}