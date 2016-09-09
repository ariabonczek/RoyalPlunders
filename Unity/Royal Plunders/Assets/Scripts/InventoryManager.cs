﻿using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour 
{
    // right now inventory only contains keys
    public int numKeys;

    public int GetNumKeys()
    {
        return numKeys;
    }

    public void LoseKey()
    {
        numKeys--;
    }

    public void GainKey()
    {
        numKeys++;
    }
}
