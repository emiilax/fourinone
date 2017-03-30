using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorSideTouchController :  ITouchController {


    public GameObject controlled; //Object being controlled by the controller

    // true if mirror object itself is being moved, the side controls cant be used at the same time.
    // Current behaviour is to drop the mirror and select the side controller.
    public bool movingWithMirror = false; 
    public bool beingDragged = false;
    private bool selectable = true;

    public Sprite selectionSprite;

    private TouchSelectionIndicator selectionIndicator;

    void Start()
    {
        selectionIndicator = new TouchSelectionIndicator(selectionSprite, gameObject);
    }
    // Update is called once per frame
    void Update () {
		
	}
    public override bool Selectable()
    {
        return true;
    }
    public override void Release()
    {
        selectionIndicator.HideSelected();
        beingDragged = false;
        //GUILog.Log("Release side controller");
        controlled.GetComponent<MirrorTouchController>().ResetControllerPositions();
        controlled.GetComponent<MirrorTouchController>().HideSideControllers();
        //controlled.GetComponent<MirrorTouchController>().UpdatePosition();
    }
    public void ReleaseWithoutReset()
    {
        movingWithMirror = false;
    }
    public override void Move(Vector3 pos, GameObject player)
    {
        MoveGameObject(pos);
        controlled.GetComponent<MirrorTouchController>().UpdatePosition(player);
    }
    public override void Begin(Vector3 offset)
    {
        if (movingWithMirror)
        {
            controlled.GetComponent<MirrorTouchController>().Release();
            movingWithMirror = false;
        }
        if (!beingDragged)
        {
            selectionIndicator.ShowSelected();
            controlled.GetComponent<MirrorTouchController>().ShowSideControllers();
            //GUILog.Log("side controller begin");
            base.Begin(offset);
            beingDragged = true;
        }
    }

    public override bool Draggable()
    {
        return !beingDragged;
    }

    public void BeginWithMirror(Vector3 offset)
    {
        if (!beingDragged)
        {
            //GUILog.Log("side controller begin");
            base.Begin(offset);
            movingWithMirror = true;
        }
    }
    public void MoveWithMirror(Vector3 pos)
    {
        MoveGameObject(pos);
    }

    public override bool BeingDragged()
    {
        return beingDragged;
    }

    //    public void MoveToCorrectPosition()
    //    {
    //        Vector3 offset = transform.position - controlled.transform.position;
    //        transform.position += -offset * (offset.magnitude - distFromCenter) / offset.magnitude;
    //    }
}
