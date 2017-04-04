using System;
using UnityEngine.Networking;
using UnityEngine;

public class LevelCompleteManager : NetworkBehaviour {

	public float restartDelay = 5f;         // Time to wait before restarting the level


	Animator anim;                          // Reference to the animator component.
	float restartTimer;                     // Timer to count up to restarting the level


	void Awake ()
	{		
		// Set up the reference.
		anim = GetComponent <Animator> ();
	}
		
	void Update ()
	{

		if (!isServer)
			return;
		// If the player has run out of health...
		if(GameController.instance.GameFinished())
		{

			RpcShowAnimation ();

		}
	}

	[ClientRpc]
	void RpcShowAnimation(){
		// ... tell the animator the game is over.
		anim.SetTrigger ("LevelComplete");

		// .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
	}


	public void ButtonBackToLobby(){
		
		MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.CancelConnection ();
	
	}
}


