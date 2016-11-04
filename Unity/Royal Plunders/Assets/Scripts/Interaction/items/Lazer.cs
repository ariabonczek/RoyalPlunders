using UnityEngine;
using System.Collections;

public class Lazer : MonoBehaviour , Iinteractable
{
    public GameObject alarmSystem;

    public void interact(InteractionButton button, GameObject interactor)
    {
        alarmSystem.GetComponent<AlarmSystem>().TurnOnAlarm();
    }

    public string getTypeLabel()
    {
        return "Lazer";
    }

    public bool isInstant()
    {
        return true;
    }
}
