using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {
	public GameObject planet;
	public Vector3 velocity;
	private PhysicsProperties planetphys;
	// Use this for initialization
	void Start () {
		float distance = Vector3.Distance (planet.transform.position, transform.position);
		float y = Mathf.Sqrt ((planet.GetComponent<PhysicsProperties>().mass + this.GetComponent<PhysicsProperties>().mass) / (distance));
		velocity = new Vector3 (0, y, 0);
		planetphys = planet.GetComponent<PhysicsProperties> ();
	}

	void Update(){}
	// Update is called once per frame
	void LateUpdate () {
		ApplyForces ();
		UpdatePosition ();
	}	
	void ApplyForces(){
		Vector3 distance =  planet.transform.position - transform.position;
		float acc_magnitude = planet.GetComponent<PhysicsProperties>().mass / (distance.sqrMagnitude);
		velocity += acc_magnitude * distance.normalized * Time.deltaTime;
	}

	void UpdatePosition(){
		transform.position = transform.position + (velocity + planetphys.velocity) * Time.deltaTime;
		//transform.Translate(velocity * Time.deltaTime);
		//transform.Translate (planet.GetComponent<PhysicsProperties> ().velocity * Time.deltaTime); 
	}
}
