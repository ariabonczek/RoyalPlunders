using UnityEngine;
using System.Collections;

public class ConvexHull : MonoBehaviour
{
    public GameObject interactableObject;

    // TODO draw the convex hull

    void OnTriggerEnter(Collider other)
    {
        Interactor inter = other.gameObject.GetComponent<Interactor>();
        if ( inter != null)
        {
            inter.addInteractable(interactableObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Interactor inter = other.gameObject.GetComponent<Interactor>();
        if ( inter != null)
        {
            inter.removeInteractable(interactableObject);
        }
    }
}