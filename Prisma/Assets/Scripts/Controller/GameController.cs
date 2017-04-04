using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {


	public static GameController instance;

	private GameObject[] listOfKeys;

	void Awake() {
		//If we don't currently have a game control...
		if (instance == null)
			//...set this one to be it...
			instance = this;
		//...otherwise...
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		
		instance = this;

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
		
	// Action handler for back-button for singlePlayer to last scene.
	public void SinglePlayerBackButtonPressed() {

		// Resets the default values for multiplayer and change back to lobby scene.
		MyNetworkLobbyManager.singelton.resetFromSinglePlayer ();

	}

}


