using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GadgetGUI : MonoBehaviour {

    public GameObject primary;
    public GameObject secondary;
    public GameObject player;

    public Sprite cake;
    public Sprite jazz;
    public Sprite popsicle;

	// Update is called once per frame
	void Update ()
    {
        if (player.GetComponent<GadgetManager>().GetPrimaryGadget() == GadgetManager.GadgetSlotType.Empty)
        {
            primary.GetComponent<Image>().color = new Color(1,1,1,0);
        }
        else if (player.GetComponent<GadgetManager>().GetPrimaryGadget() == GadgetManager.GadgetSlotType.Popsicle)
        {
            primary.GetComponent<Image>().color = Color.white;
            primary.GetComponent<Image>().sprite = popsicle;
        }
        else if (player.GetComponent<GadgetManager>().GetPrimaryGadget() == GadgetManager.GadgetSlotType.Cake)
        {
            primary.GetComponent<Image>().color = Color.white;
            primary.GetComponent<Image>().sprite = cake;
        }
        else if (player.GetComponent<GadgetManager>().GetPrimaryGadget() == GadgetManager.GadgetSlotType.Jazz)
        {
            primary.GetComponent<Image>().color = Color.white;
            primary.GetComponent<Image>().sprite = jazz;
        }

        if (player.GetComponent<GadgetManager>().GetSecondaryGadget() == GadgetManager.GadgetSlotType.Empty)
        {
            secondary.GetComponent<Image>().color = new Color(1,1,1,0);
        }
        else if (player.GetComponent<GadgetManager>().GetSecondaryGadget() == GadgetManager.GadgetSlotType.Popsicle)
        {
            secondary.GetComponent<Image>().color = Color.white;
            secondary.GetComponent<Image>().sprite = popsicle;
        }
        else if (player.GetComponent<GadgetManager>().GetSecondaryGadget() == GadgetManager.GadgetSlotType.Cake)
        {
            secondary.GetComponent<Image>().color = Color.white;
            secondary.GetComponent<Image>().sprite = cake;
        }
        else if (player.GetComponent<GadgetManager>().GetSecondaryGadget() == GadgetManager.GadgetSlotType.Jazz)
        {
            secondary.GetComponent<Image>().color = Color.white;
            secondary.GetComponent<Image>().sprite = jazz;
        }
	}
}
