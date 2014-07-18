using UnityEngine;
using System.Collections;

public class Randomizer : MonoBehaviour {
	public int num_items = 1000;
	public float range = 50.0f;
	public GameObject body;
	// Use this for initialization
	void Start () {
		for(int i=0; i<num_items; i++){
			Vector3 pos = new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0.0f);
			Quaternion quat = new Quaternion();
			GameObject b = (GameObject)Instantiate(body, pos, quat);
			Color c = new Color( Random.value, Random.value, Random.value, 1.0f );
			b.renderer.material.color = c;
			b.GetComponent<TrailRenderer>().material.SetColor("_Color", c);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
