using UnityEngine;
using System.Collections;
using System;

public class MovableObstacle : Interactable
{
    Vector3 lastPlayerPosition;

    public override void Start()
    {
    }

    public override void Update()
    {

    }

    public override void StartInteraction(Player player)
    {
        Debug.Log("Attaching to movable object");
        lastPlayerPosition = player.transform.position;
        shouldEndInteraction = false;
    }

    public override void UpdateInteraction(Player player)
    {
        if (!Input.GetKey(KeyCode.Q))
        {
            shouldEndInteraction = true;
        }

        Vector3 offset = player.transform.position - lastPlayerPosition;

        offset.Scale(transform.right);

        transform.position += offset;

        lastPlayerPosition = player.transform.position;

        player.ClipMoveToAxis(transform.right * 0.5f);
    }

    public override void EndInteraction(Player player)
    {

    }
}
