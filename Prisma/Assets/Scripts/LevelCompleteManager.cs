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
			StaticVariables.FinnishVoteFailMsg,
			MyNetworkLobbyManager.singelton.GetPlayerId(),
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

			//anim.SetTrigger ("LevelCompleteHost");

			GUILog.Log("game finnished");
			//GameObject.Find ("GUIPanelHost").SetActive (true);
			RpcShowAnimation ();

		}
	}

	public void OnChangeLevel(){

	}


	[ClientRpc]
	void RpcShowAnimation(){
		//if(isServer)

		GameObject[] austronauts = GameObject.FindGameObjectsWithTag("Austronaut");


		foreach(GameObject g in austronauts){

			if (g.activeInHierarchy) {
				Debug.Log ("Austronaut show animation");
				g.GetComponent<AustronautManager> ().ShowAnimation ();
			}
		}

		anim.SetTrigger ("LevelCompleteHost");

		// .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
	}


	public void ButtonBackToLobby(){
		vote.CastVote ("menu");
	}

	public void ButtonNextLevel(){
		vote.CastVote ("next");

	}

	public void ButtonRestartLevel(){
		vote.CastVote ("restart");
	}

	public void ServerVoteComplete(string winner){
		anim.SetTrigger ("Hidden");
	}

	public void OnVoteFail(){
		
	}

	public void OnVoteComplete(string action){

		if (action.Equals ("next")) {
			next ();
		}else if (action.Equals ("restart")) {
			restart ();
		}
		else if(action.Equals("menu")){
			menu ();
		}
		anim.SetTrigger ("Hidden");
	}

	void menu(){
		lvlselector.TriggerChangeLevel ();
		lvlselector.ToggleSelector ();
		lvlselector.ShowLevelGrid ();
	}

	void restart(){
		lvlselector.TriggerChangeLevel ();
	}

	void next(){
		if (lvlselector.currentLevel.GetComponent<LevelVariables> ().nextLevel == null) {
			Debug.Log ("potato");
			menu ();
			return;
		}
		GameObject nextLevel = lvlselector.currentLevel.GetComponent<LevelVariables> ().nextLevel;
		lvlselector.ChangeLevel (nextLevel.name);
		lvlselector.TriggerChangeLevel ();
	}

	[ClientRpc]
	void RpcSendMessage(string str){
	
		Debug.Log (str);

	}
	[ClientRpc]
	void RpcSetTrigger(string str){

		if (str.Equals ("Hidden")) {
			GameObject[] austronauts = GameObject.FindGameObjectsWithTag("Austronaut");


			foreach(GameObject g in austronauts){

				if (g.activeInHierarchy) {
					
					g.GetComponent<AustronautManager> ().HideAnimation ();
				}
			}
		}

		anim.SetTrigger (str);
	}
		
}


