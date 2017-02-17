﻿using System;
using UnityEngine;
using UnityEngine.Networking;
namespace AssemblyCSharp
{
	public class MyLobbyPlayer : NetworkLobbyPlayer {
			

		public override void OnClientEnterLobby(){
				
			//Debug.Log ("Client entered lobby");
			base.OnClientEnterLobby ();
			MyNetworkLobbyManager.singelton.PlayerAdded ();
		}

		public override void OnStartLocalPlayer(){
			base.OnStartLocalPlayer ();
			SendReadyToBeginMessage ();
		}


	}
}

