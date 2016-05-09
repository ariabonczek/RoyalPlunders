using UnityEngine;
using System.Collections;

public class Cake : MonoBehaviour, IPickup
{
    public const float CAKE_LIFETIME = 7.0f;
    public float remainingSlices = 7.0f;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public bool TakeABite()
    {
        remainingSlices -= Time.fixedDeltaTime;

        if(remainingSlices <= 0.0f)
        {
            Player.instance.cakes.Remove(this);
            Destroy(this.gameObject);
            return false;
        }
        return true;
    }

    void IPickup.Pickup(Player player)
    {
       // _rigidbody.useGravity = false;
       // isHeldByPlayer = true;
       // parentCache = transform.parent;
       // transform.position = player.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
       // transform.SetParent(player.transform);
    }  //

    void IPickup.Throw(Player player)
    {
       //isHeldByPlayer = false;
       //_rigidbody.isKinematic = false;
       //_rigidbody.useGravity = true;
       //_rigidbody.AddForce(player.transform.forward * Player.instance.ThrowForce, ForceMode.Impulse);
       ////agent.velocity = player.transform.forward * Player.instance.ThrowForce;
       //transform.SetParent(parentCache);
       //agent.Resume();
    }
}
