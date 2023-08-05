using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingLight : PointLight
{
    public float m_HealingRadius;
    public float m_HealingAmount;
    public void LateUpdate()
    {
        if(this.HasGeneratorRef || !base.activated)return;
        Collider[] colliders = Physics.OverlapSphere(base.transform.position, m_HealingRadius);
        foreach (Collider collider in colliders)
        {
            BasePart part = collider.GetComponent<BasePart>();
            if((bool)part)
            {
                float updatedHp = part.m_hp + m_HealingAmount;
                if (updatedHp <= 0f)
                {
                    Destroy(part.gameObject);
                    return;
                }
                if (updatedHp >= part.m_maxHp)
                {
                    part.m_hp = part.m_maxHp;
                }
                else if (updatedHp < part.m_maxHp)
                {
                    part.m_hp = updatedHp;
                }
            }
        }
    }
}
