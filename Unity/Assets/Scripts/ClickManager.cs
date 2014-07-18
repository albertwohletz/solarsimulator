using UnityEngine;
using System.Collections;

public class ClickManager : MonoBehaviour {
	public GameObject body;
	private Camera cam;
	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			Vector3 pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f));
			pos.z = 0;
			Quaternion quat = new Quaternion();
			GameObject b = (GameObject)Instantiate(body, pos, quat);
			Color c = new Color( Random.value, Random.value, Random.value, 1.0f );
		    b.renderer.material.color = c;
			b.GetComponent<TrailRenderer>().material.SetColor("_Color", c);
		}
	}
}
