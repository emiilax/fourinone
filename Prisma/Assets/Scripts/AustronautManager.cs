using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AustronautManager : NetworkBehaviour {

	Animator anim;



	// Use this for initialization
	void Start () {

		Debug.Log ("Austonaut start");

		anim = GetComponent <Animator> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void ShowAnimation(){
		anim.SetTrigger ("LevelComplete");
	}


}
