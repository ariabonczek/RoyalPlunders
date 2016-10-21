using UnityEngine;
using System.Collections;

public interface Iinteractable // to be used for anything interactable with a convex hull
{
    void interact(InteractionButton button, GameObject interactor); // call this to interact with an interactable, given an interactor gameobject
    string getTypeLabel(); // used for priority listing and permission polling
    bool isInstant(); // check if it triggers automatically
}

public enum InteractionButton {Y, B, NONE};

public static class InteractionTable
{
    [System.Serializable]
    struct interactorPerms
    {
        public string name; // name of the interactor
        public string[] perms; // names of the valid interactables that it can interact with, as a whitelist
    }

    [System.Serializable]
    class JSONData
    {
        public string[] priority = new string[0]; // the order by which to sort interactables if the interactor has more than 1 available to it
        public interactorPerms[] permissions  = new interactorPerms[0]; // all the per-interactor permissions
    }

    static JSONData tables;

    // load in the json file for permissions
    public static void LoadTables(string path)
    {
        tables = JsonUtility.FromJson<JSONData>(Resources.Load<TextAsset>(path).text);
	}

    // check if one gameobject can interact with another
    public static bool canInteract(GameObject interactor, GameObject interactable)
    {
        Interactor actor = interactor.GetComponent<Interactor>();
        Iinteractable obj = interactable.GetComponent<Iinteractable>();

        if (actor == null || obj == null) // interaction is not possible
            return false;

        if (actor.typeLabel == "None") // no perms applied to this role, it is the wildcard role
            return true;

        // check for the interactor in the table
        int i = 0;
        for (i = 0; i < tables.permissions.Length; ++i)
        {
            if (tables.permissions[i].name == actor.typeLabel)
                break;
        }

        if (i >= tables.permissions.Length) // interactor not found
            return false;

        if (System.Array.IndexOf(tables.permissions[i].perms, obj.getTypeLabel()) != -1) // valid interaction
            return true;

        return false; // fallthrough case of no valid interaction
    }

    // returns the difference in priority
    // + means B has higher priority than A
    // - means B has lower priority than A
    // 0 means B and A have the same priority or one is not listed
    public static int priorityCheck(GameObject objA, GameObject objB)
    {
        int objALevel = System.Array.IndexOf(tables.priority, objA.GetComponent<Iinteractable>().getTypeLabel());
        int objBLevel = System.Array.IndexOf(tables.priority, objB.GetComponent<Iinteractable>().getTypeLabel());

        if (objALevel < 0 || objBLevel < 0) // incomparable
            return 0;

        return objBLevel - objALevel;
    }
}
