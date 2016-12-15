using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

    public string GUARD_TAG; // globally recognized guard tag
    public string LAZER_TAG; // globally recognized laser tag
    public string INTERACT_PROMPT;

    public static GameObject[] guardList; // all the guards
    public static GameObject[] laserList; // all the lasers

    public static float fadeSpeed = 0.1f;

    public static GameObject prompt;
    public static Image promptImage;

    void Start()
    {
        guardList = GameObject.FindGameObjectsWithTag(GUARD_TAG); // find all the guards and cache them
        laserList = GameObject.FindGameObjectsWithTag(LAZER_TAG); // find all the lasers and cache them

        prompt = GameObject.FindGameObjectWithTag(INTERACT_PROMPT);
        if (prompt)
        {
            promptImage = prompt.GetComponent<Image>();
            promptImage.color = new Color(1, 1, 1, 0);
        }
    }
}
