using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncScreenController : NetworkBehaviour {

	public static SyncScreenController instance;

	public GameObject levelSelector;

	private bool[] isReadyBtnPressed;

	public GameObject[] players;
	public GameObject[] playerController;

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

		if (MyNetworkLobbyManager.singelton.gameMode == "MultiPlayer") {

			players = GameObject.FindGameObjectsWithTag("PlayerOnlineTouch");
			playerController = GameObject.FindGameObjectsWithTag("TouchController");
			isReadyBtnPressed = new bool[MyNetworkLobbyManager.singelton.minPlayers];

			EnablePlayer (false);
		}
	}


	// Enable or disable the player's prefabs.
	// Important to only change the children of the playerprefab so it can initialize the right spawn positions. 
	public void EnablePlayer(bool b) {

		foreach (GameObject player in players) {
			foreach (SpriteRenderer r in player.GetComponentsInChildren<SpriteRenderer>()) {
				r.enabled = b;
			}
		}

		EnablePlayerController (b);

	}

	public void EnablePlayerController(bool b) {
		foreach (GameObject controller in playerController) {
			controller.SetActive (b);
		}
	}

	// When ready-button is pressed
	public void ReadyBtnPressed(GameObject player, GameObject button) {

		var id = int.Parse(button.name);

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

		player.GetComponent<AimShootingMultiTouch> ().CmdSyncScreenStartGame();
	}

	public void StartGame() {
		GUILog.Log ("Im HERE1");


		levelSelector.GetComponent<LevelSelectorController> ().ToggleSelector ();
		GUILog.Log ("Im HERE2");
		EnablePlayer (true);
		GUILog.Log ("Im HERE3");
		gameObject.SetActive (false);

		GUILog.Log ("Im HERE4");

	}
}