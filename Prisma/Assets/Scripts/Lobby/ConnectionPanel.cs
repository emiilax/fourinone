﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour {

	public MyNetworkLobbyManager lobbyManager;
	public Text connectingText;



	void start(){
		connectingText.text = "Ansluter...";
	}




}
