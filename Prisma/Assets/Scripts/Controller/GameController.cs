﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour {


	public static GameController instance;
	private bool gameActive = false;

	public bool wantToLeave = false;



	private List<GameObject> listOfKeys;


	void Awake() {
		//If we don't currently have a game control...
		if (instance == null) {

			//...set this one to be it...
			instance = this;
		}
		//...otherwise...
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
		
	}

	// Use this for initialization
	void Start () {
		listOfKeys = new List<GameObject> ();


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
	void Update(){
		//GUILog.Log ("update" + isServer.ToString());

		if (!isServer) {
			return;
		}
		if (wantToLeave) {
			GUILog.Log ("server want to leave");
			RpcCompleted (false);
			wantToLeave = false;
		}
		if(GameFinished())
		{
			RpcCompleted (true);
		}
		/*
		GUILog.Log ("server " + wantToLeave.ToString());

		if (wantToLeave) {
			GUILog.Log ("server want to leave");
			RpcCompleted (false);
			wantToLeave = false;
		}
		else if(GameFinished())
		{
			RpcCompleted (true);
		}
		*/

	}


	public void KeyIsHit(GameObject theKey, bool keyIsHit){
		//Debug.Log (theKey.name + "keyisHitGamecontroler");
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
		
	public void UpdateWantToLeave(NetworkMessage netMsg){
		GUILog.Log ("update want to leave");
		if(netMsg.ReadMessage<IntegerMessage>().value == 1){
			wantToLeave = true;//(bool) netMsg.ReadMessage<IntegerMessage>().value;
		}else{
			wantToLeave = false;
		}

	}

	public void SetWantToLeave(){
		GUILog.Log ("set want to leave");

		//MyNetworkLobbyManager.singleton.client.Send(322, new IntegerMessage(1));
		//MyNetworkLobbyManager.singelton.client.RegisterHandler (voteCompleteMsg, OnVoteComplete);
		//wantToLeave = true;
		//CmdWantToLeave(true);
	}


		
	// Action handler for back-button for singlePlayer to last scene.
	public void SinglePlayerBackButtonPressed() {
		NetworkManager.singleton.ServerChangeScene ("LevelSelector");
	}



	[ClientRpc]
	void RpcCompleted(bool success){
		
		if (isServer) {
			GUILog.Log ("rpc server");
			//return;
		} else {
			GUILog.Log ("rpc client");
		}
		GUILog.Log ("rpc completed");
		LevelCompleteManager lvlman = GameObject.Find ("GUIPanelHost").GetComponent<LevelCompleteManager>();
		lvlman.GameComplete (success);
	}

}


