using UnityEngine;

public class OrbitalMotion : MonoBehaviour
{
    [SerializeField] private float radiusOffset = 0;
    
    [SerializeField] private Effect effect;
    [SerializeField] private Effect effect2;
    
    private Entity _entity;
    private Transform _center;
    private float _radius;
    private float _angle;
    private float _speed;
    private Effect _effect;
    private Effect _effect2;
    private Vector3 _previousPosition;
    
    public void Setup(Entity entity, float radius, float angle, float speed)
    {
        _entity = entity;
        _center = _entity.transform;
        _radius = radius;
        _angle = angle;
        _speed = speed;
        _previousPosition = transform.position;

        if (effect == null) return;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_entity.gameObject, _entity, 1);

        if (effect2 == null) return;
        _effect2 = effect2.Clone() as Effect;
        _effect2.Setup(_entity.gameObject, _entity, 1);
    }
    
    void Update()
    {
        if (_center == null) return;

        // 각도 및 위치 업데이트
        _angle += _speed * Time.deltaTime;
        float rad = _angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0.5f, Mathf.Sin(rad)) * (_radius + radiusOffset);
        transform.position = _center.position + offset;

        // 이동 방향 계산
        Vector3 moveDirection = transform.position - _previousPosition;
        _previousPosition = transform.position;

        // 이동 방향이 있을 때만 회전 적용
        if (moveDirection != Vector3.zero)
        {
            // Y축 회전만 적용 (수평 이동에 맞춤)
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_center == null) return;
        
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity == null || entity.ControlType != EntityControlType.AI) return;
        
        if(_effect != null)
            entity.SkillSystem.Apply(_effect);
        if(_effect2 != null)
            entity.SkillSystem.Apply(_effect2);
    }
}