using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {
	[SerializeField] float fireRate = 0.5f;
	LineRenderer laser;
	float nextFireTime;

	public override void OnStartLocalPlayer ()
	{
		enabled = true;
	}

	public override void OnStartClient ()
	{
		laser = transform.Find("LaserTip").GetComponent<LineRenderer>();
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
		Debug.Log ("ClientRPC");
		if(isLocalPlayer) return;
		StartCoroutine(ShowLaser());
	}

	IEnumerator ShowLaser()
	{
		Debug.Log ("ShowLaser");
		laser.enabled = true;

		yield return new WaitForSeconds(0.1f);
		laser.enabled = false;
	}

	public bool canFire {
		get {return Time.time >= nextFireTime;}
	}
}
