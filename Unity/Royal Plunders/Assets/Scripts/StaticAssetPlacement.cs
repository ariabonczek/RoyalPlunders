using UnityEngine;
using System.Collections;

public class StaticAssetPlacement : MonoBehaviour {

	public GameObject prefab;
	public GameObject difference;

	public int amount = 0;

	public Vector3 distance;
	public Vector3 rotation;
	// Use this for initialization
	void Start () {
		distance = difference.transform.position - this.transform.position;
		for (int i = 0; i < amount; i++) {
			Instantiate(prefab, new Vector3(this.transform.position.x + distance.x * i, this.transform.position.y + distance.y * i, this.transform.position.z + distance.z * i), Quaternion.Euler(rotation));
		
		}
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}
