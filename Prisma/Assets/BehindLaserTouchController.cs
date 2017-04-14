using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindLaserTouchController : AbstractTouchController {

	public GameObject player;
	// Update is called once per frame
	public Sprite selectionSprite;
	private TouchSelectionIndicator selectionIndicator;
	GameObject touch;
	void Start()
	{
		//touch = new GameObject("touch dummy object");
		//selectionIndicator = new TouchSelectionIndicator(selectionSprite, touch);
	}

	void Update () {

	}

	public override void Begin(Vector3 touchPosition)
	{
		if (touch == null) {
			touch = new GameObject("touch dummy object");
			selectionIndicator = new TouchSelectionIndicator(selectionSprite, touch);
		}
		SetTouchDisplayLocation(touchPosition);
		selectionIndicator.ShowSelected ();
		player.GetComponent<AimShootingMultiTouch>().SetLaserAim(TransformTouch(touchPosition));//.SendMessage("FireLaser", touchPosition, SendMessageOptions.DontRequireReceiver);
		player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(true);
	}

	private Vector3 TransformTouch(Vector3 pos){
		return  player.transform.position - (pos - player.transform.position);
	}

	public override void Move(Vector3 pos, GameObject player)
	{
		
		SetTouchDisplayLocation(pos);
		RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
		if (hit.collider == player.GetComponentInChildren<CircleCollider2D>())
		{
			player.GetComponent<AimShootingMultiTouch>().SetLaserAim(TransformTouch(pos));
			player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(true);
			selectionIndicator.ShowSelected ();

		}
		else
		{
			player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(false);
			selectionIndicator.HideSelected ();
		}
		//gameObject.GetComponent<AimShootingMultiTouch>().FireLaser(pos);
	}
	public override bool Selectable()
	{
		return true;
	}
	public override void Release()
	{
		selectionIndicator.HideSelected ();
		player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(false);
	}
	public override bool Draggable() { return false; }

	public override bool BeingDragged() { return false; }

	public void SetTouchDisplayLocation(Vector3 pos){
		touch.transform.position = new Vector3 (pos.x, pos.y, 0);
	}
}
