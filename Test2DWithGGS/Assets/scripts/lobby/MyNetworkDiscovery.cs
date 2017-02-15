using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery {


	public List<DiscoveredGame> discoveredGames = new List<DiscoveredGame>();

    // Use this for initialization
    void Start()
    {
        Debug.Log("Discovery start");
        Initialize();
        if (StartAsClient()) { Debug.Log("client wor"); }
        StartCoroutine(CheckGamesList());
    }
	


	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log ("On Recieve Broadcast BIG DINGERING");
		base.OnReceivedBroadcast (fromAddress, data);
		Debug.Log(fromAddress);
		//var adressSplit = fromAddress.Split(new char[]{':'});
        var dataSplit = data.Split(new char[] { ':' });

        bool found = false;
		foreach(var dGame in discoveredGames)
		{
			if(dGame.networkAddress == fromAddress)
			{
				found = true;
				dGame.lastSeen = Time.time;
				break;
			}
		}

		if(!found)
		{
			var dGame = new DiscoveredGame();
            dGame.networkAddress = fromAddress;
			dGame.networkPort = int.Parse(dataSplit[2]);
            dGame.buttonColor = dataSplit[3];
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
    public string buttonColor;
}