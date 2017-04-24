using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour {

	public GameObject door;	

	public bool unlocked;

	private double time;


	void Start(){
		unlocked = false;

	}

	public void HitKey(double hitTime){
		time = hitTime;

	}

	void Update(){

		if(!door){
			return;
		}

		if (Time.time > time) {

			door.SetActive (true);
			unlocked = false;
			//Debug.Log ("Show door");
		} else {

			door.SetActive (false);
			unlocked = true;
			//Debug.Log ("Hide door");
		}
	}

}
