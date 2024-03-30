using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickInteraction : BasePart/*,IPointerDownHandler/*Goggs: just for convenience*/
{
    [SerializeField] [NotNull] public string SpriteName;

    //private Vector3 position;
    protected virtual void Start()
    {
        //EnsureRigidbody();
        //AddPhysicsRaycaster();
        //this.position = transform.po
        rigidbody.isKinematic = true;
        
        INSerializedSprite inSerializedSprite = gameObject.GetComponent<INSerializedSprite>();
        inSerializedSprite.Reload(SpriteName);
    }

    public void FixedUpdate()
    {
        //rigidbody.velocity = Vector3.zero;
        //transform.position = position;
    }
/*
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    private void AddPhysicsRaycaster()
    {
        PhysicsRaycaster physicsRaycaster = FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            WPFMonoBehaviour.ingameCamera.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }*/

    protected override void OnTouch()
    {
        //we do need this
    }

    public override void Initialize()
    {
        
    }

    public override void Awake()
    {
        base.Awake();
        EnsureRigidbody();
        //position = transform.position;
    }

    public override bool IsTriggerable()
    {
        return false;
    }

    public override void EnsureRigidbody()
    {
        //Debug.Log("CREATE RIGIDBODY");

        if (base.rigidbody != null) return;
        
        base.rigidbody = base.gameObject.AddComponent<Rigidbody>();
        base.rigidbody.constraints = (RigidbodyConstraints)56;
        base.rigidbody.mass = 1;
        base.rigidbody.drag = 999f;
        base.rigidbody.angularDrag = 999f;
        base.rigidbody.useGravity = false;
        base.rigidbody.freezeRotation = true;
        base.rigidbody.isKinematic = true;
        base.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        if (base.gameObject.layer == LayerMask.NameToLayer("Default") || base.gameObject.layer == LayerMask.NameToLayer("Contraption"))
        {
            base.gameObject.layer = LayerMask.NameToLayer("Contraption");
            for (int i = 0; i < base.transform.childCount; i++)
            {
                base.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Contraption");
            }
        }
    }

    public override void OnCollisionEnter(Collision c) { }

    public override void OnCollisionStay(Collision c) { }

    public override void OnCollisionExit(Collision c) { }
}