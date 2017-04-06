using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LevelSelectorController : NetworkBehaviour {

	public static LevelSelectorController instance;

	// The standard UI
	public GameObject levelMenu;

	// The different panels of the different game modes.
	public GameObject singlePlayerPanel;
	public GameObject multiPlayerPanel;

	// The panel for the clients that are not the server
	public GameObject clientPanel;

	// The current gamemode. SinglePlayer or MultiPlayer.
	public string gameMode;

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

		gameMode = MyNetworkLobbyManager.singelton.gameMode;

		if (gameMode == "SinglePlayer") {

			singlePlayerPanel.SetActive (true);
			multiPlayerPanel.SetActive (false);

		} else if (gameMode == "MultiPlayer") {

			if (isServer) {
				multiPlayerPanel.SetActive (true);
			} else {
				clientPanel.SetActive (true);
				levelMenu.SetActive (false);
				multiPlayerPanel.SetActive (false);
			}

			singlePlayerPanel.SetActive (false);

		} 

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LevelSelected(string sceneName) {

		if (gameMode == "SinglePlayer") {

			MyNetworkLobbyManager.singelton.ServerChangeScene (sceneName);
			
		} else if (gameMode == "MultiPlayer") {
			
			if (isServer) {
				MyNetworkLobbyManager.singelton.ServerChangeScene (sceneName);
			}

		}
		
	}

	// Action handler for back-button for singlePlayer to last scene.
	public void SinglePlayerBackButtonPressed() {

		// Resets the default values for multiplayer and change back to lobby scene.
		MyNetworkLobbyManager.singelton.ResetFromSinglePlayer ();

	}

	// Action handler for back-button for multiPlayer to last scene.
	public void MultiPLayerBackButtonPressed() {

	}


}
