using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawn : NetworkBehaviour
{
    public Camera PlayerCamera;

    void Start()
    {
        Debug.Log(Application.loadedLevelName);
        Debug.Log("start spawn");
        if (isLocalPlayer)
        {
            Debug.Log(Application.loadedLevelName);
            Debug.Log("local start spawn");
            //GetComponent<MoveBall>().enabled = true;

        }
    }

    public override void OnStartClient() {
        // base.OnStartLocalPlayer();
        Debug.Log("Spawn script running");
        //PlayerCamera.enabled = true;
        //Debug.Log(PlayerCamera.gameObject.name);
        if (isLocalPlayer) {
            Debug.Log("is local client");
            PlayerCamera.enabled = true;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
