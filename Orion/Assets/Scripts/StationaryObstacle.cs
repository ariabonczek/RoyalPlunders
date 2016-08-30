using UnityEngine;
using System.Collections;
using System;

public class StationaryObstacle : Interactable
{
    [SerializeField]
    private Loot loot;
   
    // Use this for initialization
    public override void Start ()
    {

    }

    // Update is called once per frame
    public override void Update ()
    {
	
	}

    public override void StartInteraction(Player player)
    {
        Debug.Log("Picking up the " + loot.name);
        shouldEndInteraction = false;
    }

    public override void UpdateInteraction(Player player)
    {
        player.AddToInventory(loot);
        shouldEndInteraction = true;
    }

    public override void EndInteraction(Player player)
    {
        Destroy(this.gameObject);
    }
}
