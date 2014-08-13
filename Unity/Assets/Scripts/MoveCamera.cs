using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
	Camera cam;
	private GameObject focus = null;
	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		UpdateTrailLengths(cam.transform.localPosition.z*trail_scale);
	}
	const float trail_scale = -0.005f;
	// Update is called once per frame
	void Update () {
		cam.transform.rotation = Quaternion.Euler(Vector3.zero);
		if(Input.GetKeyDown ("a")){
			ZoomIn();
		} else if (Input.GetKeyDown("o")){
			ZoomOut();
		}
	}

	public void ZoomIn(){ 
		cam.transform.Translate(0,0,-1 * cam.transform.localPosition.z/2);
		UpdateTrailLengths(cam.transform.localPosition.z*trail_scale);
	}

	public void ZoomOut(){
		cam.transform.Translate(0,0,cam.transform.localPosition.z/2);
		UpdateTrailLengths(cam.transform.localPosition.z*trail_scale);
	}

	public void SetFocus(string target) {
		GameObject t = GameObject.Find (target);
		focus = t;
		cam.GetComponent<SmoothFollow> ().target = t.transform;
		//cam.transform.parent = t.transform;
//		cam.transform.localPosition = new Vector3 (0, 0, cam.transform.localPosition.z);
	}

	void UpdateTrailLengths(float value){
		GameObject[] bodies = GameObject.FindGameObjectsWithTag ("Planet");
		foreach (GameObject body in bodies){
			body.GetComponent<TrailRenderer>().startWidth = value;
			body.GetComponent<TrailRenderer>().endWidth = value;
		}
	}
}
