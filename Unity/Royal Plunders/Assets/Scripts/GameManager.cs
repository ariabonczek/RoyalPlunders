using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public string GUARD_TAG; // globally recognized guard tag
    public string LAZER_TAG; // globally recognized laser tag

    public static GameObject[] guardList; // all the guards
    public static GameObject[] laserList; // all the lasers

    void Start()
    {
        guardList = GameObject.FindGameObjectsWithTag(GUARD_TAG); // find all the guards and cache them
        laserList = GameObject.FindGameObjectsWithTag(LAZER_TAG); // find all the lasers and cache them
    }
}
