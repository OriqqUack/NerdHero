using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OrbSpawner : MonoSingleton<OrbSpawner> 
{
    public float orbitRadius = 2f;
    public float moveDuration = 0.5f;
    public float speed = 100f;
    
    private List<GameObject> _orbs = new List<GameObject>();
    public void AddOrb(OrbGroup orbGroup, Entity owner)
    {
        GameObject orbContainer = new GameObject("OrbContainer");
        for (int i = 0; i < orbGroup.count; i++)
        {
            GameObject orb = Instantiate(orbGroup.orbObjectPrefab, transform.position, Quaternion.identity);
            orb.transform.position = transform.position; // 몸에서 시작
            _orbs.Add(orb);
            orb.transform.SetParent(orbContainer.transform);
        }
        UpdateOrbPositions(owner);
    }
    
    private void UpdateOrbPositions(Entity owner)
    {
        int count = _orbs.Count;
        if (count == 0) return;

        // 총 그룹 수 계산 (3개씩 그룹화)
        int groupCount = Mathf.CeilToInt(count / 3f);
    
        // 새로운 순서대로 오브 인덱스를 생성
        List<int> reorderedIndices = new List<int>();
        for (int positionInGroup = 0; positionInGroup < 3; positionInGroup++)
        {
            for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
            {
                int orbIndex = groupIndex * 3 + positionInGroup;
                if (orbIndex < count)
                {
                    reorderedIndices.Add(orbIndex);
                }
            }
        }

        // 재정렬된 인덱스 순서대로 오브 배치
        for (int i = 0; i < reorderedIndices.Count; i++)
        {
            int orbIndex = reorderedIndices[i];
            GameObject orb = _orbs[orbIndex];
            float angle = (360f / count) * i; // 균등한 각도 배치
            SetOrbPosition(owner, orb, angle);
        }
    
    }
    
    private void SetOrbPosition(Entity owner, GameObject orb, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 targetPos = transform.position + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
        orb.transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutQuad);
        
        OrbitalMotion motion = orb.GetComponent<OrbitalMotion>();
        motion.Setup(owner, orbitRadius, angle, speed);
    }
}
