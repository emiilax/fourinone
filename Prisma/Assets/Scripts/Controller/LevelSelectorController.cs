﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LevelSelectorController : NetworkBehaviour {

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
		if(SceneManager.GetActiveScene ().name.Equals("LevelSelectorMP")){
			this.gameMode = "MultiPlayer";
		}else if(SceneManager.GetActiveScene ().name.Equals("LevelSelectorSP")){
			this.gameMode = "SinglePlayer";
		}else{
			Debug.Log("Forgot to set levelselectors in LevelSelectController");
			this.gameMode = null;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LevelSelected(string sceneName) {

		if (gameMode == "SinglePlayer") {

			MyNetworkLobbyManager.singelton.StartSinglePlayer (sceneName);
			
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