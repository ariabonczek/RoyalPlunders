using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour 
{
    // right now inventory only contains keys
    public int numKeys = 0; // keys left in inventory
    public int MAX_KEYS = 100; // max keys per inventory

    // get the number of available keys
    public int GetNumKeys()
    {
        return numKeys;
    }

    // lose a key
    // returns true if there was a key dropped
    public bool LoseKey()
    {
        bool bRetVal = false;

        if (numKeys > 0) // if there are keys
        {
            numKeys--; // decriment the count
            bRetVal = true; // state a key was lost
        }

        return bRetVal;
    }

    // gain a key
    // returns true if the key was aquired
    public bool GainKey()
    {
        bool bRetVal = false;

        if (numKeys < MAX_KEYS) // if there is room for a new key
        {
            numKeys++; // incriment the count
            bRetVal = true; // state a key was gained
        }

        return bRetVal;
    }
}
