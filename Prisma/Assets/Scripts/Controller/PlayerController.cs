using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	[SerializeField] float shutDownDelay = 0.3f;
	[SerializeField] float keyShutDownDelay = 0.1f;

	LineRenderer laser;
	float LaserShutOffTimer;
	float KeyShutOffTimer;
	Camera playerCamera;

	private float assignedScreen;

	private float offsetAngle;

	public GameObject gameObjectToDrag;

	public Vector3 GOcenter; // The game object center

	public Vector3 touchPosition; // Where on the object the mouse click

	public Vector3 offset; // vector between GOcenter and touchPosition

	public Vector3 newGOcenter;

	public bool draggingMode;

	RaycastHit hit;



	void Start(){
	//	Debug.Log ("Startposition: " + gameObject.transform.position);
	}

	public override void OnStartLocalPlayer ()
	{

		//NetworkStartPosition[] spawnPoints = FindObjectsOfType<NetworkStartPosition>();

		//Debug.Log ("Startpos: " + spawnPoints.Length);
		enabled = true;

		initPlayer ();


		//Debug.Log ("Assigned Screen: " + assignedScreen);

		// This sets playercamera as the camera closest to your spawnposition.
		float dist;
		Vector3 offset;
		Camera[] cameras = new Camera[Camera.allCamerasCount];
		Camera.GetAllCameras (cameras);
		offset = cameras [0].gameObject.transform.position - gameObject.transform.position;
		dist = offset.sqrMagnitude;
		playerCamera = cameras [0];
		foreach (Camera cam in cameras) {
			if (cam != null) {
				offset = cam.gameObject.transform.position - gameObject.transform.position;
				float camDist = offset.sqrMagnitude;
			//	Debug.Log (camDist - dist);
				if (camDist < dist) {
					dist = camDist;
					playerCamera = cam;
				}	
			}
		}

		//This aparantley is how you chose your active camera...
		playerCamera.enabled = false;
		playerCamera.enabled = true;

	}


	public override void OnStartClient ()
	{
		laser = gameObject.GetComponentInChildren<LineRenderer>();
		laser.sortingLayerName = "Midground";

		//Debug.Log (gameObject.transform.position + "playerposition");
		//Debug.Log (laser.transform.position +  "laserposition");
		laser.SetPosition(0, laser.transform.position);

		laser.enabled = false;
		enabled = false;
	}

	// Init all player stuff
	private void initPlayer(){

		Vector3 GOpos = gameObject.transform.position;


		// Calculate what screen you are to be assiged
		if (GOpos.x < 0) {
			assignedScreen = (GOpos.y > 0) ? 1 : 3;
		} else {
			assignedScreen = (GOpos.y > 0) ? 2 : 4;
		}

		FlipSprite ();

		CalculateOffsetAngle ();



	}

	// Flips the sprite depending in the assigned screen
	private void FlipSprite() {

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer> ();

		if (assignedScreen == 2) {
			sr.flipX = true;
		} else if (assignedScreen == 3) {
			sr.flipY = true;
		} else if (assignedScreen == 4) {
			sr.flipX = true;
			sr.flipY = true;
		}
	}

	// Calculate offset-angle depending on assigned screen
	private void CalculateOffsetAngle(){

		RectTransform rt = (RectTransform)gameObject.transform;

		float x;
		float y;

		// To get the angle of the laser when pointing 45 degreese, you could get the 
		// centerposition and the corner off the sprite where the laser should shoot from

		if (assignedScreen == 1 || assignedScreen == 4) {
			x = gameObject.transform.position.x + (rt.rect.width / 2);
		} else {
			x = gameObject.transform.position.x - (rt.rect.width / 2);
		}

		if (assignedScreen == 1 || assignedScreen == 2) {
			y = gameObject.transform.position.y - (rt.rect.height / 2);

		} else {
			y = gameObject.transform.position.y + (rt.rect.height / 2);
		}

		Vector3 GOpos = gameObject.transform.position;

		Vector3 direction = new Vector3 (x - GOpos.x, y - GOpos.y, 0);


		// If on other side of x axis, add 180 degreese
		float add = (assignedScreen == 3 || assignedScreen == 4) ? 180.0f : 0 ;


		offsetAngle = Vector2.Angle (Vector2.right, direction) + add;


	}




	void Update () {


		if (!isLocalPlayer)
			return;
		

		if (Input.GetButton ("Fire1")) {

			Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 playerPosition = gameObject.transform.position;
			Vector3 direction = new Vector3 (mousePosition.x - playerPosition.x, mousePosition.y - playerPosition.y, 0);

			if (direction.magnitude < 5) {
				FireLaser ();
				LaserShutOffTimer = Time.time + shutDownDelay;
				ShowLaser();

			} else {
				
				MoveObject ();

			}



		} else {
			draggingMode = false;
		}

		//TODO make more efficient
		if (Time.time > LaserShutOffTimer) {
			SetLaserEnabled (false);
			CmdSetLaserEnabled (false);
		}


	}

	// Method for firing the laser
	private void FireLaser(){
	

		Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);
		Vector3 playerPosition = gameObject.transform.position;
		Vector3 direction = new Vector3 (mousePosition.x-playerPosition.x, mousePosition.y-playerPosition.y, 0);



		CmdFireLaser (mousePosition, playerPosition, gameObject);


		/*Ray2D ray = new Ray2D (playerPosition, direction);
		RaycastHit2D hit = Physics2D.Raycast (playerPosition, direction);



		//EmilMultiplayerController.instance.HandleHit();

		laser.numPositions = 2;
		int ptNum = 1;
		int maxBounces = 16;
		int bounceNum = 0;

		//Verkar som att reflektionen kan fastna på samma spegel igen
		//om man inte flyttar ut den lite från spegeln bör kanske 
		//lösa detta på nåt bättre sätt men verkar fungera iallafall
		float offsetReflection = 0.01f;
		bool continueBouncing = true;
		while (continueBouncing) {
			if (hit.collider) {
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
					hit = Physics2D.Raycast (hitPoint + offsetReflection * normal, reflected);
					ptNum++;
					laser.numPositions++;
					bounceNum++;
				} else if (hit.collider.tag.Equals ("Key")) {
					laser.SetPosition (ptNum, hit.point);
					KeyShutOffTimer = Time.time + keyShutDownDelay;
					//hit.collider.gameObject.GetComponent<KeyScript> ().door.SetActive (false);
					//CmdSetObjectEnabled(hit.collider.gameObject.GetComponent<KeyScript> ().door, false);
					StartCoroutine(KeyIsHit(hit.collider.gameObject.GetComponent<KeyScript> ().door));
					break;
				}

			} else {
				laser.SetPosition (ptNum, ray.GetPoint (100));
				break;
			}
		}


		//gets all the points of the laser
		Vector3[] laserPositions = new Vector3[laser.numPositions];
		laser.GetPositions (laserPositions);



		CmdSynchLaser (gameObject, laserPositions, laser.numPositions);*/

	}

	[Command]
	void CmdFireLaser(Vector3 mousePosition, Vector3 playerPosition, GameObject player){

		//Debug.Log ("Fire Laser!!");
		Vector3 direction = new Vector3 (mousePosition.x-playerPosition.x, mousePosition.y-playerPosition.y, 0);

		RpcRotateSprite (direction, player);
		Ray2D ray = new Ray2D (playerPosition, direction);
		RaycastHit2D hit = Physics2D.Raycast (playerPosition, direction);

		LineRenderer playerLaser = player.GetComponentInChildren<LineRenderer> ();
		
		//EmilMultiplayerController.instance.HandleHit();

		playerLaser.numPositions = 2;
		int ptNum = 1;
		int maxBounces = 16;
		int bounceNum = 0;

		//Verkar som att reflektionen kan fastna på samma spegel igen
		//om man inte flyttar ut den lite från spegeln bör kanske 
		//lösa detta på nåt bättre sätt men verkar fungera iallafall
		float offsetReflection = 0.01f;
		bool continueBouncing = true;
		while (continueBouncing) {
			if (hit.collider) {
				if (hit.collider.tag.Equals ("Wall")) {

					playerLaser.SetPosition (ptNum, hit.point);
					//isHit = true;
					break;
				} else if (hit.collider.tag.Equals ("Mirror")) {
					// Debug.Log(bounceNum);
					if (bounceNum == maxBounces) {
						playerLaser.SetPosition (ptNum, hit.point);
						//isHit = true;
						break;
					}
					playerLaser.SetPosition (ptNum, hit.point);
					//isHit = true;

					Vector3 origin = playerLaser.GetPosition (ptNum - 1);
					Vector3 hitPoint = hit.point;
					Vector3 incoming = hitPoint - origin;
					Vector3 normal = new Vector3 (hit.normal.x, hit.normal.y, 0);
					Vector3 reflected = Vector3.Reflect (incoming, hit.normal);

					ray = new Ray2D (hitPoint, reflected);
					hit = Physics2D.Raycast (hitPoint + offsetReflection * normal, reflected);
					ptNum++;
					playerLaser.numPositions++;
					bounceNum++;
				} else if (hit.collider.tag.Equals ("Key")) {
					playerLaser.SetPosition (ptNum, hit.point);
					KeyShutOffTimer = Time.time + keyShutDownDelay;
					//hit.collider.gameObject.GetComponent<KeyScript> ().door.SetActive (false);
					//CmdSetObjectEnabled(hit.collider.gameObject.GetComponent<KeyScript> ().door, false);
					StartCoroutine(KeyIsHit(hit.collider.gameObject));
					break;
				}

			} else {
				playerLaser.SetPosition (ptNum, ray.GetPoint (100));
				break;
			}
		}


		//gets all the points of the laser
		Vector3[] laserPositions = new Vector3[playerLaser.numPositions];
		playerLaser.GetPositions (laserPositions);



		RpcSynchLaser (gameObject, laserPositions, playerLaser.numPositions);

	}

	// Function used to rotate the player sprite depending on laser-angle
	[ClientRpc]
	private void RpcRotateSprite(Vector3 direction, GameObject player) {


		float angle = Vector2.Angle (Vector2.right, direction);
		//Debug.Log ("Accual angle: " + angle);

		Vector3 diff = Vector3.right - direction;
		float sign = (diff.y < 0) ? -1.0f : 1.0f;

		//Debug.Log ("Rotation: " + (offsetAngle-angle*sign));
		//Debug.Log ("Up/Down: " + diff.y);

		player.transform.rotation = Quaternion.Euler (0, 0, offsetAngle-angle*sign );

	}



	// Function for moving objects on the board
	private void MoveObject(){

		//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit2D hit = Physics2D.Raycast (playerCamera.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

		if (hit.collider != null) {
			if (hit.collider.tag.Equals ("Mirror")) {

				Debug.Log ("Raycast hit");

				gameObjectToDrag = hit.collider.gameObject;

				GOcenter = gameObjectToDrag.transform.position;

				touchPosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);

				offset = touchPosition - GOcenter;

				draggingMode = true;
			}
		}


		if (draggingMode) {

			touchPosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);

			newGOcenter = touchPosition - offset;

			gameObjectToDrag.transform.position = newGOcenter;

			CmdMoveObject (gameObjectToDrag,newGOcenter);

		}

	}



	void ShowLaser(){
		//SetLaserEnabled(true);
		CmdSetLaserEnabled(true);
	}



	void SetLaserEnabled(bool b){

		laser.enabled = b;

	}


	IEnumerator KeyIsHit(GameObject key){
		GameObject door = key.GetComponent<KeyScript> ().door;
		//door.SetActive (false);
		//CmdSetObjectEnabled (door, false);
		RpcSetObjectEnabled(door, false);
		//Debug.Log(EmilMultiplayerController.instance.KeyIsHit (key, true));

		//is this creating stackoverflow?
		while(Time.time < KeyShutOffTimer)
		{
			yield return new WaitForSeconds(0.1f);
		}
		//door.SetActive (true);
		//Debug.Log(EmilMultiplayerController.instance.KeyIsHit (key, false));
		//CmdSetObjectEnabled(door,true);
		RpcSetObjectEnabled (door, true);

	}



	/* ---- Command calls -----*/

	// Tells Server to enable or disable laser on clients
	[Command]
	void CmdSetLaserEnabled(bool b){
		RpcSetLaserEnabled(b);
	}

	// Tells Server to enable or disable gameObjects
	[Command]
	void CmdSetObjectEnabled(GameObject o, bool b){
		RpcSetObjectEnabled(o,b);
	}
	// Tells Server to move a object
	[Command]
	void CmdMoveObject(GameObject GO, Vector3 newpos){
		RpcMoveObject (GO, newpos);
	}


	// Tells Server to sync laser on clients 
	[Command]
	void CmdSynchLaser(GameObject player, Vector3[] laserPos, int nrOfPos ){
		RpcSynchLaser (player, laserPos, nrOfPos);

	}


	/* ---- ClientRPC Calls -----*/

	[ClientRpc]
	void RpcMoveObject(GameObject GO, Vector3 newpos){
		if (isLocalPlayer)
			return;


		GO.transform.position = newpos;
	}

	[ClientRpc]
	void RpcSetObjectEnabled(GameObject o, bool b){
		//if (isLocalPlayer) return;
		o.SetActive (b);
	}


	[ClientRpc]
	void RpcSetLaserEnabled(bool b) {
	//	if(isLocalPlayer) return;

		SetLaserEnabled(b);
	}
		

	[ClientRpc]
	void RpcSynchLaser(GameObject player, Vector3[] laserPos,int nrOfPos){
		//if (isLocalPlayer) {
	//		return;
//		}
		LineRenderer activeLaser = player.GetComponentInChildren<LineRenderer> ();
		activeLaser.numPositions = nrOfPos;
		activeLaser.SetPositions (laserPos);

	}


	void OnApplicationQuit(){
		// Must make NetworkLobbyManager active to make it destroy match
		MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.QuitGame ();

	}

}
	