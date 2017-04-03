using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class AbstractTouchController : MonoBehaviour{

    public Color spriteColor = Color.white;


    private Vector3 offset;
    //private GameObject player;


    public virtual void Begin(Vector3 touchPosition) {
        Vector3 GOcenter = gameObject.transform.position;
        this.offset = touchPosition - GOcenter;
    }


    //public void SetPlayer(GameObject player) { this.player = player; }
    //public GameObject GetPlayer() { return player; }
    public abstract void Move(Vector3 pos, GameObject player);
    public abstract bool Selectable();
    public abstract void Release();
    public abstract bool Draggable();

    public abstract bool BeingDragged();

    protected void MoveGameObject(Vector3 pos)
    {
        Vector3 newGOcenter = pos - offset;
        gameObject.transform.position = newGOcenter;
    }

}
