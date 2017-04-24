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

	void update(){
		if (unlocked && Time.time > time) {
			door.SetActive (true);
			unlocked = false;
		}
	}

}
