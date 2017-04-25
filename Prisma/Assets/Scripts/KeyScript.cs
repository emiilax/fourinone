using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KeyScript : NetworkBehaviour {

	public GameObject door;	

	public GameObject key;

	public bool unlocked;

	void Awake(){
		//Debug.Log ("I'm awake");

		//NetworkServer.Spawn (door);

	}


	public override void OnStartServer(){
		//Debug.Log ("now im active!");
	}


	void Start(){
		unlocked = false;
	}
}
