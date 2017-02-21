using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayer : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {
        Camera playerCamera = this.GetComponent<Camera>();
        if (isLocalPlayer)
        {
            //Camera.main.enabled = false;
            Debug.Log(Application.loadedLevelName);
            Debug.Log("created player");
            Debug.Log(playerCamera.transform.position);
            //Destroy(playerCamera);
            //GetComponent<MoveBall>().enabled = true;
        }
        else
        {
            Destroy(playerCamera);
            Debug.Log("spawning other player");
        }
    }

}
