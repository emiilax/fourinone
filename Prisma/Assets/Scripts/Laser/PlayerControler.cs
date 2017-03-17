using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {

	[SerializeField] float shutDownDelay = 0.3f;

	LineRenderer laser;
	float ShutOffTimer;
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



	void Start(){}


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

	// Flips the sprite depending in the screen
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

	private void CalculateOffsetAngle(){
		RectTransform rt = (RectTransform)gameObject.transform;

		Vector3 lowercorner = Vector3.zero;

		float x;
		float y;
	
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

		//Vector3 diff = Vector3.right - direction;
		float add = (assignedScreen == 3 || assignedScreen == 4) ? 180.0f : 0 ;


		offsetAngle = Vector2.Angle (Vector2.right, direction) + add;
		//oldDirection = direction;

		Debug.Log ("offsetangle: " + offsetAngle);

		Ray2D ray = new Ray2D (rt.rect.center, direction);
	}


	public override void OnStartLocalPlayer ()
	{
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
				Debug.Log (camDist - dist);
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

		Debug.Log (gameObject.transform.position + "playerposition");
		Debug.Log (laser.transform.position +  "laserposition");
		laser.SetPosition(0, laser.transform.position);

		laser.enabled = false;
		enabled = false;
	}


	private void RotateSprite(Vector3 direction) {
	

		float angle = Vector2.Angle (Vector2.right, direction);
		Debug.Log ("Accual angle: " + angle);

		Vector3 diff = Vector3.right - direction;
		float sign = (diff.y < 0) ? -1.0f : 1.0f;

		Debug.Log ("Rotation: " + (offsetAngle-angle*sign));
		Debug.Log ("Up/Down: " + diff.y);

		transform.rotation = Quaternion.Euler (0, 0, offsetAngle-angle*sign );

	}

	void Update () {


		if (!isLocalPlayer)
			return;
		/*
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Clicked!");

			//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast(playerCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if(hit.collider != null){

				Debug.Log ("Raycast hit");

				gameObjectToDrag = hit.collider.gameObject;

				GOcenter = gameObjectToDrag.transform.position;

				touchPosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);

				offset = touchPosition - GOcenter;

				draggingMode = true;

			}
		}

		if (Input.GetMouseButton (0)) {

			if (draggingMode) {

				touchPosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);

				newGOcenter = touchPosition - offset;

				CmdMoveObject (gameObjectToDrag,newGOcenter);

				//gameObjectToDrag.transform.position = newGOcenter;

			}

		}

		if (Input.GetMouseButtonUp (0)) {
			draggingMode = false;
		} */




		if(Input.GetButton("Fire1"))
		{

			if (isLocalPlayer) {
				Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);
				Vector3 playerPosition = gameObject.transform.position;
				Vector3 direction = new Vector3 (mousePosition.x-playerPosition.x, mousePosition.y-playerPosition.y, 0);
				Ray2D ray = new Ray2D (playerPosition, direction);
				RaycastHit2D hit = Physics2D.Raycast (playerPosition, direction);

				RotateSprite (direction);


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
			

						}

					} else {
						laser.SetPosition (ptNum, ray.GetPoint (100));
						break;
					}
				}


				//gets all the points of the laser
				Vector3[] laserPositions = new Vector3[laser.numPositions];
				laser.GetPositions (laserPositions);



				CmdSynchLaser (gameObject, laserPositions, laser.numPositions);

			}



			ShutOffTimer = Time.time + shutDownDelay;
			Fire();
		}
		if (Time.time > ShutOffTimer) {
			SetLaserEnabled (false);
			CmdSetLaserEnabled (false);
		}


	}

	[Command]
	void CmdMoveObject(GameObject GO, Vector3 newpos){
		GO.transform.position = newpos;
	}
		

	void Fire()
	{
		SetLaserEnabled(true);
		CmdSetLaserEnabled(true);
	}

	[Command]
	void CmdSetLaserEnabled(bool b)
	{
		RpcSetLaserEnabled(b);
	}

	[ClientRpc]
	void RpcSetLaserEnabled(bool b)
	{
		if(isLocalPlayer) return;

		SetLaserEnabled(b);
	}

	void SetLaserEnabled(bool b)
	{

		laser.enabled = b;

	}


	[Command]
	void CmdSynchLaser(GameObject player, Vector3[] laserPos, int nrOfPos ){
		RpcSynchLaser (player, laserPos, nrOfPos);
	}


	[ClientRpc]
	void RpcSynchLaser(GameObject player, Vector3[] laserPos,int nrOfPos){
		if (isLocalPlayer) {
			return;
		}
		LineRenderer activeLaser = player.GetComponentInChildren<LineRenderer> ();
		activeLaser.numPositions = nrOfPos;
		activeLaser.SetPositions (laserPos);

	}

}
