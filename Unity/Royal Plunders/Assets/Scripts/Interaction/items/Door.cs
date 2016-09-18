using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Iinteractable
{
    private Quaternion startingRotation;
    private bool isOpen = false;

    public float rotation = 0;
    public float rotationSpeed = 1;

    public int numKeyReq = 0;
    
    void Start()
    {
        startingRotation = transform.rotation;
    }

    void Update()
    {
        if (isOpen)
        {
            Quaternion quat = Quaternion.Euler(0, rotation, 0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, quat, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, startingRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }

    public void WipeKeyReq()
    {
        numKeyReq = 0;
    }

    public void interact(GameObject interactor)
    {
        InventoryManager invManager = interactor.GetComponent<InventoryManager>();

        if (invManager == null) // not a player, right?
        {
            Open();
            return;
        }

        if (invManager.GetNumKeys() >= numKeyReq && !isOpen)
            Open();
        else if (isOpen)
            Close();
    }

    public string getTypeLabel()
    {
        return "Door";
    }

    public bool isInstant()
    {
        return false;
    }
}
