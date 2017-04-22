using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayersLevels : MonoBehaviour {


	private int selGridInt = -1;
	public GameObject[] levels;
	public Texture2D[] levelImages;
	private string[] selStrings;
	private GUIContent[] contents;
	public GUIStyle style;
	private bool initialized = false;

	public int padding;
	public int lastId = -1;
	LevelSelectorController sel;


	void OnGUI() {
		if (!initialized) {
			style = new GUIStyle (GUI.skin.button);
			style.margin = new RectOffset (48,48,48,48);
			style.imagePosition = ImagePosition.ImageAbove;
			style.fontSize = 32;
		}
		selGridInt = GUI.SelectionGrid(new Rect(padding,padding,1024-padding*2,768-padding*2),selGridInt, contents, levels.Length/2, style);
		if (lastId != selGridInt) {
			sel.LevelSelected (selGridInt);
			lastId = selGridInt;

		}
	}


	void FindLevels(){
		sel = GameObject.Find("SelectorMenu").GetComponent<LevelSelectorController>();

		GameObject levels = GameObject.Find ("Levels");
		foreach(Transform trans in levels.transform)
		{
			GameObject level = trans.gameObject;
		}

	}

	// Use this for initialization
	void Start () {
		FindLevels ();
		contents = new GUIContent[levels.Length];
		style = new GUIStyle ("button");
		//style.margin = new RectOffset( Screen.width / 100, Screen.width / 100, 0,0);
		for (int i = 0; i < levels.Length; i++) {
			contents [i] = new GUIContent (levels[i].name, levelImages[i]);
			//selStrings [i] = levels [i].name;
		}
	}

	public void Reset(){
		lastId = -1;
		selGridInt = -1;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
