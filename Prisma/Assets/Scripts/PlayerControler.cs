﻿using UnityEngine;
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

		laser.enabled = false;
		enabled = false;
	}
	Vector3 mousePosition;
	void Update () {
		if(Input.GetAxis("Fire1") != 0 && canFire)
		{
			nextFireTime = Time.time + fireRate;

			//mousePosition = Input.mousePosition; //playerCamera.ScreenToWorldPoint(Input.mousePosition.x - transform.po);
			Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
			mousePosition = new Vector3 (ray.origin.x, ray.origin.y, -10);

			Debug.Log ("test ray: " + ray);

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
		
		laser.enabled = true;


		Ray2D ray = new Ray2D(transform.position, mousePosition);

		Debug.Log ("mouse position " + mousePosition);
		Debug.Log ("transform.position " + transform.position );
		Debug.Log ("ray: + " + ray.ToString());
		laser.numPositions = 2;
		laser.SetPosition(0, ray.origin);
		laser.SetPosition(1, ray.GetPoint(100));

		yield return new WaitForSeconds(0.1f);
				
		laser.enabled = false;


	}




	public bool canFire {
		get {return Time.time >= nextFireTime;}

	}
}
