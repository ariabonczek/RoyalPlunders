using UnityEngine;
using System.Collections;

public class AlarmSystem : MonoBehaviour, Iinteractable {

    public GameObject alarmLight;

    public float disabledTime;

    // shouldn't be edited, just public facing data
    public float currentTime;

    // how bright do we want the lights
    public float alarmLightMaxIntensity;
    public float alarmLightMinimumIntensity;
    public float alarmLightCurrentIntensity;
    public float alarmLightIncrement;

    // disablable for the alarm and is the alarm on
    public bool alarmDisabled;
    public bool alarmActive;

    public GuardAITest.AIState alarmState;
    public GuardAITest.AIState alarmOffState;

    private float targetIntensity;
    private float lastUpdate;

    void Start()
    {
        targetIntensity = alarmLightMaxIntensity;
        currentTime = 0;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (!alarmDisabled)
        {
            TurnOffAlarm();
        }
        alarmDisabled = true;
    }

	// Update is called once per frame
	void Update () 
    {
        if (alarmActive && !alarmDisabled)
        {
            alarmLight.GetComponent<Light>().enabled = true;

            alarmLight.GetComponent<Light>().intensity = alarmLightCurrentIntensity;

            alarmLightCurrentIntensity = Mathf.Lerp(alarmLightCurrentIntensity, targetIntensity, alarmLightIncrement * Time.deltaTime);
            CheckIntensity();
        }
        else
        {
            alarmLight.GetComponent<Light>().enabled = false;
            alarmLightCurrentIntensity = alarmLightMinimumIntensity;
        }

        UpdateDisableTimer();
	}

    public void UpdateDisableTimer()
    {
        if (alarmDisabled && (currentTime <= disabledTime) )
        {
            if (Time.time - lastUpdate >= 1f)
            {
                currentTime += 1;
                lastUpdate = Time.time;
            }
        }
        else
        {
            currentTime = 0;
            alarmDisabled = false;

            for (int i = 0; i < GameManager.laserList.Length; i++)
            {
                GameManager.laserList[i].gameObject.SetActive(true);
            }
        }
    }

    public void TurnOnAlarm()
    {
        alarmActive = true;

        for (int i = 0; i < GameManager.guardList.Length; i++)
        {
            GameManager.guardList[i].GetComponent<GuardAITest>().myState = alarmState;
        }
    }

    public void TurnOffAlarm()
    {
        alarmActive = false;
        alarmDisabled = true;

        for (int i = 0; i < GameManager.guardList.Length; i++)
        {
            GameManager.guardList[i].GetComponent<GuardAITest>().myState = alarmOffState;
        }

        for (int i = 0; i < GameManager.laserList.Length; i++)
        {
            GameManager.laserList[i].gameObject.SetActive(false);
        }
    }

    public void ResetAlarm()
    {
        alarmActive = false;
        alarmDisabled = false;

        for (int i = 0; i < GameManager.guardList.Length; i++)
        {
            GameManager.guardList[i].GetComponent<GuardAITest>().myState = alarmOffState;
        }

        for (int i = 0; i < GameManager.laserList.Length; i++)
        {
            GameManager.laserList[i].gameObject.SetActive(true);
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

    public string getTypeLabel()
    {
        return "Alarm";
    }

    public bool isInstant()
    {
        return false;
    }
}
