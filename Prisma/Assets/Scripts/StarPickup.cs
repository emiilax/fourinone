using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPickup : MonoBehaviour {

	public bool characterInQuicksand;
	void OnTriggerEnter2D(Collider2D other) {
		characterInQuicksand = true;
		Debug.Log ("Hit!");

		GameObject gObj = other.gameObject;

		Destroy (gameObject);
	}
}
