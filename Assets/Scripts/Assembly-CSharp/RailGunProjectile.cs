using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class RailGunProjectile : MonoBehaviour
{
    private GameObject Point;
    public Vector3 m_Acceleration;
    public Vector3 m_AccelerationMod;
    public double FrequencyFactor;
    public double RadiusFactor;
    public double ExplodeRadius;
    private bool flag = false;

    private void Start()
    {
        Point = base.transform.Find("Point").gameObject;
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameTime.IsPaused())
        {
            return;
        }
        base.GetComponent<Rigidbody>().velocity = m_Acceleration;
        CircleMove();
    }

    private void FixedUpdate()
    {
        if (FrequencyFactor > 9d)
        {
            RadiusFactor *= 1.03d;
        }
        if (RadiusFactor >= ExplodeRadius && flag)
        {
            Collider[] colliders = Physics.OverlapSphere(base.transform.position, (float)ExplodeRadius);
            foreach (Collider _collider in colliders)
            {
                Rigidbody e = _collider.GetComponent<Rigidbody>();
                if (!e) continue;
                Vector3 dir = base.transform.position - e.transform.position;
                Vector3 a = (math.log(math.abs(dir) + new float3(0.0185f, 0.0185f, 0.0185f)));//WOW Ln A VECTOR IS SO COOL
                e.AddForce(dir.normalized * (a.magnitude * 17.68f), ForceMode.Impulse);
                BasePart d = e.GetComponent<BasePart>();
                if (!d) continue;
                d.Hurt(a.magnitude*(float)math.sqrt(FrequencyFactor*2.718281828));
            }

            FrequencyFactor = 0;
            RadiusFactor = 0;
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(gameObject);
        }
    }

    private void CircleMove()
    {
        Point.transform.localPosition = new Vector3((float)(RadiusFactor * math.cos(Time.timeAsDouble * FrequencyFactor)), (float)(RadiusFactor * math.sin(Time.timeAsDouble * FrequencyFactor)), 0);
    }

    public void LateUpdate()
    {
        if (GameTime.IsPaused())
        {
            return;
        }
        //m_Acceleration += m_AccelerationMod;
    }

    public void OnCollisionEnter(Collision collision)
    {
        flag = true;
        FrequencyFactor = 49d;
    }

    public void OnLightEnter(EntityLightCollision collision)
    {
    }

    private void OnDestroy()
    {
        Destroy(Point);
    }
    
}
