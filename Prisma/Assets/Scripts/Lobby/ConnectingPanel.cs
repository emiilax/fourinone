﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingPanel : PromptWindow {

	public Text connectingText;

	void start(){
		connectingText.text = "Ansluter...";
	}
		
}
