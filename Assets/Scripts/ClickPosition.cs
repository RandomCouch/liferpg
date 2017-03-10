using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Scripts.Utilities;

public class ClickPosition : MonoBehaviour {

	//This will give position on the map of where we clicked
	public float distance = 50f;
	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate(){
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, distance)) {
				Debug.DrawLine (ray.origin, hit.point);
				Debug.Log (hit.point);
				Debug.Log ("Point in latlong: " + VectorExtensions.GetGeoPosition (hit.point));
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
