using UnityEngine;
using System.Collections;

public class AlarmSystem : MonoBehaviour {

    public GameObject alarmLight;

    public float alarmLightMaxIntensity;
    public float alarmLightMinimumIntensity;
    public float alarmLightCurrentIntensity;
    public float alarmLightIncrement;
    public bool alarmActive;

    public GuardAITest.AIState alarmState;
    public GuardAITest.AIState alarmOffState;

    private float targetIntensity;
    private bool alarmTurnedOn;

    void Start()
    {
        alarmTurnedOn = false;
        targetIntensity = alarmLightMaxIntensity;
    }

	// Update is called once per frame
	void Update () 
    {
        if (alarmActive)
        {
            if (!alarmTurnedOn)
            {
                alarmLight.GetComponent<Light>().enabled = true;
            
                alarmLight.GetComponent<Light>().intensity = alarmLightCurrentIntensity;

                for (int i = 0; i < GameManager.guardList.Length; i++)
                {
                    GameManager.guardList[i].GetComponent<GuardAITest>().myState = alarmState;
                }

                alarmTurnedOn = true;
            }
            
            alarmLightCurrentIntensity = Mathf.Lerp(alarmLightCurrentIntensity, targetIntensity, alarmLightIncrement * Time.deltaTime);
            CheckIntensity();
        }
        else
        {
            alarmTurnedOn = false;
            alarmLight.GetComponent<Light>().enabled = false;
            alarmLightCurrentIntensity = alarmLightMinimumIntensity;

            for (int i = 0; i < GameManager.guardList.Length; i++)
            {
                GameManager.guardList[i].GetComponent<GuardAITest>().myState = alarmOffState;
            }
        }
	}

    public void CheckIntensity()
    {
        if (Mathf.Abs(targetIntensity - alarmLightCurrentIntensity) < alarmLightIncrement)
        {
            if (targetIntensity >= alarmLightMaxIntensity)
            {
                targetIntensity = alarmLightMinimumIntensity;
            }
            else
            {
                targetIntensity = alarmLightMaxIntensity;
            }
        }
    }
}
