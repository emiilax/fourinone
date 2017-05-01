using System;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour, IVoteListener {

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
	public GameObject successFailTextGameObj;
	public string successText = "Bra jobbat!";
	public string exitText = "Matchen avslutad";


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



	void Start ()
	{
		//Debug.Log ("SceneLoaded: " + SceneManager.sceneLoaded);

		text = textObj.GetComponent<Text> ();
		defaultText = text.text;
		vote = new VotingSystem (
			StaticVariables.FinnishedGameVoteMsg, 
			StaticVariables.FinnishedGameVoteCompletedMsg,
			StaticVariables.FinnishVoteFailMsg,
			MyNetworkLobbyManager.singelton.GetPlayerId(),
			MyNetworkLobbyManager.singelton.minPlayers,
			MyNetworkLobbyManager.singleton.client,
			this
		);

		lvlselector = LevelSelectorController.instance;//GameObject.Find ("SelectorMenu").GetComponent<LevelSelectorController> ();
		hostPanel = GameObject.Find ("GUIPanelHost");
	}

	void Update ()
	{



	}

	public void GameComplete(bool success){
		GUILog.Log("succeeded  " + success.ToString());
		GUILog.Log("show animation");
		GameObject[] austronauts = GameObject.FindGameObjectsWithTag("Austronaut");
		if (LevelSelectorController.instance.HasNextLevel ()) {
			nextButton.SetActive (true);
		}else{
			nextButton.SetActive (false);

		}
		if (success) {
			
			//anim.playbackTime = 0.0f;
			//anim.speed = 1.0f;
			successFailTextGameObj.GetComponent<Text> ().text = successText;
			foreach (GameObject g in austronauts) {

				if (g.activeInHierarchy) {
					Debug.Log ("Austronaut show animation");
					g.GetComponent<AustronautManager> ().ShowAnimation ();
				}
			}
			anim.SetTrigger ("LevelCompleteHost");
		} else {
			
			//anim.playbackTime = 2.0f;
			//anim.speed = 1.0f;
			//anim.StartPlayback ("LevelCompleteHost");
			anim.CrossFade("LevelCompleteHost", 0.1f, -1, 0.8f);
			//anim.SetTarget ("LevelCompleteHost", 2.0f);
			//anim.SetTrigger ("LevelCompleteHost");
			successFailTextGameObj.GetComponent<Text> ().text = exitText;
		}



		// .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
	}


	public void OnChangeLevel(){

	}





	public void ButtonBackToLobby(){
		selectedButton.transform.position = menuButton.transform.position;
		selectedButton.SetActive (true);
		vote.CastVote ("menu");
	}

	public void ButtonNextLevel(){
		selectedButton.transform.position = nextButton.transform.position;
		selectedButton.SetActive (true);
		vote.CastVote ("next");

	}

	public void ButtonRestartLevel(){
		selectedButton.transform.position = restartButton.transform.position;
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
		anim.speed = 10.0f;
		anim.SetTrigger ("Hidden");
		anim.speed = 1.0f;
		//anim.CrossFadeInFixedTime("Hidden", 0.5f);
		selectedButton.SetActive (false);
		text.text = defaultText;
		GameObject[] austronauts = GameObject.FindGameObjectsWithTag("Austronaut");
		foreach(GameObject g in austronauts){

			if (g.activeInHierarchy) {
				Debug.Log ("Austronaut show animation");
				g.GetComponent<AustronautManager> ().HideAnimation ();
			}
		}
	}

	void menu(){
		LevelSelectorController.instance.TriggerChangeLevel ();
		LevelSelectorController.instance.ToggleSelector ();
		LevelSelectorController.instance.ShowLevelGrid ();
	}

	void restart(){
		LevelSelectorController.instance.TriggerChangeLevel ();
	}

	void next(){
		//GUILog.Log ("level selector == null " + (lvlselector == null).ToString());
		GameObject nextLevel = LevelSelectorController.instance.SetNextLevel ();
		if (nextLevel == null) {
			Debug.Log ("potato");
			menu ();
			return;
		}
	}


		
}


