using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptLobbyFull : PromptWindow {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void CancelButtonClicked(){

		MyNetworkLobbyManager.singelton.ShowPromptWindow (this, false);

	}
}
