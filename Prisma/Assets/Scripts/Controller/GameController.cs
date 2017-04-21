using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour {


	public static GameController instance;
	private bool gameActive = false;

	private List<GameObject> listOfKeys;

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
		listOfKeys = new List<GameObject> ();
		Debug.Log ("IsServer: " + isServer);

	}

	public void OnChangeLevel(){
		Debug.Log ("changing level in game controller");
		gameActive = false;
		GameObject[] allKeys = GameObject.FindGameObjectsWithTag("Key");
		List<GameObject> gameKeys = new List<GameObject> ();
		foreach (GameObject key in allKeys) {
			if (key.activeInHierarchy) {
				gameKeys.Add (key);
			}
		}
		foreach (GameObject key in gameKeys) {
			key.GetComponent<KeyScript> ().unlocked = false;
		}
		listOfKeys = gameKeys;
		gameActive = true;
	}
	
	// Update is called once per frame
	void Update () {}


	public void KeyIsHit(GameObject theKey, bool keyIsHit){
		Debug.Log (theKey.name + "keyisHitGamecontroler");
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
		if (!gameActive) {
			return false;
		}

		foreach (GameObject go in listOfKeys) {
			if (!go.GetComponent<KeyScript> ().unlocked)
				return false;
		}

		Debug.Log ("GAME IS FINISHED");
		gameActive = false;
		return true;

	}
		
	// Action handler for back-button for singlePlayer to last scene.
	public void SinglePlayerBackButtonPressed() {
		NetworkManager.singleton.ServerChangeScene ("LevelSelector");
	}

}


