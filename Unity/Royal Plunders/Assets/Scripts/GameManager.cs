using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public string GUARD_TAG;

    public static GameObject[] guardList;

    void Start()
    {
        guardList = GameObject.FindGameObjectsWithTag(GUARD_TAG);
    }
}
