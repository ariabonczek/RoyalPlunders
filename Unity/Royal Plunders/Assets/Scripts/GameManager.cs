using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public string GUARD_TAG;
    public string LAZER_TAG;

    public static GameObject[] guardList;
    public static GameObject[] laserList;

    void Start()
    {
        guardList = GameObject.FindGameObjectsWithTag(GUARD_TAG);
        laserList = GameObject.FindGameObjectsWithTag(LAZER_TAG);
    }
}
