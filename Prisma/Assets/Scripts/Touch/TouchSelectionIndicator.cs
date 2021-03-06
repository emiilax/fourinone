﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSelectionIndicator  {
	
    private SpriteRenderer selectedRenderer;
    private GameObject selectedRendererObject;

    public TouchSelectionIndicator (Sprite selectedSprite, GameObject gameObject) {
       // Debug.Log("created renderer");
        selectedRendererObject = new GameObject("selected renderer");
        selectedRendererObject.transform.position = gameObject.transform.position;
        selectedRendererObject.transform.rotation = gameObject.transform.rotation;
        selectedRendererObject.transform.localScale = gameObject.transform.localScale;
        selectedRendererObject.transform.parent = gameObject.transform;
        selectedRenderer = selectedRendererObject.AddComponent<SpriteRenderer>();
        selectedRenderer.sortingLayerName = "Midground";
        selectedRenderer.sortingOrder = 2;
        selectedRenderer.sprite = selectedSprite;
        selectedRenderer.enabled = false;
    }

    public void ShowSelected()
    {
        selectedRenderer.enabled = true;
    }

	public void SetColor(Color color){
		selectedRenderer.color = color;
	}

    public void HideSelected()
    {
        selectedRenderer.enabled = false;
    }
}
