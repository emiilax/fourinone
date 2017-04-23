﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class FixPrefabs : MonoBehaviour {

	// Use this for initialization

	//[MenuItem("CONTEXT/GameObject/Fix Prefabs")]
	[MenuItem("Custom Functions/Relink Level Prefabs")]
	static void Prefabs()
	{
		string[] prefabPaths = {"Prefabs/", "Prefabs/Laser/",  "Prefabs/Laser/Blocks/"};
		string[] prefabs = {"Test"};
		Transform[] sel = Selection.transforms;

		foreach (Transform topLevel in sel) {
			
			Transform[] transforms = topLevel.gameObject.GetComponentsInChildren<Transform>();
			foreach(Transform t in transforms){
				//most likely deleted because the parent was replaced by the prefab already
				if (t == null) {
					continue;
				}
				Debug.Log ("t="+t.ToString());
				Regex regex = new Regex(@"[A-z\-]+");
				Match match = regex.Match(t.name);

				GameObject prefab = GameObject.Find ("prefab_instances_for_script/" + match);
				Debug.Log (prefab);
				if (match.Success  && prefab != null)
				{
					
					Debug.Log ("Success!");

					Debug.Log (t.gameObject);
					//if (PrefabUtility.GetPrefabParent (t.gameObject.transform.parent) != null) {
					//	continue;
					//}
					GameObject newObject = PrefabUtility.InstantiatePrefab(PrefabUtility.GetPrefabParent(prefab)) as GameObject;
					Debug.Log (newObject);
					PrefabUtility.SetPropertyModifications(newObject, PrefabUtility.GetPropertyModifications(t.gameObject));
					newObject.transform.parent = t.gameObject.transform.parent;
					newObject.transform.position = t.gameObject.transform.position;
					newObject.transform.localPosition = t.gameObject.transform.localPosition;
					newObject.transform.rotation = t.gameObject.transform.rotation;
					newObject.transform.localRotation = t.gameObject.transform.localRotation;
					newObject.transform.localScale = t.gameObject.transform.localScale;

					newObject.name = t.gameObject.name;
					Undo.RegisterCreatedObjectUndo(newObject, t.gameObject.name + " replaced");
					Undo.DestroyObjectImmediate(t.gameObject);

					//PrefabUtility.ConnectGameObjectToPrefab (t.gameObject, prefab);

					/*
					Debug.Log (match.Value);
					bool isFirstMatch = true;
					bool doubleMatch = false;
					List<string> paths = new List<string>();

					foreach (string path in prefabPaths){
						Debug.Log (path + match.Value);
						//GameObject p = Resources.Load (path + match.Value, typeof(GameObject)) as GameObject;
						try{
							GameObject instance = Instantiate(AssetDatabase.LoadAssetAtPath ("Assets/" + path + match.Value + ".prefab", typeof(GameObject))) as GameObject;
							if(isFirstMatch){
								paths.Add("Assets/" + path + match.Value + ".prefab");
								instance.name = "_tmp".ToString();
								Debug.Log ("Success!");
								isFirstMatch = false;
							}
							else{
								paths.Add("Assets/" + path + match.Value + ".prefab");
								doubleMatch = true;

							}

						}catch(System.Exception e){
							Debug.Log ("was null");
						}
						Debug.Log ("After");
						//if (p != null){

						//	Debug.Log (p);
							//PrefabUtility.ConnectGameObjectToPrefab (t.gameObject, p);
						//}
					}
					if(doubleMatch){
						foreach (string s in paths) {
							Debug.Log (s);
						}
						throw new System.ArgumentException("Parameter produced double match", match.Value);
						 
					}
					PrefabUtility.ConnectGameObjectToPrefab (t.gameObject, GameObject.Find("_tmp"));
					//DestroyImmediate (GameObject.Find("_tmp"));
					*/
				}


			}

			//Debug.Log (trans.name);
		}
		Debug.Log("Doing Something...");
	}
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
