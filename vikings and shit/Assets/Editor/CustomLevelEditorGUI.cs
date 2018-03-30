using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomLevelEditor))]
public class CustomLevelEditorGUI : Editor 
{

	bool pressed = false;
	Vector3 start_hit_point;

	bool indicate_vertices;

	int i = 0;

	public override void OnInspectorGUI() 
	{
		CustomLevelEditor level_editor = (CustomLevelEditor)target;
		base.OnInspectorGUI();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Indicate:");
		indicate_vertices = EditorGUILayout.Toggle (indicate_vertices);
		GUILayout.EndHorizontal();

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		if (level_editor.tool == Tools.Raise) 
		{
			GUILayout.Space (10);
			GUILayout.Label ("RAISE TOOL");
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("delta height:");
			level_editor.delta_height = EditorGUILayout.Slider(level_editor.delta_height, -1.0f, 1.0f);
			//level_editor.delta_height = GUILayout.TextField
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("radius:");
			level_editor.radius = EditorGUILayout.Slider(level_editor.radius, 0.0f, 10.0f);
			GUILayout.EndHorizontal ();
		}
		else if(level_editor.tool == Tools.Flat)
		{
			GUILayout.Space (10);
			GUILayout.Label ("FLAT TOOL");
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("delta height:");
			level_editor.flat_strenght = EditorGUILayout.Slider(level_editor.flat_strenght, 0.0f, 1.0f);
			//level_editor.delta_height = GUILayout.TextField
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("radius:");
			level_editor.radius = EditorGUILayout.Slider(level_editor.radius, 0.0f, 10.0f);
			GUILayout.EndHorizontal ();
		}
		else if(level_editor.tool == Tools.SetHeight)
		{
			GUILayout.Space (10);
			GUILayout.Label ("SET HEIGHT TOOL");
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("height:");
			level_editor.height = EditorGUILayout.Slider(level_editor.height, -20.0f, 20.0f);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("radius:");
			level_editor.radius = EditorGUILayout.Slider(level_editor.radius, 0.0f, 10.0f);
			GUILayout.EndHorizontal ();
		}




		if (GUILayout.Button ("Reset Terrain")) 
		{
			level_editor.ResetTerrain ();
		}
	}


	void OnSceneGUI() 
	{
		bool first_frame = false;
		Vector3 hit_point = new Vector3 ();
		CustomLevelEditor level_editor = (CustomLevelEditor)target;

		if (level_editor.chunks == null) 
		{
			level_editor.LoadChunkMap ();
		}

		Event guiEvent = Event.current;
		List<SelectedVertices> selected_vertices;

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) 
		{
			start_hit_point = ColliderRaycastDetection ();
			pressed = true;
			first_frame = true;
			selected_vertices = new List<SelectedVertices>();
		}
		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0) 
		{
			pressed = false;
		}

		selected_vertices = new List<SelectedVertices>();

		switch (level_editor.selection) 
		{
		case Selection.HitPoint3D:
			hit_point = ColliderRaycastDetection ();
			break;
		case Selection.HitPoint2D:
			hit_point = ColliderRaycastDetection ();
			break;
		case Selection.SeaLevel2D:
			hit_point = AxisRaycastDetection();
			break;
		}
		//hit_point -= new Vector3 (5, 0, 5);
		selected_vertices = level_editor.FindInRange (hit_point, level_editor.radius, level_editor.selection);
		level_editor.highlight.GetComponent<CustomHighlights> ().Draw (selected_vertices, level_editor.chunks);


		if (pressed && guiEvent.button == 0) 
		{

			if((first_frame && level_editor.event_type == DragType.OnClick) || level_editor.event_type == DragType.Drag)
			{				
				switch(level_editor.tool)
				{
				case Tools.SetHeight:
					foreach (SelectedVertices sv in selected_vertices) 
					{
						level_editor.SetHeight (sv, level_editor.height);
					}
					break;
				case Tools.Raise:
					foreach (SelectedVertices sv in selected_vertices) 
					{
						level_editor.Raise (sv, level_editor.delta_height);
					}
					break;
				case Tools.Flat:
					if (level_editor.flat_options == FlatOptions.ToHitPoint) 
					{
						foreach (SelectedVertices sv in selected_vertices) 
						{
							level_editor.Flat (sv, new Vector3 (hit_point.x, start_hit_point.y, hit_point.z));
						}
					} 
					else if(level_editor.flat_options == FlatOptions.ToMedianPoint) 
					{
						float average = level_editor.Average (selected_vertices);
						foreach (SelectedVertices sv in selected_vertices) 
						{
							level_editor.FlatToAverage (sv, average);
						}
					}
					break;
				}
				foreach (SelectedVertices sv in selected_vertices) 
				{
					level_editor.UpdateTexture(level_editor.chunks[sv.chunk_index]);
				}
			}
		}
		SceneView.RepaintAll();
		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
	}


	Vector3 ColliderRaycastDetection() 
	{
		Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		RaycastHit hit;
 
		if (Physics.Raycast (ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"))) 
		{
			return hit.point;
		}
		return Vector3.zero;
	}

	Vector3 AxisRaycastDetection() 
	{
		CustomLevelEditor level_editor = (CustomLevelEditor)target;
		Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000, 1 << LayerMask.NameToLayer("Water"))) 
		{
			return hit.point;
		}
		return Vector3.zero;
	}


	void Awake() 
	{
		CustomLevelEditor level_editor = (CustomLevelEditor)target;
		if (level_editor.chunks == null) 
		{
			level_editor.LoadChunkMap ();
		}
	}

}