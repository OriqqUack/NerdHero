using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SkillObject/ObjectScaler")]
public class SkillObjectScaler : MonoBehaviour, ISkillObjectComponent
{
    [SerializeField]
    private Vector3 baseScale;

    public void OnSetupSkillObject(SkillObject skillObject)
    {
        var scaledBaseScale = Vector3.Scale(baseScale, skillObject.ObjectScale);
        var localScale = transform.localScale;
        var resultScale = Vector3.Scale(localScale, scaledBaseScale);
        
        resultScale.x = GetValidValue(resultScale.x, localScale.x);
        resultScale.y = GetValidValue(resultScale.y, localScale.y);
        resultScale.z = GetValidValue(resultScale.z, localScale.z);

        transform.localScale = resultScale;
    }

    public void OnSetupSkillObject(SkillColliderObject skillObject)
    {
        var scaledBaseScale = Vector3.Scale(baseScale, skillObject.ObjectScale);
        var localScale = transform.localScale;
        var resultScale = Vector3.Scale(localScale, scaledBaseScale);
        
        resultScale.x = GetValidValue(resultScale.x, localScale.x);
        resultScale.y = GetValidValue(resultScale.y, localScale.y);
        resultScale.z = GetValidValue(resultScale.z, localScale.z);

        transform.localScale = resultScale;
        
        if(GetComponent<Collider>())
            AdjustCapsuleCollider(resultScale);
    }

    private float GetValidValue(float value, float defaultValue)
        => Mathf.Approximately(value, 0f) ? defaultValue : value;
    
    private void AdjustCapsuleCollider(Vector3 scale)
    {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            // 캡슐 콜라이더의 기본 크기
            float defaultRadius = capsuleCollider.radius;
            float defaultHeight = capsuleCollider.height;

            // X, Z 축 스케일을 평균내어 radius 조정 (캡슐은 보통 원형이므로)
            float maxScaleXZ = Mathf.Max(scale.x, scale.z);
            capsuleCollider.radius = defaultRadius * maxScaleXZ;

            // Y축 스케일을 적용하여 height 조정
            capsuleCollider.height = defaultHeight * scale.y;
        }
    }
}
