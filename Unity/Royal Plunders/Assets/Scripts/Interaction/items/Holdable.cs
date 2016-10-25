using UnityEngine;
using System.Collections;

public class Holdable : MonoBehaviour, Iinteractable
{
    public float YOffsetFromOwner = 3;
    private float originalY;
    private bool held = false;

    Movement mover;
    Collider col;
    GameObject owner;

    void Start()
    {
        mover = GetComponent<Movement>();
        col = GetComponent<Collider>();
    }
	
	void Update()
    {
        if(owner && held)
            transform.position = owner.transform.position + Vector3.up * YOffsetFromOwner;
    }

     public bool IsHeld()
    {
        return held;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button != InteractionButton.Y)
            return;

        if (mover)
            mover.enabled = held;
        col.enabled = held;

        if (!held)
        {
            originalY = transform.position.y;
            owner = interactor;
        }
        else
        {
            transform.position = owner.transform.position + owner.transform.GetChild(0).forward;
            transform.position.Set(transform.position.x, originalY, transform.position.z);
        }

        held = !held;

        Movement player = interactor.GetComponent<Movement>();
        if (player)
            player.holding = held;
    }

    public string getTypeLabel()
    {
        return "Holdable";
    }

    public bool isInstant()
    {
        return false;
    }

    public void Reset()
    {
        held = false;
        owner = null;
    }
}
