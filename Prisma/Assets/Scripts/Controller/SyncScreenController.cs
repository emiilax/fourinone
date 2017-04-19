using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncScreenController : NetworkBehaviour {
	
	public static SyncScreenController instance;

	private bool[] isReadyBtnPressed;
	public GameObject[] players;

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

		players = GameObject.FindGameObjectsWithTag("PlayerOnlineTouch");
		isReadyBtnPressed = new bool[MyNetworkLobbyManager.singelton.minPlayers];

		EnablePlayer (false);
	}
		
		
	// Enable or disable the player's prefabs.
	// Important to only change the children of the playerprefab so it can initialize the right spawn positions. 
	public void EnablePlayer(bool b) {

		foreach (GameObject player in players) {
			foreach (SpriteRenderer r in player.GetComponentsInChildren<SpriteRenderer>()) {
				r.enabled = b;
			}
		}

	}

	// When ready-button is pressed. True = opacity 50%. False = opacity 100%. 
	public void ReadyBtnPressed(int id) {

		Debug.Log ("Player id: " + id);

		if (id > isReadyBtnPressed.Length) {
			return;
		}

		if (!isReadyBtnPressed [id]) {
			isReadyBtnPressed [id] = true;
		} else {
			isReadyBtnPressed [id] = false;
		}

		// Check if all the button is pressed i.e all players are ready. 
		foreach (bool b in isReadyBtnPressed) {
			if (b == false) {
				return;
			}
		}
		startGame ();

	}

	void startGame() {
		
		MyNetworkLobbyManager.singelton.ServerChangeScene ("LevelSelector");

		// Put this in LevelSelectorController because it eanabled it too fast
		//EnablePlayer (true);

	}
}
