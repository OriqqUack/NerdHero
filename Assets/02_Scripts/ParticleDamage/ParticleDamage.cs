using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
    public float triggerRadius = 1.0f; // 파티클의 충돌 범위 (반지름)
    
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        // 파티클이 트리거된 위치를 가져옴
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        foreach (var particle in inside)
        {
            // 파티클의 위치를 가져옴
            Vector3 particlePosition = particle.position;

            // 해당 위치를 중심으로 OverlapSphere를 사용하여 범위 내에 있는 적을 찾음
            Collider[] hitColliders = Physics.OverlapSphere(particlePosition, triggerRadius);
            
            foreach (var hitCollider in hitColliders)
            {
                // 적이 범위 안에 있을 경우, 데미지 처리
                if (hitCollider.CompareTag("Enemy"))
                {
                    // 예시: 적에게 데미지를 입히는 코드
                    Debug.Log("Enemy Hit by Particle");
                }
            }
        }
    }
}