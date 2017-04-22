using System;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelCompleteManager : NetworkBehaviour, IVoteListener {

	public float restartDelay = 5f;         // Time to wait before restarting the level

	public string currentScene;
	public string nextScene;
	public GameObject game;
	GameObject hostPanel;
	LevelSelectorController lvlselector;

	public GameObject selectedButton;
	public GameObject nextButton;
	public GameObject menuButton;
	public GameObject restartButton;
	public GameObject textObj;

	Text text;
	string defaultText;
	string voteFailtext = "Alla måste välja samma!";

	Animator anim;                          // Reference to the animator component.
	float restartTimer;                     // Timer to count up to restarting the level

	VotingSystem vote;
	void Awake ()
	{
		anim = GetComponent <Animator> ();
	}

	void Start(){
		text = textObj.GetComponent<Text> ();
		defaultText = text.text;
		selectedButton.SetActive (false);
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
		
		if(GameController.instance.GameFinished())
		{
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
		selectedButton.transform.position = menuButton.transform.position;
		selectedButton.SetActive (true);
		vote.CastVote ("menu");
	}

	public void ButtonNextLevel(){
		selectedButton.transform.position = menuButton.transform.position;
		selectedButton.SetActive (true);
		vote.CastVote ("next");

	}

	public void ButtonRestartLevel(){
		selectedButton.transform.position = menuButton.transform.position;
		selectedButton.SetActive (true);
		vote.CastVote ("restart");
	}

	public void ServerVoteComplete(string winner){
		anim.SetTrigger ("Hidden");
	}

	public void OnVoteFail(){
		text.text = voteFailtext;
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
		selectedButton.SetActive (false);
		text.text = defaultText;
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
		GameObject nextLevel = lvlselector.SetNextLevel ();
		if (nextLevel == null) {
			Debug.Log ("potato");
			menu ();
			return;
		}
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


