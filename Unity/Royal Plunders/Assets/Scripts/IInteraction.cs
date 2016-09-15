using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IInteractable
{
    void Interact();
    void CheckType();
}

public class Interactor : MonoBehaviour
{
    public List<GameObject> listOfInteractables;

    private void Sort(GameObject other)
    {
    }

    public void addInteractable(GameObject other)
    {
    }

    public void removeInteractable(GameObject other)
    {
    }
}