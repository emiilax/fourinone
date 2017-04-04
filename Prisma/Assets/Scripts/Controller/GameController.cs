using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {


	public static GameController instance;

	private GameObject[] listOfKeys;

	// Use this for initialization
	void Start () {
		
		instance = this;
		//Debug.Log ("IsServer: " + isServer);
		listOfKeys = GameObject.FindGameObjectsWithTag("Key");
		Debug.Log ("NbrOfKeys: " + listOfKeys.Length);



	}
	
	// Update is called once per frame
	void Update () {}


	public void KeyIsHit(GameObject theKey, bool keyIsHit){

		// Safety first..
		if (!theKey.CompareTag ("Key"))
			return;

		foreach (GameObject go in listOfKeys) {
			if (go.Equals (theKey)) {
				go.GetComponent<KeyScript> ().unlocked = keyIsHit;
			}
				
		}




	}

	public bool GameFinished(){

		foreach (GameObject go in listOfKeys) {
			if (!go.GetComponent<KeyScript> ().unlocked)
				return false;
		}

		return true;

	}

}


