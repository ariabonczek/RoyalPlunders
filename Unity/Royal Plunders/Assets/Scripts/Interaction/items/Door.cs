using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Iinteractable
{
    private Quaternion startingRotation;
    private bool isOpen = false;
    private Transform hinge;

    public float rotation = 90;
    public float rotationSpeed = 1;

    public int numKeyReq = 0;
    
    void Start()
    {
        hinge = transform.parent;
        startingRotation = hinge.localRotation;
    }

    void Update()
    {
        if (isOpen)
        {
            Quaternion quat = Quaternion.Euler(0, startingRotation.eulerAngles.y + rotation, 0);
            hinge.localRotation = Quaternion.Lerp(hinge.localRotation, quat, Time.deltaTime * rotationSpeed);
        }
        else
        {
            hinge.localRotation = Quaternion.Lerp(hinge.localRotation, startingRotation, Time.deltaTime * rotationSpeed);
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

        if (invManager == null)
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
