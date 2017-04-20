using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCompleteManager : NetworkBehaviour {

	public float restartDelay = 5f;         // Time to wait before restarting the level

	public string currentScene;
	public string nextScene;
	public GameObject game;
	public LevelSelectorController lvlselector;

	Animator anim;                          // Reference to the animator component.
	float restartTimer;                     // Timer to count up to restarting the level


	void Awake ()
	{		
		// Set up the reference.
		anim = GetComponent <Animator> ();
		//currentScene = "MPLevel1";
		//nextScene = "MPLevel2";
	}
		
	void Update ()
	{

		if (!isServer)
			return;


		// If the player has run out of health...
		if(GameController.instance.GameFinished())
		{

			if (isServer) {
			//	MyNetworkLobbyManager.singelton.ServerChangeScene (nextScene);
			}
			//Debug.Log ("in here");
			anim.SetTrigger ("LevelCompleteHost");

			RpcShowAnimation ();

		}
	}

	public void OnChangeLevel(){

	}


	[ClientRpc]
	void RpcShowAnimation(){
		// ... tell the animator the game is over.
		if (isServer)
			return;

		anim.SetTrigger ("LevelCompleteClient");
		// .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
	}


	public void ButtonBackToLobby(){

		if (!isServer)
			return;
		
		//anim.gameObject.SetActive (false);
		RpcSendMessage("ButtonBackToLobbypressed");

		lvlselector.TriggerChangeLevel ();
		lvlselector.ToggleSelector ();
		RpcSetTrigger ("Hidden");



	/*	if (MyNetworkLobbyManager.singelton.gameMode == "SinglePlayer") {
			MyNetworkLobbyManager.singelton.ResetFromSinglePlayer ();
		} else {
			//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
			MyNetworkLobbyManager.singelton.CancelConnection ();	
		}
	*/
	}

	public void ButtonNextLevel(){
		if (!isServer)
			return;

		RpcSendMessage("ButtonNextLevelPressed");

		RpcSendMessage (MyNetworkLobbyManager.singelton.ToString());
		if (lvlselector.currentLevel.GetComponent<LevelVariables> ().nextLevel == null) {
			Debug.Log ("potato");
			ButtonBackToLobby ();
			return;
		}
		GameObject nextLevel = lvlselector.currentLevel.GetComponent<LevelVariables> ().nextLevel;
		lvlselector.ChangeLevel (nextLevel.name);
		lvlselector.TriggerChangeLevel ();
		RpcSetTrigger ("Hidden");

		//anim.gameObject.SetActive (false);
		//MyNetworkLobbyManager.singelton.ServerChangeScene (nextScene);
	}


	public void ButtonRestartLevel(){
		if (!isServer)
			return;
		
		RpcSendMessage("ButtonRestartLevelpressed");
		lvlselector.TriggerChangeLevel ();
		RpcSetTrigger ("Hidden");

		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		//MyNetworkLobbyManager.singelton.ServerChangeScene (currentScene);
	}


	[ClientRpc]
	void RpcSendMessage(string str){
	
		Debug.Log (str);

	}
	[ClientRpc]
	void RpcSetTrigger(string str){
		anim.SetTrigger (str);
	}
		
}


