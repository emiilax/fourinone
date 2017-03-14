using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PromptWindow : MonoBehaviour{

	public Image backgroundColor;
	public Image blur;



	void Start(){
		blur.color = new Color (0f, 0f, 0f, 0.7f);
	}


	public void SetColor(Color color){
		backgroundColor.color = color;
	}

}


