using UnityEngine;
using System.Collections;

public class ConvexHull : MonoBehaviour
{
    // the object this ConvexHull is triggering for
    public GameObject interactableObject;

    void OnTriggerEnter(Collider other)
    {
        if (interactableObject == null) // this convex hull is not bound to an interactable!
        {
            Debug.Log("Something triggered a ConvexHull that has no linked object! " + transform.position);
            return;
        }

        // if the object has an interactor, give this interactableObject to it
        Interactor inter = other.gameObject.GetComponent<Interactor>();
        if ( inter != null)
            inter.addInteractable(interactableObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (interactableObject == null) // this convex hull is not bound to an interactable!
        {
            Debug.Log("Something triggered a ConvexHull that has no linked object! " + transform.position);
            return;
        }

        // if the object has an interactor, take this interactableObject away from it
        Interactor inter = other.gameObject.GetComponent<Interactor>();
        if ( inter != null)
            inter.removeInteractable(interactableObject);
    }
}