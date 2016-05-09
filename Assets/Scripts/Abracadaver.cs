using UnityEngine;
using System.Collections;

public class Abracadaver : MonoBehaviour, IAttackable, IPickup
{
    public const int MAXIMUM_HEALTH = 5;
    [SerializeField]
    private int health = 5;

    [SerializeField]
    GameObject smokePrefab;

    // Use this for initialization
    void Start ()
    {
        GameObject p = (Instantiate(smokePrefab, transform.position, transform.rotation) as GameObject);

        p.transform.parent = this.gameObject.transform;
        p.GetComponent<ParticleSystem>().Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool IAttackable.TakeDamage()
    {
        --health;

        if(health <= 0)
        {
            Player.instance.abracadavers.Remove(this);
            Destroy(this.gameObject);
            return false;
        }
        return true;
    }

    void IPickup.Pickup(Player player)
    {

    }

    void IPickup.Throw(Player player)
    {

    }
}
