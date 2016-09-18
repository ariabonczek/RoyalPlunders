using UnityEngine;
using System.Collections;

public interface Iinteractable // to be used for anything interactable with a convex hull
{
    void interact(GameObject interactor); // call this to interact with an interactable, given an interactor gameobject
    string getTypeLabel(); // used for priority listing and permission polling
    bool isInstant(); // check if it triggers automatically
}

public static class InteractionTable
{
    // table goes here
    // priority list goes here

	public static void LoadTables()
    {
	    // load in table and priority chart
	}

    public static bool canInteract(GameObject interactor, GameObject interactable)
    {
        Interactor actor = interactor.GetComponent<Interactor>();
        Iinteractable obj = interactable.GetComponent<Iinteractable>();

        if (actor == null || obj == null)
            return false;

        if (actor.typeLabel == "None")
            return true;

        // check the table using actor.typeLabel and obj.getTypeLabel()
        return true; // TEMPORARY
    }

    // returns the difference in priority
    // + means B has higher priority than A
    // - means B has lower priority than A
    // 0 means B and A have the same priority (same label)
    public static int priorityCheck(GameObject objA, GameObject objB)
    {
        // get the priority values of the two labels using objA.getTypeLabel() and objB.getTypeLabel()
        // then return their difference
        return 0; // TEMPORARY
    }
}
