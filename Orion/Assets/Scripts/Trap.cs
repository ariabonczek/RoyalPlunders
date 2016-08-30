using UnityEngine;
using System.Collections;

public abstract class Trap : MonoBehaviour
{
	// Use this for initialization
	public virtual void Start() { }

    // Update is called once per frame
    public virtual void Update() { }

    public abstract void Trigger();
}
