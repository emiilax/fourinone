using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManu : MonoBehaviour, IVoteListener {

	VotingSystem exitVote;
	public GameObject gameController;
	// Use this for initialization
	void Start () {
		exitVote = new VotingSystem (
			StaticVariables.ExitGameVoteMsg, 
			StaticVariables.ExitGameCompletedMsg,
			StaticVariables.ExitGameVoteFailMsg,
			MyNetworkLobbyManager.singelton.GetPlayerId (),
			MyNetworkLobbyManager.singelton.minPlayers,
			MyNetworkLobbyManager.singleton.client,
			this
		);
		
	}
	public void OnVoteComplete(string winner){
		GUILog.Log("voted " + winner);
		if (winner.Equals ("exit")) {
			GUILog.Log("setting want to leave");
			gameController.GetComponent<GameController> ().SetWantToLeave ();
		}
	}
	public void OnVoteFail(){

	}

	public void ServerVoteComplete (string winner){
		//gameController.GetComponent<GameController> ().SetWantToLeave ();
		GameObject.Find("GameController").GetComponent<GameController>().wantToLeave = true;
		GUILog.Log ("server vote");
	}

	// Update is called once per frame
	void Update () {
		
	}



	public void SelectedExitBtn(){
		exitVote.CastVote ("exit");
	}

	public void DeselectedExitBtn(){
		exitVote.CastVote ("notExit");
	}

}
