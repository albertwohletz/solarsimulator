using UnityEngine;
using System.Collections;

public class PhysicsProperties : MonoBehaviour {
	public float mass = 5; // mass times 10^24 kgs
	public float rotation_period = 1; // Length of rotation in days.  1 = 1day, 5 = days, etc. 
	public Vector3 velocity;
	private const float sun_mass = 1988550.0f;

	// Use this for initialization
	void Start () {
		float y = 0.0f;
		// Initialize Velocity
		if (transform.position.x > 0.0f && tag == "Planet"){
			y = Mathf.Sqrt ((sun_mass + mass) / (transform.position.x));
		}
		velocity = new Vector3 (0, y, 0);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		PhysicsProperties otherprops = other.gameObject.GetComponent<PhysicsProperties> ();
		float othermass = otherprops.mass;
		if (this.mass >= othermass){
			// Compute Momentums
			Vector3 othermomentum = otherprops.mass * otherprops.velocity;
			Vector3 thismomentum = this.mass * this.velocity;

			// Compute New Properties
			this.mass = other.gameObject.GetComponent<PhysicsProperties>().mass + this.mass;
			this.velocity = (othermomentum + thismomentum)/mass;

			// Destroy smaller object
			Destroy (other.gameObject);

			// Update Size
			UpdateSize ();
		}  
	}

	void UpdateSize(){
		float m = Mathf.Sqrt (mass);
		transform.localScale = new Vector3(m,m,m);
	}
}
