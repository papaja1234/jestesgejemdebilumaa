using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickInteraction : BasePart
{
    [SerializeField] public string SpriteName;
    // Start is called before the first frame update
    void Start()
    {
        INSerializedSprite inSerializedSprite = gameObject.GetComponent<INSerializedSprite>();
        inSerializedSprite.Reload(SpriteName);
    }

    public void FixedUpdate()
    {
        return;
    }

    protected override void OnTouch()
    {
        
    }

    public override void Initialize()
    {
        
    }

    public override void OnCollisionEnter(Collision c) { }

    public override void OnCollisionStay(Collision c) { }

    public override void OnCollisionExit(Collision c) { }
}
