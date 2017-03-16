using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {

	[SerializeField] float shutDownDelay = 0.3f;

	LineRenderer laser;
	float ShutOffTimer;
	Camera playerCamera;





	public GameObject gameObjectToDrag;

	public Vector3 GOcenter; // The game object center

	public Vector3 touchPosition; // Where on the object the mouse click

	public Vector3 offset; // vector between GOcenter and touchPosition

	public Vector3 newGOcenter;

	public bool draggingMode;

	RaycastHit hit;



	void Start(){}




	public override void OnStartLocalPlayer ()
	{
		enabled = true;


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
				//Debug.Log (cam.gameObject.transform.position + "CAMPOS");
				//Vector3 a = cam.gameObject.transform.position;
				//Vector3 b = gameObject.transform.position;
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
		Debug.Log (gameObject.transform.position + "playerposition");
		Debug.Log (laser.transform.position +  "laserposition");
		laser.SetPosition(0, laser.transform.position);

		laser.enabled = false;
		enabled = false;
	}

	void Update () {

		if (!isLocalPlayer)
			return;

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
		}



		/*
		if(Input.GetButton("Fire1"))
		{

			if (isLocalPlayer) {
				Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);
				Vector3 playerPosition = gameObject.transform.position;
				Vector3 direction = new Vector3 (mousePosition.x-playerPosition.x, mousePosition.y-playerPosition.y, 0);
				Ray2D ray = new Ray2D (playerPosition, direction);
				RaycastHit2D hit = Physics2D.Raycast (playerPosition, direction);
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
		*/

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
