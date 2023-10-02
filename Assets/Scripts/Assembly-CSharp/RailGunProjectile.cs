using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class RailGunProjectile : MonoBehaviour
{
    private GameObject Point;
    public Vector3 m_Acceleration;
    public Vector3 m_AccelerationMod;

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
        Point.transform.localPosition = new Vector3(0, 2*math.sin(Time.time*9), 2*math.cos(Time.time*9));
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
        throw new NotImplementedException();
    }

    public void OnLightEnter(EntityLightCollision collision)
    {
    }

    private void OnDestroy()
    {
        
    }
    
}
