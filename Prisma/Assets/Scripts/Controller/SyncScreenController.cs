using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncScreenController : NetworkBehaviour {


	void Awake() {

		EnablePlayer (false);

	}
		

	// Use this for initialization
	void Start () {

		EnablePlayer (false);

	}
		
		
	// Enable or disable the player's prefabs.
	// Important to only change the children of the playerprefab so it can initialize the right spawn positions. 
	void EnablePlayer(bool b) {

		foreach (Renderer r in MyNetworkLobbyManager.singelton.gamePlayerPrefab.GetComponentsInChildren<Renderer>()) {
			Debug.Log ("[SyncScreen]: In EnablePlayer()");
			r.GetComponent<Renderer> ().enabled = b;
		}
	}

	public void ReadyButtonPressed() {
		Debug.Log ("[SyncScreen]: PlayedID: ");
	}

	void OnDestroy() {
		EnablePlayer (true);
	}
}
