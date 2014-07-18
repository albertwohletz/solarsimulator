using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {
	public GameObject planet;
	public Vector3 velocity;
	// Use this for initialization
	void Start () {
		float y = Mathf.Sqrt ((planet.GetComponent<PhysicsProperties>().mass + this.GetComponent<PhysicsProperties>().mass) / (Mathf.Abs(transform.localPosition.x)));
		velocity = new Vector3 (0, y, 0);
	}
	
	// Update is called once per frame
	void Update () {
		ApplyForces ();
		UpdatePosition ();
	}	
	void ApplyForces(){
		Vector3 distance =  planet.transform.position - transform.position;
		float acc_magnitude = planet.GetComponent<PhysicsProperties>().mass / (distance.sqrMagnitude);
		velocity += acc_magnitude * distance.normalized * Time.deltaTime;
	}

	void UpdatePosition(){
		transform.Translate(velocity * Time.deltaTime);
		//transform.Translate (planet.GetComponent<PhysicsProperties> ().velocity * Time.deltaTime); 
	}
}
