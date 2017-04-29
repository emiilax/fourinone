using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncScreenController : MonoBehaviour, IVoteListener {

	public static SyncScreenController instance;

	public GameObject levelSelector;

	private bool[] isReadyBtnPressed;

	public GameObject[] players;
	public GameObject[] playerController;

	VotingSystem vote;

	void Awake() {

		//If we don't currently have a game control...
		if (instance == null){
			//...set this one to be it...
			instance = this;

		}
		//...otherwise...
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);

	}

	public void OnVoteComplete(string winner){
		gameObject.SetActive (false);
		GUILog.Log ("vote complete " + winner);
		levelSelector.SetActive (true);
		levelSelector.GetComponent<LevelSelectorController> ().ShowLevelGrid ();
	}


	public void OnVoteFail(){
		GUILog.Log ("vote fail");
	}

	public void ServerVoteComplete (string winner){
	}

	// Use this for initialization
	void Start () {
		
		if (MyNetworkLobbyManager.singelton.gameMode == "MultiPlayer") {
			vote = new VotingSystem (
				StaticVariables.SyncVoteMsg, 
				StaticVariables.SyncVoteCompletedMsg,
				StaticVariables.SyncVoteFailMsg,
				MyNetworkLobbyManager.singelton.GetPlayerId(),
				MyNetworkLobbyManager.singelton.minPlayers,
				MyNetworkLobbyManager.singleton.client,
				this
			);
			EnablePlayer (false);
		}
		else if(MyNetworkLobbyManager.singelton.gameMode == "SinglePlayer"){
			GUILog.Log ("singleplayer sync screen");
			gameObject.SetActive (false);
			levelSelector.SetActive (true);
			levelSelector.GetComponent<LevelSelectorController> ().ShowLevelGrid ();
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

	public void ButtonActivated(){
		GUILog.Log ("button active");
		vote.CastVote ("ready");
	}
	public void ButtonDeactivated(){
		GUILog.Log ("button not active");
		vote.CastVote ("notReady");
	}



	public void StartGame() {

	}


}