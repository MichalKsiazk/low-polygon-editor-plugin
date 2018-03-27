using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallCreator))]
public class WallEditor : Editor {

	bool editMode = false;

	Vector3 startPoint;
	Vector3 endPoint;
	float distance;
	float loopedDistance;

	int curves = 0;

	GameObject parentObject;
	List <GameObject> tray = new List<GameObject>();
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
	}
	
	void OnSceneGUI() {

		bool isPressedInLoop = false;

		WallCreator wallCreator = (WallCreator)target;
		Event guiEvent = Event.current;


		//start drag

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && !editMode) {
			StartNewDrag ();
		}

		//cancel drag

		if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.X && editMode) {
			CancelCurrentDrag ();
		}

		//end drag

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && editMode) {
			EndCurrentDrag ();
		}
			
		if (editMode) {



			Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {
				endPoint = hit.point;

				if (!isPressedInLoop && guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
					curves++;
					startPoint = Vector3.MoveTowards (startPoint, endPoint, loopedDistance);
					foreach (GameObject obj in tray) {
						GameObject notTemp = Instantiate (obj);
						notTemp.transform.parent = parentObject.transform;
					}
				}


				distance = Vector3.Distance (startPoint, endPoint);
				Debug.Log (distance);
				foreach (GameObject obj in tray) {
					DestroyImmediate (obj);
				}

				tray = new List<GameObject> ();	



				if (wallCreator.wallWidth != null || wallCreator.wallWidth != 0) {
					//if(curves == 0)
						DrawFirstWall (wallCreator.wallWidth, wallCreator.prefab, wallCreator.distorted);
				}
			}
		}
		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
	}


	void StartNewDrag(){
		curves = 0;
		editMode = true;
		Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			startPoint = hit.point;
			parentObject = new GameObject ();
		}
	}

	void EndCurrentDrag(){
		foreach (GameObject obj in tray) {
			GameObject notTemp = Instantiate (obj);
			notTemp.transform.parent = parentObject.transform;
		}
		foreach (GameObject obj in tray) {
			DestroyImmediate (obj);
		}
		editMode = false;
		parentObject = null;
		tray = new List<GameObject>();	
	}

	void CancelCurrentDrag() {
		editMode = false;
		DestroyImmediate (parentObject);
		parentObject = null;
		foreach (GameObject obj in tray) {
			DestroyImmediate (obj);
		}
		tray = new List<GameObject>();
	}

	void DrawFirstWall(float wallWidth, GameObject prefab, bool distorted) {
		Vector3 fixedStartPoint = startPoint;

		fixedStartPoint = Vector3.MoveTowards (startPoint, endPoint, wallWidth / 1.5f);
		float count = distance / wallWidth;
		count = Mathf.Floor (count);
		for (int i = 0; i < (int)count; i++) {

			Vector3 pos = Vector3.MoveTowards (fixedStartPoint, endPoint, (float)i * wallWidth);
			tray.Add (Instantiate (prefab, pos, Quaternion.identity));
			tray [tray.Count - 1].transform.LookAt (Vector3.MoveTowards (fixedStartPoint, endPoint, distance));
			tray [tray.Count - 1].transform.Rotate (new Vector3 (0, 90, 0));
			if (distorted) {
				tray [tray.Count - 1].transform.Translate (Vector3.down * Random.Range (0.0f, 1.0f));
			}
		
		}
		loopedDistance = count * wallWidth;
	}

	void InstantiateSegment(){
	}
}
