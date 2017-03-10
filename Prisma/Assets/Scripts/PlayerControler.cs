using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {
	[SerializeField] float fireRate = 0.5f;
	LineRenderer laser;
	float nextFireTime;

	//Camera playerCamera;


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


	}

	public override void OnStartClient ()
	{
		//laser = transform.Find("LaserTip").GetComponent<LineRenderer>();
		laser = gameObject.GetComponentInChildren<LineRenderer>();
		Debug.Log (gameObject.transform.position + "playerposition");
		Debug.Log (laser.transform.position +  "laserposition");

		laser.enabled = false;
		enabled = false;
	}

	void Update () {
		if(Input.GetAxis("Fire1") != 0 && canFire)
		{
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
		Debug.Log ("COMMAND");
		RpcShowLaser();
	}

	[ClientRpc]
	void RpcShowLaser()
	{
		if(isLocalPlayer) return;
		Debug.Log ("ClientRPC");
		Debug.Log ("Laser Pos:" + laser.GetPosition(0));
		StartCoroutine(ShowLaser());
	}

	IEnumerator ShowLaser()
	{
		//Debug.Log ("ShowLaser");
		//laser.enabled = true;

		//yield return new WaitForSeconds(0.1f);
		//laser.enabled = false;


		laser.enabled = true;


		//Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
		//transform.forward = playerCam.ScreenToWorldPoint(Input.mousePosition);
		//Ray2D ray = new Ray2D(transform.position, mousePosition);

		//laser.numPositions = 2;


		//laser.SetPosition(0, ray.origin);

		yield return new WaitForSeconds(0.1f);
				
		laser.enabled = false;


	}




	public bool canFire {
		get {return Time.time >= nextFireTime;}

	}
}
