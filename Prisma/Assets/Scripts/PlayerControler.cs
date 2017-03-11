using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {
	[SerializeField] float shutDownDelay = 0.3f;
	LineRenderer laser;
	float ShutOffTimer;
	Camera playerCamera;


	void Start(){
	}



	public override void OnStartLocalPlayer ()
	{
		enabled = true;

		float dist;
		Vector3 offset;
		Camera[] cameras = new Camera[Camera.allCamerasCount];
		Camera.GetAllCameras (cameras);
		offset = cameras [0].gameObject.transform.position - gameObject.transform.position;
		dist = offset.sqrMagnitude;
		playerCamera = cameras [0];
		foreach (Camera cam in cameras) {
			if (cam != null) {
				Debug.Log (cam.gameObject.transform.position + "CAMPOS");
				Vector3 a = cam.gameObject.transform.position;
				Vector3 b = gameObject.transform.position;
				offset = a - b;
				float camDist = offset.sqrMagnitude;
				Debug.Log (camDist - dist);
				if (camDist < dist) {
					dist = camDist;
					playerCamera = cam;
				}	
			}
		}
		//playerCamera = gameObject.GetComponent<Camera>();
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
		if(Input.GetButton("Fire1"))
		{

			if (isLocalPlayer) {
				Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);
				Vector3 playerPosition = gameObject.transform.position;
				Vector3 direction = new Vector3 (mousePosition.x-playerPosition.x, mousePosition.y-playerPosition.y, 0);
				//Vector3 origin = new Vector3 (playerPosition.x, playerPosition.y, 0);
				Ray2D ray = new Ray2D (playerPosition, direction);
				RaycastHit2D hit = Physics2D.Raycast (gameObject.transform.position, mousePosition);
				laser.numPositions = 2;
				int ptNum = 1;
				int maxBounces = 16;
				int bounceNum = 0;
				Vector3 rayposition = new Vector3 (ray.GetPoint (100).x, ray.GetPoint (100).y, 0);
				laser.SetPosition (ptNum, rayposition);

				Vector3[] laserPositions = new Vector3[laser.numPositions];
				laser.GetPositions (laserPositions);

				foreach (Vector3 vector in laserPositions) {
					//Debug.Log("laserpositions" + vector.ToString ());
				}

				CmdSynchLaser (gameObject, laserPositions);
			}



			ShutOffTimer = Time.time + shutDownDelay;
			Fire();
		}
		if (Time.time > ShutOffTimer) {
			SetLaserEnabled (false);
			CmdSetLaserEnabled (false);
		}
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
	void CmdSynchLaser(GameObject player, Vector3[] laserPos){
		//Debug.Log ("synch command");
		RpcSynchLaser (player, laserPos);
	}

	[ClientRpc]
	void RpcSynchLaser(GameObject player, Vector3[] laserPos){
		if (isLocalPlayer) {
			return;
		}
		LineRenderer activeLaser = player.GetComponentInChildren<LineRenderer> ();
		activeLaser.SetPositions (laserPos);

	}

}
