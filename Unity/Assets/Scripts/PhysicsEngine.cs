using UnityEngine;
using System.Collections;

public class PhysicsEngine : MonoBehaviour {
	public GameObject[] bodies;
	
	const float days_per_second = 2.60714285714286f;
	// Use this for initialization
	void Start () {
		bodies = GameObject.FindGameObjectsWithTag ("Planet");
		foreach (GameObject body in bodies){
			body.GetComponent<TrailRenderer>().material.SetColor("_Color", body.renderer.material.color);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Find all Planets
		bodies = GameObject.FindGameObjectsWithTag ("Planet");

		// Update Forces
		foreach (GameObject body in bodies) { 
			ApplyForces (body);
		}
		
		const float days_per_second = 2.60714285714286f;

		// Update Transform
		foreach (GameObject body in bodies) { 
			UpdatePosition(body);
			UpdateRotation(body);
		}
	}

	void UpdatePosition(GameObject body){
		body.transform.position = body.transform.position + body.GetComponent<PhysicsProperties>().velocity * Time.deltaTime;
	}

	void UpdateRotation(GameObject body){
		Vector3 y = new Vector3 (0, 1, 0);
		body.transform.Rotate(y , Time.deltaTime / body.GetComponent<PhysicsProperties>().rotation_period * days_per_second * 360.0f, Space.World );
	}


	void ApplyForces(GameObject target){
		foreach (GameObject body in bodies) {
			if (body != target){
				Vector3 distance =  body.transform.position - target.transform.position;
				if (distance.magnitude > 0){

					float acc_magnitude = body.GetComponent<PhysicsProperties>().mass / (distance.sqrMagnitude);
					target.GetComponent<PhysicsProperties>().velocity += acc_magnitude * distance.normalized * Time.deltaTime;
				}
			}
		}
	}

}
