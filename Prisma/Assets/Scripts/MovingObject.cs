using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

	public Vector3 lastpos;

	public Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {

		lastpos = gameObject.transform.position;

		rigidBody = gameObject.GetComponent<Rigidbody2D> ();

		rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 curpos = gameObject.transform.position;

		if (curpos != lastpos) {
			Debug.Log ("Moving!");
			rigidBody.constraints = 0;
			lastpos = curpos;
		} else {
			
			rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

		}

	}
}
