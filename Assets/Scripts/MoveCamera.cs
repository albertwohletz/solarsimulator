using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
	Camera cam;
	private GameObject focus = null;
	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown ("a")){
			cam.transform.Translate(0,0,-1 * cam.transform.localPosition.z/2);
		} else if (Input.GetKeyDown("o")){
			cam.transform.Translate(0,0,cam.transform.localPosition.z/2);
		}
		if (cam.transform.position.z >= 0){
			cam.transform.Translate (0,0,cam.transform.position.z * -2);
		}
	}

	public void SetFocus(string target) {
		GameObject t = GameObject.Find (target);
		cam.transform.parent = t.transform;
		cam.transform.localPosition = new Vector3 (0, 0, cam.transform.localPosition.z);
	}
}
