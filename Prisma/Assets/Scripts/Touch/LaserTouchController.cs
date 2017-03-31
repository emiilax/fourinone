using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTouchController : AbstractTouchController {

    public GameObject player;
    // Update is called once per frame

    void Start()
    {

    }

    void Update () {
		
	}

    public override void Begin(Vector3 touchPosition)
    {
        //laser.FireLaser(touchPosition);
        //base.Begin(touchPosition);
        
        player.GetComponent<AimShootingMultiTouch>().SetLaserAim(touchPosition);//.SendMessage("FireLaser", touchPosition, SendMessageOptions.DontRequireReceiver);
        player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(true);
        GUILog.Log("got aim shooting component");

    }

    public override void Move(Vector3 pos, GameObject player)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider == player.GetComponentInChildren<CircleCollider2D>())
        {
            player.GetComponent<AimShootingMultiTouch>().SetLaserAim(pos);
            player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(true);
        }
        else
        {
            player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(false);
        }
        //gameObject.GetComponent<AimShootingMultiTouch>().FireLaser(pos);
    }
    public override bool Selectable()
    {
        return true;
    }
    public override void Release()
    {
        player.GetComponent<AimShootingMultiTouch>().SetLaserEnabled(false);
    }
    public override bool Draggable() { return false; }

    public override bool BeingDragged() { return false; }

}
