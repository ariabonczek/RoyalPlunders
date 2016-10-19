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

    public void interact(GameObject interactor)
    {
        if (!isOpen && itemPrefab)
        {
            Vector3 pos = transform.position + transform.forward * transform.localScale.z;
            pos += transform.forward * itemPrefab.transform.localScale.z;
            
            GameObject.Instantiate(itemPrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
            isOpen = true;
        }
    }

    public bool isInstant()
    {
        return false;
    }
}
