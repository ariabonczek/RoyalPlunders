using UnityEngine;
using System.Collections;
using System;

public class Chest : MonoBehaviour, Iinteractable
{
    public GameObject itemPrefab;
    bool isOpen = false;

    public string getTypeLabel()
    {
        return "Chest";
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button != InteractionButton.Y)
            return;

        if (!isOpen && itemPrefab)
        {
            Vector3 pos = transform.position + transform.forward * transform.localScale.z;
            pos += transform.forward * itemPrefab.transform.localScale.z;
            
            GameObject.Instantiate(itemPrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
            isOpen = true;
        }
    }

    public void Reset()
    {
        isOpen = false;
    }

    public bool isInstant()
    {
        return false;
    }
}
