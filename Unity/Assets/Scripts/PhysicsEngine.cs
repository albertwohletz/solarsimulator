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
    // Apply force of gravity 
    foreach (GameObject body in bodies) {
      if (body != target){
        // The distance is the difference between the two bodies' positions
        Vector3 distance =  body.transform.position - target.transform.position;

        // The acceleration vector is equal to the mass of the other object, divided by the magnitude squared, and multiplied by the Gravitational constant G.
        float acc_magnitude = body.GetComponent<PhysicsProperties>().mass / (distance.sqrMagnitude);

        // The velocity of the object is equal to its current velocity added to the product of acceleration and time elapsed.
        target.GetComponent<PhysicsProperties>().velocity = target.GetComponent<PhysicsProperties>().velocity + acc_magnitude * distance.normalized * Time.deltaTime;
      }
    }
  }

}
