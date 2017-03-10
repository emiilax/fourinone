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
		

		//Camera playerCamera = gameObject.GetComponentInChildren<Camera>();
		//Camera playerCamera = gameObject.GetComponent<Camera>();
		Debug.Log (isLocalPlayer);
        if (isLocalPlayer) 
        {
			
			//playerCamera.enabled = true;
			//playerCamera.transform.position = gameObject.transform.position;
			//Debug.Log("Start");

            //Debug.Log("Start LocalPlayer");
            //Debug.Log(playerCamera.transform.position);
            
        }
        else
        {
          //  Destroy(playerCamera); Why would we do this ? AAAAAAAAAAAAGGH
			//playerCamera.enabled = false;
            //Debug.Log("spawning other player");
        }

    }

	public override void OnStartLocalPlayer ()
	{
		Camera playerCamera = gameObject.GetComponent<Camera>();
		//playerCam.gameObject.SetActive(true);
		//gameObject.transform.position.Set (gameObject.transform.position.x, gameObject.transform.position.y, -10);

		Debug.Log ("OnStartLocalPlayer");

	}

	public override void OnStartClient ()
	{
		Debug.Log ("OnStartClient");


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
