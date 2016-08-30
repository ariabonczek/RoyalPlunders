using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider))]
public class SpikeTrap : Trap
{
    BoxCollider boxCollider;

    bool isTriggered = false;
    bool isPlayerInside = false;

    const float TRAP_DELAY = 1.0f;
    float trapTimer = 0.0f;

	// Use this for initialization
	public override void Start ()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
	}
	
	// Update is called once per frame
	public override void Update ()
    {
	    if(isTriggered)
        {
            trapTimer += Time.deltaTime;

            if (trapTimer >= TRAP_DELAY)
            {
                trapTimer = 0.0f;
                isTriggered = false;

                if (isPlayerInside)
                {
                    (Player.instance as IAttackable).TakeDamage();
                }
            }
        }
	}

    public override void Trigger()
    {
        isPlayerInside = true;
        isTriggered = true;
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player")
        {
            Trigger();
        }
    }

    void OnTriggerExit(Collider c)
    {
        if(c.tag == "Player")
        {
            isPlayerInside = false;
        }
    }
}
