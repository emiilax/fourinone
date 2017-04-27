using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class VotingSystem {
	// number of players in the current game
	int numPlayers;

	short voteMsg;
	short voteCompleteMsg;
	short voteFailMsg;
	// Our connectionId should not change during game
	string connId;

	private Dictionary<string, string> votes;

	NetworkClient client;

	IVoteListener listener;




	public VotingSystem(short voteId, short voteCompleteId, short voteFailId, string playerid, NetworkClient client, IVoteListener listener){
		this.voteMsg = voteId;
		this.voteCompleteMsg = voteCompleteId;
		this.voteFailMsg = voteFailId;
		this.client = client;
		this.listener = listener;
		this.connId = playerid;

		NetworkServer.RegisterHandler(voteMsg, OnVoteCast);
		client.RegisterHandler (voteCompleteMsg, OnVoteComplete);
		client.RegisterHandler (voteFailMsg, OnVoteFail);

	}

	public void setupServer(int numPlayers){
		votes = new Dictionary<string, string> ();
		this.numPlayers = numPlayers;
	}

	//strings are used to represent a vote
	public void CastVote(string vote){
		GUILog.Log ("sending with id " + connId.ToString());
		client.Send(voteMsg, new StringMessage(connId.ToString() + " " + vote));
	}

	public void OnVoteComplete(NetworkMessage netMsg){
		
		string winner = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log (winner);
		listener.OnVoteComplete (winner);

	}

	public void OnVoteFail(NetworkMessage netMsg){

		string winner = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log (winner);
		listener.OnVoteFail ();

	}

	public void OnVoteCast(NetworkMessage netMsg){
		string vote = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log ("recieved vote " + vote);
		var idAndLevel = vote.Split ();
		String id = idAndLevel[0];
		string level = idAndLevel [1];
		votes [id] = level;
		if (votes.Count == numPlayers) {
			string firstVote = null;
			bool unanimous = true;
			foreach(KeyValuePair<string, string> entry in votes)
			{
				if (firstVote == null) {
					firstVote = entry.Value;
				} else {
					if (firstVote != entry.Value) {
						unanimous = false;
						break;
					} 
				}
				// do something with entry.Value or entry.Key
			}
			if (unanimous) {
				GUILog.Log ("successful vote");
				NetworkServer.SendToAll (voteCompleteMsg, new StringMessage (firstVote));
				votes.Clear ();
				listener.ServerVoteComplete (firstVote);
			} else {
				GUILog.Log ("failed vote");
				NetworkServer.SendToAll (voteFailMsg, new StringMessage ("."));
			}
		}

	}

}
