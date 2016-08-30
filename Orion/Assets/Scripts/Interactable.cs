using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour
{
    protected bool shouldEndInteraction;
    public bool ShouldEndInteraction
    {
        get { return shouldEndInteraction; }
    }

	public virtual void Start () { }

    public virtual void Update () { }

    /// <summary>
    /// Overriden by subclasses, performs interaction
    /// </summary>
    public abstract void StartInteraction(Player player);

    public abstract void UpdateInteraction(Player player);

    public abstract void EndInteraction(Player player);
}
