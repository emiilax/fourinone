using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayer : NetworkBehaviour
{
    //public GameObject laserPrefab;
    //public LineRenderer laserLineRenderer;
	//public Camera playerCamera;
    
    // Use this for initialization
    void Start()
    {
		Camera playerCamera = gameObject.GetComponent<Camera>();
		//Camera playerCamera = gameObject.GetComponentInChildren<Camera>();

        if (isLocalPlayer)
        {
			playerCamera.enabled = true;
			playerCamera.transform.position = gameObject.transform.position;
            //Camera.main.enabled = false;
            Debug.Log(Application.loadedLevelName);
            Debug.Log("created player");
            Debug.Log(playerCamera.transform.position);
            //Destroy(playerCamera);
            //GetComponent<MoveBall>().enabled = true;
        }
        else
        {
          //  Destroy(playerCamera); Why would we do this ? AAAAAAAAAAAAGGH
			playerCamera.enabled = false;
            Debug.Log("spawning other player");
        }
    }
   /* void Update()
    {
        if (!isLocalPlayer) { return; }

        if (Input.GetButtonDown("Fire1"))
        {
            CmdShootLaser();
        }
    }
	
    [Command]
    void CmdShootLaser()
    {
        var laserTip = (GameObject)Instantiate(
            laserPrefab); //Todo add more here?

        NetworkServer.Spawn(laserPrefab);

    }
    */
}
