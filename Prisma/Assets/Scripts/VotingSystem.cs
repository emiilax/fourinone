﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class VotingSystem {
	// number of players in the current game
	int numPlayers;

	short voteMsg;
	short voteCompleteMsg;
	short idMsg;
	// Our connectionId should not change during game
	int connId;

	private Dictionary<int, string> votes;

	NetworkClient client;

	IVoteListener listener;


	public VotingSystem(short voteId, short voteCompleteId, short idMsg, NetworkClient client, IVoteListener listener){
		this.voteMsg = voteId;
		this.voteCompleteMsg = voteCompleteId;
		this.client = client;
		this.listener = listener;
		this.idMsg = idMsg;
		NetworkServer.RegisterHandler(voteMsg, OnVoteCast);
		NetworkServer.RegisterHandler(idMsg, OnRequestId);
		client.RegisterHandler (voteCompleteMsg, OnVoteComplete);
		client.RegisterHandler (idMsg, OnRecieveId);
		client.Send(idMsg, new IntegerMessage(0));
	}

	public void setupServer(int numPlayers){
		votes = new Dictionary<int, string> ();
		this.numPlayers = numPlayers;
	}
	//strings are used to represent a vote
	public void CastVote(string vote){
		client.Send(voteMsg, new StringMessage(connId.ToString() + " " + vote));
	}

	public void OnVoteComplete(NetworkMessage netMsg){
		
		string winner = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log (winner);
		listener.OnVoteComplete (winner);
	}

	public void OnRequestId(NetworkMessage netMsg){
		NetworkServer.SendToClient(netMsg.conn.connectionId, idMsg, new IntegerMessage(netMsg.conn.connectionId));
	}

	public void OnRecieveId(NetworkMessage netMsg){
		int id = netMsg.ReadMessage<IntegerMessage> ().value;
		GUILog.Log ("recieved id in votingsystem" + id.ToString());
		connId = id;
	}

	public void OnVoteCast(NetworkMessage netMsg){
		string vote = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log ("recieved vote " + vote);
		var idAndLevel = vote.Split ();
		int id = int.Parse(idAndLevel[0]);
		string level = idAndLevel [1];
		votes [id] = level;
		if (votes.Count == numPlayers) {
			string firstVote = null;
			bool unanimous = true;
			foreach(KeyValuePair<int, string> entry in votes)
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
				NetworkServer.SendToAll (voteCompleteMsg, new StringMessage(firstVote));
				votes.Clear ();
				listener.ServerVoteComplete (firstVote);
			}
		}

	}

}
