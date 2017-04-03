using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTracker  {
    protected bool draggingMode = false;
    protected GameObject gameObjectToDrag;
    public Vector3 newGOcenter;
    public Vector3 GOcenter;
    public Vector3 touchPosition; // Where on the object the mouse click
    private Vector3 offset;

    public TouchTracker() {
    }

    public void Begin(Vector3 pos)
    {

        if (draggingMode) { return; }
        //GUILog.Log("begin");
        FindTappedObject(pos);

        
    }


    public void Move(Vector3 pos, GameObject player)
    {
        if (!draggingMode)
        {
            FindTappedObject(pos);
            return;
        }
        gameObjectToDrag.GetComponent<AbstractTouchController>().Move(pos, player);
        //DrawDot();
    }


    public void End()
    {
        if (draggingMode)
        {
            //GUILog.Log("End");
            gameObjectToDrag.GetComponent<AbstractTouchController>().Release();
        }
       
    }

    public void FindTappedObject(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        //GUILog.Log("casting ray");
        if (hit.collider != null)
        {
            Debug.Log("Raycast hit");
            gameObjectToDrag = hit.collider.gameObject;
            if (gameObjectToDrag.tag.Equals("TouchController") || gameObjectToDrag.tag.Equals("Mirror"))
            {
                //GUILog.Log("tracker checking if selectable");
                if (gameObjectToDrag.GetComponent<AbstractTouchController>().Selectable())
                {
                    //GUILog.Log("Tracker found object");
                    gameObjectToDrag.GetComponent<AbstractTouchController>().Begin(pos);
                    draggingMode = true;
                }
            }
            
        }
            

        
    }

}
