
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class AimShootingMultiTouch : NetworkBehaviour
{
    [SerializeField]
    float shutDownDelay = 0.3f;
    [SerializeField]
    float keyShutDownDelay = 0.1f;


    LayerMask controllerLayerMask = ~(1 << 11); // Masks out layer 11 (the controller layer), used for raycasting laser without hitting the controllers

	public LineRenderer laser;
    
	public bool isHit;

	public GameObject playerSprites;

	public Material[] laserMaterials = new Material [4];

    private Vector3 offset;

    private Vector3 oldDirection;
    private Vector3 laserAim; //location aimed at with the laser
    private float offsetAngle;


    float LaserShutOffTimer;
    float KeyShutOffTimer;
    Camera playerCamera;

    private float assignedScreen;


    public GameObject gameObjectToDrag;

    public Vector3 GOcenter; // The game object center

    public Vector3 touchPosition; // Where on the object the mouse click

    public Vector3 dragOffset; // vector between GOcenter and touchPosition

    public Vector3 newGOcenter;

    public bool draggingMode;

    private Dictionary<int, TouchTracker> touchMap; // For keeping track of TouchTrackers by there touchId

   

	void Start() { 

		touchMap = new Dictionary<int, TouchTracker>();/*DontDestroyOnLoad (gameObject)*/;
	}


    public override void OnStartLocalPlayer()
    {
        enabled = true;

        InitPlayerSprite();


		SetCamera();

		//Debug.Log ("Assigned screen: " + assignedScreen);
		laser.material = laserMaterials[(int) assignedScreen - 1];

		CmdChangeLaserMaterial(gameObject, (int)assignedScreen);

        //This aparantley is how you chose your active camera...
        playerCamera.enabled = false;
        playerCamera.enabled = true;

        
    }

	[Command]
	void CmdChangeLaserMaterial(GameObject player, int playerScreen){
		
		RpcChangeLaserMaterial (player, playerScreen);

	}

	[ClientRpc]
	void RpcChangeLaserMaterial(GameObject player, int playerscreen){
		if (isLocalPlayer)
			return;

		LineRenderer activeLaser = player.GetComponentInChildren<LineRenderer>();

		activeLaser.material = laserMaterials [playerscreen - 1];
	}


	// This sets playercamera as the camera closest to your spawnposition.
	public void SetCamera(){
		float dist;
		Vector3 offset;
		Camera[] cameras = new Camera[Camera.allCamerasCount];
		Camera.GetAllCameras(cameras);
		offset = cameras[0].gameObject.transform.position - gameObject.transform.position;
		dist = offset.sqrMagnitude;
		playerCamera = cameras[0];
		foreach (Camera cam in cameras)
		{
			if (cam != null)
			{
				offset = cam.gameObject.transform.position - gameObject.transform.position;
				float camDist = offset.sqrMagnitude;
				//Debug.Log("CAMDIST" + (camDist - dist));
				if (camDist < dist)
				{
					dist = camDist;
					playerCamera = cam;
				}
			}
		}

		//This aparantley is how you chose your active camera...
		//Debug.Log("Camera: " + playerCamera.ToString());
		playerCamera.enabled = false;
		playerCamera.enabled = true;

	}

	public void OnChangeLevel(){
		if (isLocalPlayer) {
			SetCamera ();
		}
	}

	public Material test;
    public override void OnStartClient()
    {
		

	

        laser.sortingLayerName = "Midground";


        laser.SetPosition(0, laser.transform.position);



        laser.enabled = false;
        //enabled = false; <-- Was there any purpose for this? 
    }



    // Init all player stuff
    private void InitPlayerSprite() {

        Vector3 GOpos = gameObject.transform.position;


        // Calculate what screen you are to be assiged
        if (GOpos.x < 0)
        {
            assignedScreen = (GOpos.y > 0) ? 1 : 3;
        }
        else
        {
            assignedScreen = (GOpos.y > 0) ? 2 : 4;
        }

        SetSprite();

        CalculateOffsetAngle();
    }




    // Flips the sprite depending in the assigned screen
    private void SetSprite()
    {
		
		GameObject[] playersprites = getFirstChildren (playerSprites).ToArray();

		foreach (GameObject ps in playersprites) {
			ps.SetActive (false);
		}

		playersprites [(int)assignedScreen-1].SetActive (true);

		SpriteRenderer sr = playersprites [(int)assignedScreen-1].GetComponent<SpriteRenderer>();

        if (assignedScreen == 2)
        {
            sr.flipX = true;
        }
        else if (assignedScreen == 3)
        {
            sr.flipY = true;
        }
        else if (assignedScreen == 4)
        {
            sr.flipX = true;
            sr.flipY = true;
        }
    }

    // Calculate offset-angle depending on assigned screen
    private void CalculateOffsetAngle()
    {

        RectTransform rt = (RectTransform)gameObject.transform;

        float x;
        float y;

        // To get the angle of the laser when pointing 45 degreese, you could get the 
        // centerposition and the corner off the sprite where the laser should shoot from

        if (assignedScreen == 1 || assignedScreen == 4){
            x = gameObject.transform.position.x + 1;
        }
        else{
            x = gameObject.transform.position.x - 1;
        }

		if (assignedScreen == 1 || assignedScreen == 2) {
            y = gameObject.transform.position.y - 1;
		}
        else {
            y = gameObject.transform.position.y + 1;
        }

        Vector3 GOpos = gameObject.transform.position;

        Vector3 direction = new Vector3(x - GOpos.x, y - GOpos.y, 0);


        // If on other side of x axis, add 180 degreese
        float add = (assignedScreen == 3 || assignedScreen == 4) ? 180.0f : 0;


        offsetAngle = Vector2.Angle(Vector2.right, direction) + add;


    }



    public void SetLaserEnabled(bool enable)
    {
		
		laser.enabled = enable;
        CmdSetLaserEnabled(enable);
    }

  

    public void EnableLaser()
    {
        laser.enabled = true;
    }
    public void DisableLaser()
    {
        laser.enabled = false;
    }

    public void SetLaserAim(Vector3 pos)
    {
        laserAim = pos;
    }

    public RaycastHit2D RayCastExcludingControllers(Vector3 pos, Vector3 direction)
    {
        
        return Physics2D.Raycast(pos, direction, Mathf.Infinity, controllerLayerMask.value);
    }
    // Method for firing the laser
    private void FireLaser(Vector3 mousePosition)
    {
        //Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPosition = gameObject.transform.position;
        Vector3 direction = new Vector3(mousePosition.x - playerPosition.x, mousePosition.y - playerPosition.y, 0);
        Ray2D ray = new Ray2D(playerPosition, direction);
        RaycastHit2D hit = RayCastExcludingControllers(playerPosition, direction);

        RotateSprite(direction);


        laser.numPositions = 2;
        int ptNum = 1;
        int maxBounces = 16;
        int bounceNum = 0;

        //Verkar som att reflektionen kan fastna på samma spegel igen
        //om man inte flyttar ut den lite från spegeln bör kanske 
        //lösa detta på nåt bättre sätt men verkar fungera iallafall
        float offsetReflection = 0.01f;
        bool continueBouncing = true;
        while (continueBouncing)
        {
            if (hit.collider)
            {
				if (hit.collider.tag.Equals ("Wall")) {

					laser.SetPosition (ptNum, hit.point);
					//isHit = true;
					break;
				} else if (hit.collider.tag.Equals ("Mirror")) {
					// Debug.Log(bounceNum);
					if (bounceNum == maxBounces) {
						laser.SetPosition (ptNum, hit.point);
						//isHit = true;
						break;
					}
					laser.SetPosition (ptNum, hit.point);
					//isHit = true;

					Vector3 origin = laser.GetPosition (ptNum - 1);
					Vector3 hitPoint = hit.point;
					Vector3 incoming = hitPoint - origin;
					Vector3 normal = new Vector3 (hit.normal.x, hit.normal.y, 0);
					Vector3 reflected = Vector3.Reflect (incoming, hit.normal);

					ray = new Ray2D (hitPoint, reflected);
					hit = RayCastExcludingControllers (hitPoint + offsetReflection * normal, reflected);
					ptNum++;
					laser.numPositions++;
					bounceNum++;
				} else if (hit.collider.tag.Equals ("Key")) {
					laser.SetPosition (ptNum, hit.point);
                    
					//StartCoroutine(KeyIsHit(hit.collider.gameObject.GetComponent<KeyScript>().door));
					CmdKeyIsHit (hit.collider.gameObject);
					break;
				} else {
					laser.SetPosition (ptNum, hit.point);
					//isHit = true;
					break;
				}


            }

            else
            {
                laser.SetPosition(ptNum, ray.GetPoint(100));
                break;
            }
        }


        //gets all the points of the laser
        Vector3[] laserPositions = new Vector3[laser.numPositions];
        laser.GetPositions(laserPositions);


		CmdChangeLaserMaterial(gameObject, (int)assignedScreen);
        CmdSynchLaser(gameObject, laserPositions, laser.numPositions);

    }


	IEnumerator KeyIsHit(GameObject key){
		GameObject door = key.GetComponent<KeyScript> ().door; 

		//Debug.Log (door.name + "KeyisHit");
		RpcSetObjectEnabled(door, false);
		//Debug.Log (GameController.instance.gameObject.name);
		GameController.instance.KeyIsHit (key, true);

		//is this creating stackoverflow?
		KeyShutOffTimer = Time.time + keyShutDownDelay;
		while(Time.time < KeyShutOffTimer)
		{
			yield return new WaitForSeconds(0.1f);
		}
			
		RpcSetObjectEnabled (door, true);

	}


    // Function used to rotate the player sprite depending on laser-angle
    private void RotateSprite(Vector3 direction)
    {

        float angle = Vector2.Angle(Vector2.right, direction);

        Vector3 diff = Vector3.right - direction;
        float sign = (diff.y < 0) ? -1.0f : 1.0f;


        transform.rotation = Quaternion.Euler(0, 0, offsetAngle - angle * sign);

    }


	private List<GameObject> getFirstChildren(GameObject gameObj){
		List<GameObject> list = new List<GameObject> ();
		//gets all components
		Transform[] allcomponents = gameObj.GetComponentsInChildren<Transform> (true);
		foreach (Transform tf in allcomponents) {
			if (tf.gameObject.Equals (gameObj)) { //top level object do nothing

			} else if (tf.parent.gameObject.Equals (gameObj)) {
				list.Add (tf.gameObject);
				Debug.Log (tf.gameObject.name);
			}
		}
		return list;
	}


	TouchPhase mousePhase = TouchPhase.Ended;
	private int mouseFingerId = 999; // Will work as long as there are less than 999 fingers on the iPad simultaneously
	Vector3 lastMousePosition;

    // Update is called once per frame
    void Update()
    {
		if (!isLocalPlayer) { return;  }
		if (playerCamera == null) {
			SetCamera ();
		}

		if (Input.GetButton ("Fire1") && Input.touchCount == 0) {
			
			
			Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);

			if (mousePhase == TouchPhase.Ended) {
				//GUILog.Log ("began");
				mousePhase = TouchPhase.Began;
				lastMousePosition = mousePosition;
			} else if (mousePhase == TouchPhase.Began) {

				if (lastMousePosition != mousePosition) {
					//GUILog.Log ("moved");
					mousePhase = TouchPhase.Moved;
					lastMousePosition = mousePosition;
				}

			}

			UpdateTouchMap (mousePosition, mousePhase, mouseFingerId);

		} else if (Input.GetMouseButtonUp(0) && mousePhase != TouchPhase.Ended){


			mousePhase = TouchPhase.Ended;

			UpdateTouchMap (Vector3.zero, mousePhase, mouseFingerId);

		}


        if (laser.enabled) {
			
            FireLaser(laserAim);

        }
        var fingerCount = Input.touchCount;
        for (int i = 0; i < fingerCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector3 position = playerCamera.ScreenToWorldPoint(touch.position);
			UpdateTouchMap (position, touch.phase, touch.fingerId);
		}

    }

	private void UpdateTouchMap(Vector3 position, TouchPhase phase, int fingerId){
		
		if (!touchMap.ContainsKey(fingerId))
		{
			touchMap[fingerId] = new TouchTracker();
		}
		switch (phase)
		{
		// Record initial touch position.
		case TouchPhase.Began:
			touchMap[fingerId].Begin(position);
			break;

			// Determine direction by comparing the current touch position with the initial one.
		case TouchPhase.Moved:
			touchMap[fingerId].Move(position, gameObject);
			break;

			// Report that a direction has been chosen when the finger is lifted.
		case TouchPhase.Ended:
			touchMap[fingerId].End();
			touchMap.Remove(fingerId);
			break;
		}
		//Do whatever you want with the current touch.
		
	}



    /* ---- Command calls -----*/
    [Command]
    public void CmdMoveRotate(GameObject GO, Vector3 pos, Quaternion rotation)
    {
        RpcMoveRotate(GO, pos, rotation);
    }


    // Tells Server to enable or disable laser on clients
    [Command]
    void CmdSetLaserEnabled(bool b)
    {
        RpcSetLaserEnabled(b);
    }

    // Tells Server to enable or disable gameObjects
    [Command]
    void CmdSetObjectEnabled(GameObject o, bool b)
    {
        RpcSetObjectEnabled(o, b);
    }
    // Tells Server to move a object
    [Command]
    void CmdMoveObject(GameObject GO, Vector3 newpos)
    {
        ////GUILog.Log("sent RPC move");
        RpcMoveObject(GO, newpos);
    }
		
	[Command]
	void CmdKeyIsHit(GameObject key){

		StartCoroutine (KeyIsHit (key));

	}



    // Tells Server to sync laser on clients 
    [Command]
    void CmdSynchLaser(GameObject player, Vector3[] laserPos, int nrOfPos)
    {
        RpcSynchLaser(player, laserPos, nrOfPos);
    }


    /* ---- ClientRPC Calls -----*/

    [ClientRpc]
    void RpcMoveObject(GameObject GO, Vector3 newpos)
    {
        ////GUILog.Log("recieved RPC move");
        if (isLocalPlayer)
            return;
       
        GO.transform.position = newpos;
    }

    [ClientRpc]
    void RpcSetObjectEnabled(GameObject go, bool b)
    {
		if (go == null) {
			Debug.Log ("GO IS NULL WHAT IS EVEN HAPPENING");
			return;
		}
        go.SetActive(b);

    }


    [ClientRpc]
    void RpcSetLaserEnabled(bool b)
    {
        if (isLocalPlayer) return;

        laser.enabled = b;
    }


    [ClientRpc]
    void RpcSynchLaser(GameObject player, Vector3[] laserPos, int nrOfPos)
    {
        if (isLocalPlayer)
        {
            return;
        }

		//GameObject playersLasers = player.GetComponent<AimShootingMultiTouch> ().playerLasers;


        LineRenderer activeLaser = player.GetComponentInChildren<LineRenderer>();

		
        activeLaser.numPositions = nrOfPos;
        activeLaser.SetPositions(laserPos);

    }

	private LineRenderer GetActiveLaser(GameObject playerlasers){

		foreach(GameObject child in getFirstChildren(playerlasers)){
			if (child.activeInHierarchy) {
				return child.GetComponent<LineRenderer> ();
			}
		}

		return null;
	}


	[ClientRpc]
	void RpcMoveRotate(GameObject GO, Vector3 pos, Quaternion rotation)
	{
		if (isLocalPlayer) return;
		GO.transform.position = pos;
		GO.transform.rotation = rotation;
	}


	void OnApplicationQuit(){
		// Must make NetworkLobbyManager active to make it destroy match
		MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.QuitGame ();

	}

}




