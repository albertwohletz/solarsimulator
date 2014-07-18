﻿using UnityEngine;
using System.Collections;

public class PhysicsEngine : MonoBehaviour {
	public GameObject[] bodies;

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

		// Update Positions
		foreach (GameObject body in bodies) { 
			body.transform.Translate(body.GetComponent<PhysicsProperties>().velocity * Time.deltaTime);
		}

		// Update Rotations
		/*foreach (GameObject body in bodies){
			float p = body.GetComponent<PhysicsProperties>().rotation_period;
			body.transform.Rotate(Vector3.up * Time.deltaTime * 100);
		}*/
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
