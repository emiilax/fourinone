using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCompleteManager : NetworkBehaviour, IVoteListener {

	public float restartDelay = 5f;         // Time to wait before restarting the level

	public string currentScene;
	public string nextScene;
	public GameObject game;
	GameObject hostPanel;
	LevelSelectorController lvlselector;

	Animator anim;                          // Reference to the animator component.
	float restartTimer;                     // Timer to count up to restarting the level

	VotingSystem vote;
	int hiddenHash = Animator.StringToHash("Hidden");
	int showHash = Animator.StringToHash("LevelCompleteHost");
	void Awake ()
	{		
		// Set up the reference.
		anim = GetComponent <Animator> ();
		//currentScene = "MPLevel1";
		//nextScene = "MPLevel2";
	}
	void Start(){
		vote = new VotingSystem (
			StaticVariables.FinnishedGameVoteMsg, 
			StaticVariables.FinnishedGameVoteCompletedMsg,
			StaticVariables.FinnishedIdMsg,
			MyNetworkLobbyManager.singleton.client,
			this
		);
		if (isServer) {
			vote.setupServer (MyNetworkLobbyManager.singleton.numPlayers);
		}
		lvlselector = GameObject.Find ("SelectorMenu").GetComponent<LevelSelectorController> ();
		hostPanel = GameObject.Find ("GUIPanelHost");
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
			GUILog.Log("game finnished");
			//GameObject.Find ("GUIPanelHost").SetActive (true);


			RpcShowAnimation ();

		}
	}

	public void OnChangeLevel(){

	}


	[ClientRpc]
	void RpcShowAnimation(){
		GUILog.Log ("showing animation");
		// ... tell the animator the game is over.
		//if (isServer)
		//	return;

		anim.SetTrigger ("LevelCompleteHost");
		// .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
	}


	public void ButtonBackToLobby(){
		
		//anim.gameObject.SetActive (false);
		//RpcSendMessage("ButtonBackToLobbypressed");
		vote.CastVote ("next");
		//lvlselector.TriggerChangeLevel ();
		//lvlselector.ToggleSelector ();
		//RpcSetTrigger ("Hidden");

	}

	public void ButtonNextLevel(){
		vote.CastVote ("next");
		//RpcSendMessage("ButtonNextLevelPressed");

		//RpcSendMessage (MyNetworkLobbyManager.singelton.ToString());

		//RpcSetTrigger ("Hidden");

	}


	public void ButtonRestartLevel(){
		//if (!isServer)
		//	return;
		
		//RpcSendMessage("ButtonRestartLevelpressed");
		vote.CastVote ("restart");

		//RpcSetTrigger ("Hidden");

		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		//MyNetworkLobbyManager.singelton.ServerChangeScene (currentScene);
	}

	public void ServerVoteComplete(string winner){
		anim.SetTrigger ("Hidden");
	}

	public void OnVoteComplete(string action){
		GUILog.Log ("recieved vote complete");

		if (action.Equals ("next")) {			
			GUILog.Log (lvlselector.currentLevel.ToString());
			if (lvlselector.currentLevel.GetComponent<LevelVariables> ().nextLevel == null) {
				Debug.Log ("potato");
				ButtonBackToLobby ();
				return;
			}
			GameObject nextLevel = lvlselector.currentLevel.GetComponent<LevelVariables> ().nextLevel;
			lvlselector.ChangeLevel (nextLevel.name);
			lvlselector.TriggerChangeLevel ();
			//anim.SetTrigger ("Hidden");
			//gameObject.SetActive (false);
		}else if (action.Equals ("restart")) {
			//GameObject.Find ("GUIPanelHost").SetActive (true);
			GUILog.Log ("restarting");
			lvlselector.TriggerChangeLevel ();
		}
		else if(action.Equals("menu")){
			//GameObject.Find ("GUIPanelHost").SetActive (false);

			lvlselector.TriggerChangeLevel ();
			lvlselector.ToggleSelector ();
		}
		//anim.StopPlayback ();
		//anim.ResetTrigger (showHash);
		//anim.SetTrigger ("Hidden");
		anim.Play ("Empty");
		GUILog.Log ("hid panel");
		//anim.CrossFade ("Hidden", 0.1f);
		//anim.enabled = false;
		//GameObject.Find ("GUIPanelHost").GetComponent<Canvas>().enabled = false;
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


