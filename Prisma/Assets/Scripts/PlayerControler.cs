using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {
	[SerializeField] float fireRate = 0.5f;
	LineRenderer laser;
	float nextFireTime;
	Camera playerCamera;


	void Start(){

		/*if (isLocalPlayer) {
			Debug.Log ("Start");
			playerCamera = gameObject.GetComponent<Camera> ();
			playerCamera.enabled = true;
			playerCamera.transform.position = gameObject.transform.position;

		} else {
			playerCamera.enabled = false;
		}*/
	}



	public override void OnStartLocalPlayer ()
	{
		enabled = true;
		playerCamera = gameObject.GetComponent<Camera>();
		playerCamera.enabled = true;



	}

	public override void OnStartClient ()
	{
		//laser = transform.Find("LaserTip").GetComponent<LineRenderer>();
		laser = gameObject.GetComponentInChildren<LineRenderer>();
		Debug.Log (gameObject.transform.position + "playerposition");
		Debug.Log (laser.transform.position +  "laserposition");
		laser.SetPosition(0, laser.transform.position);

		laser.enabled = false;
		enabled = false;
	}
	void Update () {
		if(Input.GetAxis("Fire1") != 0 && canFire)
		{

			if (isLocalPlayer) {
				Vector3 mousePosition = playerCamera.ScreenToWorldPoint (Input.mousePosition);
				Ray2D ray = new Ray2D (laser.transform.position, mousePosition);
				RaycastHit2D hit = Physics2D.Raycast (laser.transform.position, mousePosition);
				laser.numPositions = 2;
				int ptNum = 1;
				int maxBounces = 16;
				int bounceNum = 0;
				laser.SetPosition (ptNum, ray.GetPoint (100));

				Vector3[] laserPositions = new Vector3[laser.numPositions];
				laser.GetPositions (laserPositions);

				foreach (Vector3 vector in laserPositions) {
					Debug.Log("laserpositions" + vector.ToString ());
				}

				//Copy Old postion to the new LineRenderer
				//newLine.GetComponent<LineRenderer>().SetPositions(newPos);
				CmdSynchLaser (gameObject, laserPositions);
			}



			nextFireTime = Time.time + fireRate;
			Fire();
		}
	}

	void Fire()
	{
		StartCoroutine(ShowLaser());
		CmdShowLaser();
	}

	[Command]
	void CmdShowLaser()
	{
		//Debug.Log ("COMMAND");
		RpcShowLaser();
	}

	[ClientRpc]
	void RpcShowLaser()
	{
		if(isLocalPlayer) return;
		//Debug.Log ("ClientRPC");
		StartCoroutine(ShowLaser());
	}

	IEnumerator ShowLaser()
	{
		
		laser.enabled = true;
	
		yield return new WaitForSeconds(0.1f);
				
		laser.enabled = false;

	}
	[Command]
	void CmdSynchLaser(GameObject player, Vector3[] laserPos){
		Debug.Log ("synch command");
		RpcSynchLaser (player, laserPos);
	}

	[ClientRpc]
	void RpcSynchLaser(GameObject player, Vector3[] laserPos){
		if (isLocalPlayer) {
			return;
		}
		player.GetComponentInChildren<LineRenderer> ().SetPositions (laserPos);

	}
		


	public bool canFire {
		get {return Time.time >= nextFireTime;}

	}
}
