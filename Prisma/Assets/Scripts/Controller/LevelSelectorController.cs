using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorController : MonoBehaviour {

	public static LevelSelectorController instance;

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

		this.gameMode = "SinglePlayer";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LevelSelected(string sceneName) {

		if (gameMode == "SinglePlayer") {

			MyNetworkLobbyManager.singelton.startSinglePlayer (sceneName);
			
		} else if (gameMode == "MultiPlayer") {
			
		}
		
	}

	// Action handler for back-button for singlePlayer to last scene.
	public void SinglePlayerBackButtonPressed() {

		// Resets the default values for multiplayer and change back to lobby scene.
		MyNetworkLobbyManager.singelton.resetFromSinglePlayer ();

	}

	// Action handler for back-button for multiPlayer to last scene.
	public void MultiPLayerBackButtonPressed() {

	}
}
