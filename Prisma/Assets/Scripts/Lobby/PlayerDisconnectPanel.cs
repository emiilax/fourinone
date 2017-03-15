using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisconnectPanel : PromptWindow {


	public void OkButtonPressed(){

		MyNetworkLobbyManager.singelton.ShowPromptWindow (this, false);

	}
			
}
