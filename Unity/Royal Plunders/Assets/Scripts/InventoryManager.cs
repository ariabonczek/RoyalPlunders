using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour 
{
    // right now inventory only contains keys
    public int numKeys = 0;
    public int MAX_KEYS = 100;

    public int GetNumKeys()
    {
        return numKeys;
    }

    public bool LoseKey()
    {
        bool bRetVal = false;

        if (numKeys > 0)
        {
            numKeys--;
            bRetVal = true;
        }

        return bRetVal;
    }

    public bool GainKey()
    {
        bool bRetVal = false;

        if (numKeys < MAX_KEYS)
        {
            numKeys++;
            bRetVal = true;
        }

        return bRetVal;
    }
}
