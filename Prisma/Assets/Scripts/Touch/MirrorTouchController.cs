using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MirrorTouchController :  AbstractTouchController
{


    public float distFromCenter;
    public GameObject ctrlLeft;
    public GameObject ctrlRight;
    protected bool beingDragged = false;
    protected bool abortFadeOut = false;
    protected IEnumerator fadeEnumerator;
    public float fadeOutTime = 1.5f;
	private int overlaps; //number of colliders this collider is currently overlapping
    public Sprite selectionSprite;

	private Vector3 lastNonOverlappingPosition;
	private Vector3 lastPosition;
    private TouchSelectionIndicator selectionIndicator;

    // Use this for initialization
     void Start() {
        selectionIndicator = new TouchSelectionIndicator(selectionSprite, gameObject);
		selectionIndicator.SetColor (Color.green);
		gameObject.GetComponent<LineRenderer> ().sortingLayerName = "Foreground";
		//gameObject.GetComponent<LineRenderer> ().sortingOrder = 2;
		fadeEnumerator = FadeOut();
        SetControllerAlpha(0.0f);
        ResetControllerPositions();
    }

	// Update is called once per frame
	void FixedUpdate () {
		if (overlaps == 0) {
			lastNonOverlappingPosition = lastPosition;
			lastPosition = gameObject.transform.position;
		}
    }

	void OnCollisionEnter2D(Collision2D col)
	{
		selectionIndicator.SetColor (Color.red);
		//lastNonOverlappingPosition = gameObject.transform.position;
		overlaps++;
		GUILog.Log("collided");	
	}
	void OnCollisionStay2D(Collision2D col)
	{
		//GUILog.Log("stay");	
	}
	void OnCollisionExit2D(Collision2D col)
	{
		overlaps--;
		if (overlaps == 0) {
			selectionIndicator.SetColor (Color.green);
		}
		GUILog.Log("exit");
	}


    public override bool Selectable()
    {
        return Draggable();
    }
    public void SetControllerAlpha(float a)
    {
        spriteColor.a = a;
        ctrlRight.GetComponent<SpriteRenderer>().color = spriteColor;
        ctrlLeft.GetComponent<SpriteRenderer>().color = spriteColor;
        gameObject.GetComponent<LineRenderer>().material.color = spriteColor;
    }

    public void ShowSideControllers()
    {
        //abortFadeOut = true;
        StopCoroutine(fadeEnumerator);
        fadeEnumerator = FadeOut();
        SetControllerAlpha(1.0f);
    }

    public void HideSideControllers()
    {
        StartCoroutine(fadeEnumerator);
    }

    public override void Begin(Vector3 touchpPosition)
    {

        if(!ctrlLeft.GetComponent<MirrorSideTouchController>().BeingDragged() && !ctrlRight.GetComponent<MirrorSideTouchController>().BeingDragged())
        {
            selectionIndicator.ShowSelected();
            ShowSideControllers();
            //GUILog.Log("Mirror begin");
            base.Begin(touchpPosition);
            beingDragged = true;
            ctrlLeft.GetComponent<MirrorSideTouchController>().BeginWithMirror(touchpPosition);
            ctrlRight.GetComponent<MirrorSideTouchController>().BeginWithMirror(touchpPosition);
        }

        //ctrlLeft.GetComponent<MirrorSideTouchController>();
    }

    public override void Move(Vector3 pos, GameObject player)
    {
        if (beingDragged)
        {
            player.GetComponent<AimShootingMultiTouch>().CmdMoveRotate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
			MoveGameObject (pos);
			MoveSideControlleraWithMirror(pos);
            UpdateLinePoints();
        }
    }

    public override void Release()
    {

        beingDragged = false;
		selectionIndicator.HideSelected();
		HideSideControllers();

		ctrlLeft.GetComponent<MirrorSideTouchController> ().ReleaseWithoutReset (); 
		ctrlRight.GetComponent<MirrorSideTouchController> ().ReleaseWithoutReset ();
		if (overlaps > 0) {
			ctrlLeft.GetComponent<MirrorSideTouchController>().BeginWithMirror(gameObject.transform.position);
			ctrlRight.GetComponent<MirrorSideTouchController>().BeginWithMirror(gameObject.transform.position);

			gameObject.transform.position = lastNonOverlappingPosition;

			//ResetControllerPositions ();
			MoveSideControlleraWithMirror (lastNonOverlappingPosition);
			UpdateLinePoints();
			ctrlLeft.GetComponent<MirrorSideTouchController>().ReleaseWithoutReset (); 
			ctrlRight.GetComponent<MirrorSideTouchController>().ReleaseWithoutReset ();

			//ctrlLeft.GetComponent<MirrorSideTouchController> ().Release (); 
			//ctrlRight.GetComponent<MirrorSideTouchController> ().Release ();
			//
		} else {


		}
    }

	public void MoveSideControlleraWithMirror(Vector3 pos){
		ctrlLeft.GetComponent<MirrorSideTouchController>().MoveWithMirror(pos); 
		ctrlRight.GetComponent<MirrorSideTouchController>().MoveWithMirror(pos);
	}

    public override bool Draggable()
    {
        return !beingDragged && !ctrlLeft.GetComponent<MirrorSideTouchController>().BeingDragged() && !ctrlRight.GetComponent<MirrorSideTouchController>().BeingDragged();
    }

    public override bool BeingDragged()
    {
        return beingDragged;
    }


    public void UpdatePosition(GameObject player)
    {
        if (!beingDragged)
        {
            transform.position = (ctrlLeft.transform.position + ctrlRight.transform.position) * 0.5f;
            //transform.LookAt(ctrlRight.transform);
            Quaternion rot = Quaternion.LookRotation(ctrlLeft.transform.position - ctrlRight.transform.position);
            transform.rotation = new Quaternion(0.0f, 0.0f, rot.z, rot.w);
            player.GetComponent<AimShootingMultiTouch>().CmdMoveRotate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
            //GUILog.Log("hasAuthority="+hasAuthority.ToString()+" isServer="+isServer.ToString()+"");
            //CmdTest();

            //CmdSyncTransform(gameObject, gameObject.transform.position, transform.rotation);
            //ServercSyncTransform(gameObject.transform.position, transform.rotation);
            UpdateLinePoints();
        }
    }

    public void UpdateLinePoints()
    {

        float radius = ctrlLeft.GetComponent<CircleCollider2D>().radius;

        Vector3 leftOffset = (ctrlLeft.transform.position - transform.position);
        leftOffset.Normalize();
        leftOffset = leftOffset * radius;

        gameObject.GetComponent<LineRenderer>().SetPosition(0, ctrlLeft.transform.position - leftOffset);
        gameObject.GetComponent<LineRenderer>().SetPosition(1, ctrlRight.transform.position + leftOffset);
    }

    public void ResetControllerPositions()
    {
        if(!ctrlLeft.GetComponent<MirrorSideTouchController>().BeingDragged() && !ctrlRight.GetComponent<MirrorSideTouchController>().BeingDragged())
        {
			GUILog.Log ("reset");
            Vector3 offset = ctrlLeft.transform.position - transform.position;
            ctrlLeft.transform.position += -offset * (offset.magnitude - distFromCenter) / offset.magnitude;

            offset = ctrlRight.transform.position - transform.position;
            ctrlRight.transform.position += -offset * (offset.magnitude - distFromCenter) / offset.magnitude;

            UpdateLinePoints();
        }
    }



    public IEnumerator FadeOut()
    {
        float fade = 1.0f;
        float startTime = Time.time;
        while (fade > 0.0f)
        {
            //if (abortFadeOut) { abortFadeOut = false;  break; }
            fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / fadeOutTime);
            //spriteColor.a = fade;
            //ctrlRight.GetComponent<SpriteRenderer>().color = spriteColor;
            SetControllerAlpha(fade);
            yield return null;
        }
    }

}
