using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBounce : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] private float speed = 10f;              // 총알 속도
    [SerializeField] private int maxBounces = 3;             // 최대 튕길 횟수
    [SerializeField] private float bounceRadius = 5f;        // 튕길 수 있는 최대 거리
    [SerializeField] private LayerMask enemyLayer;           // 적 레이어 설정

    private int bounceCount = 0;           // 현재 튕긴 횟수
    private Rigidbody rb;

    void Start()
    {
        rb.linearVelocity = transform.forward * speed; // 처음 발사 방향
    }

    void OnTriggerEnter(Collider other)
    {
        // 적을 맞췄다면
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            bounceCount++;

            if (bounceCount > maxBounces)
            {
                DestroyBullet();
                return;
            }

            // 가장 가까운 적 찾기
            Transform nextTarget = FindClosestEnemy();

            if (nextTarget != null)
            {
                Vector3 direction = (nextTarget.position - transform.position).normalized;
                rb.linearVelocity = direction * speed; // 새로운 방향으로 튕김
                transform.LookAt(nextTarget); // 방향 조정
            }
            else
            {
                DestroyBullet(); // 더 이상 튕길 적이 없으면 삭제
            }
        }
    }

    Transform FindClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, bounceRadius, enemyLayer);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
