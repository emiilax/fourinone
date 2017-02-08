using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery {


	public List<DiscoveredGame> discoveredGames = new List<DiscoveredGame>();

	// Use this for initialization
	void Start () {
		
		Initialize();
		StartAsClient();
		StartCoroutine(CheckGamesList());

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log ("On Recieve Broadcast");
		base.OnReceivedBroadcast (fromAddress, data);
		Debug.Log(fromAddress);
		var parts = fromAddress.Split(new char[]{':'});

		bool found = false;
		foreach(var dGame in discoveredGames)
		{
			if(dGame.networkAddress == parts[3])
			{
				found = true;
				dGame.lastSeen = Time.time;
				break;
			}
		}

		if(!found)
		{
			var dGame = new DiscoveredGame();
			dGame.networkAddress = parts[3];
			dGame.networkPort = int.Parse(data);
			dGame.lastSeen = Time.time;
			discoveredGames.Add(dGame);
		}

	}

	IEnumerator CheckGamesList()
	{
		while(true)
		{
			for(int i = discoveredGames.Count -1; i >= 0; i--)
			{
				if(discoveredGames[i].lastSeen < Time.time-1.5f)
				{
					discoveredGames.RemoveAt(i);
				}
			}
			/*
			foreach(var gButton in gameButtons)
			{
				//gButton.transform.SetParent(null, false);
				//DestroyImmediate(gButton.gameObject);
			}*/
			//gameButtons.Clear();

			foreach(var dGame in discoveredGames)
			{
				Debug.Log ("Game");
				/*var gButton = Instantiate<LanGameButton>(gameButtonPrefab);
				gButton.networkAddress = dGame.networkAddress;
				gButton.transform.SetParent(buttonGrid, false);
				gameButtons.Add(gButton);*/
			}

			yield return new WaitForSeconds(1.5f);
		}
	}


}


[System.Serializable]
public class DiscoveredGame
{
	public string networkAddress;
	public int networkPort;
	public float lastSeen;
}