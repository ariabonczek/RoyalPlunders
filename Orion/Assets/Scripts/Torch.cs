using UnityEngine;
using System.Collections;

public class Torch : Interactable
{
    Light light;

	void Start ()
    {
        light = GetComponentInChildren<Light>();
	}
	
	void Update ()
    {
	
	}

    public override void StartInteraction(Player player)
    {
        light.enabled = !light.enabled;
        shouldEndInteraction = true;
    }

    public override void UpdateInteraction(Player player)
    {
        
    }

    public override void EndInteraction(Player player)
    {
        shouldEndInteraction = false;
    }

}
