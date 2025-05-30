using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SpawnAreaRandomizer : MonoBehaviour
{
    [Header("Spawn Area Bounds (X-Z)")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("Grid Settings")]
    public int columns = 3;
    public int rows = 3;

    private void Start()
    {
        WaveManager.Instance.OnWaveStart += RepositionSpawnPoints;
        RepositionSpawnPoints(0);
    }

    [ContextMenu("Reposition Spawn Points")]
    public void RepositionSpawnPoints(int wave)
    {
        List<Transform> spawnPoints = new List<Transform>();

        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        int totalCells = columns * rows;
        if (spawnPoints.Count > totalCells)
        {
            Debug.LogWarning("Spawn points exceed grid cells. Some will overlap.");
        }

        float cellWidth = (maxX - minX) / columns;
        float cellHeight = (maxZ - minZ) / rows;

        // Generate grid cell centers
        List<Vector2> cellCenters = new List<Vector2>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float centerX = minX + col * cellWidth + cellWidth / 2f;
                float centerZ = minZ + row * cellHeight + cellHeight / 2f;
                cellCenters.Add(new Vector2(centerX, centerZ));
            }
        }

        // Shuffle the cell positions
        Shuffle(cellCenters);

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Vector2 basePos = cellCenters[i % cellCenters.Count];

            // Random offset inside the cell
            float offsetX = Random.Range(-cellWidth / 2f, cellWidth / 2f);
            float offsetZ = Random.Range(-cellHeight / 2f, cellHeight / 2f);

            Vector3 newPosition = new Vector3(basePos.x + offsetX, 0f, basePos.y + offsetZ);
            spawnPoints[i].localPosition = newPosition;
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
