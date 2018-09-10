using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LPWE))]
public class LPWE_GUI : Editor {

	int page_index = 0;
	string[] page_names = new string[] {"Map Creator", "Mesh Editor", "World Painter", "Object Brush"};
	GameObject lpwe_components;
	Vector3 start_hit_point;
	bool pressed = false;
	DialogInfo sensitive_buttons = new DialogInfo();
	int i = 0;
	int st = 0;
	bool search_on_visual = false;

	public override void OnInspectorGUI()
	{
		LPWE main = (LPWE)target;
		page_index = GUILayout.Toolbar(page_index, page_names);
		SelectPage(main);
		base.OnInspectorGUI();
	} 


	void OnSceneGUI() 
	{

		bool first_frame = false;
		LPWE main = (LPWE)target;
		Vector3 hit_point = new Vector3 ();

		i++; st++;


		search_on_visual = page_index == 2;
		bool highlight = !search_on_visual || (search_on_visual && st == 4);

		if(st == 10)
		{
			st = 0; 
		}

		if (main.chunks == null) 
		{
			main.terrain_creator.LoadChunkMap ();
		}

		List<SelectedVertices> selected_vertices = new List<SelectedVertices>();
		Event guiEvent = Event.current;

		HandleMouseInput(guiEvent, ref main, ref first_frame);
		hit_point = NewHitPoint(ref main);

		//Debug.Log(hit_point);
		if(highlight)
		{
			selected_vertices = main.selectors.FindInRange (hit_point, main.selectors.radius, main.hit_point, search_on_visual);
		}
		main.highlights.vp = search_on_visual;
		if(highlight)
		{
			main.highlights.Draw(selected_vertices);
		}


		Action(first_frame, guiEvent, ref main, hit_point, ref selected_vertices);

		SceneView.RepaintAll();
		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
	}

	void HandleMouseInput(Event guiEvent, ref LPWE main, ref bool first_frame)
	{
		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) 
		{
			start_hit_point = main.selectors.ColliderRaycastDetection ();
			pressed = true;
			first_frame = true;
		}
		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0) 
		{
			pressed = false;
		}
	}

	Vector3 NewHitPoint(ref LPWE main)
	{
		switch (main.hit_point) 
		{
			case HitPoint.HitPoint3D:
				return main.selectors.ColliderRaycastDetection ();
			case HitPoint.HitPoint2D:
				return main.selectors.ColliderRaycastDetection ();
			case HitPoint.SeaLevel2D: 
				return main.selectors.AxisRaycastDetection();
		}
		return new Vector3(0,0,0);
	}

	void Action(bool first_frame, Event guiEvent, ref LPWE main, Vector3 hit_point, ref List<SelectedVertices> selected_vertices)
	{
		if (pressed && guiEvent.button == 0) 
		{
			/*  MeshEditor   - 1
			 *  WorldPainter - 2
			 *	ObjectBrush  - 3
			 */ 


			if((first_frame && main.event_type == DragType.OnClick) || (main.event_type == DragType.Drag && i % main.modulo == 0))
			{	
				i = 0;			
				switch(page_index)
				{
					case 1:
						MeshEditorAction(ref main, ref selected_vertices, hit_point);
					 	break;
					case 2:
						PainterAction(ref main, ref selected_vertices, hit_point);
						break;
				}
			}
		}
	}

	void MeshEditorAction(ref LPWE main, ref List<SelectedVertices> selected_vertices, Vector3 hit_point)
	{
		switch(main.mesh_editor.current_used_tool)
		{
			case MeshTools.SetHeight:
				foreach (SelectedVertices sv in selected_vertices) 
				{
					main.mesh_editor.SetHeight (sv, main.mesh_editor.s_height);
				}
				break;
			case MeshTools.Raise:
				foreach (SelectedVertices sv in selected_vertices) 
				{
					main.mesh_editor.Raise (sv, main.mesh_editor.raise_force);
				}
				break;
			case MeshTools.FlatRaise:
				Extremes extremes = main.mesh_editor.fr_force >= 0 ? Extremes.Minimum : Extremes.Maximum;
				main.mesh_editor.FlatRaise (selected_vertices, main.mesh_editor.fr_force , extremes, 0.1f);
				break;
			case MeshTools.Flat:
				if (main.mesh_editor.flat_options == FlatOptions.ToHitPoint) 
				{
					foreach (SelectedVertices sv in selected_vertices) 
					{
						main.mesh_editor.Flat (sv, new Vector3 (hit_point.x, start_hit_point.y, hit_point.z), main.mesh_editor.flat_force);
					}
				} 
				else if(main.mesh_editor.flat_options == FlatOptions.ToMedianPoint) 
				{
					float average = main.utils.Average (selected_vertices);
					foreach (SelectedVertices sv in selected_vertices) 
					{
						main.mesh_editor.FlatToAverage (sv, average, main.mesh_editor.flat_force);
					}
				}
				break;
			case MeshTools.Randomize:
				float random = main.mesh_editor.raise_force;
				Vector3 random_vector = new Vector3(Random.Range(-random,random),0, Random.Range(-random,random));
				foreach (SelectedVertices sv in selected_vertices) 
				{
					main.mesh_editor.Randomize (sv, random_vector);
				}
			 	break;
		}
		foreach (SelectedVertices sv in selected_vertices) 
		{
			main.utils.UpdateMesh(sv);
		}
	}

	void PainterAction(ref LPWE main, ref List<SelectedVertices> selected_vertices, Vector3 hit_point)
	{
		switch(main.world_painter.tools)
		{
			case PaintTools.Paint:
				foreach (SelectedVertices sv in selected_vertices) 
				{
					main.world_painter.Paint(sv, main.world_painter.temp_color);
				}
				break;
		}
	}



	void SelectPage(LPWE main)
	{
		switch(page_index)
		{
			case 0:
				LPWE_Pages.TerrainCreatorPage(ref main, sensitive_buttons);
				break;
			case 1:
				LPWE_Pages.MeshEditorPage(ref main);
				break;
			case 2:
				LPWE_Pages.WorldPainterPage(ref main);
				break;

		}
	}



	void SetComponents(LPWE main)
	{
		main.mesh_editor = lpwe_components.GetComponent<LPWE_MeshEditor>();
		main.world_painter = lpwe_components.GetComponent<LPWE_WorldPainter>();
		main.terrain_creator = lpwe_components.GetComponent<LPWE_TerrainCreator>();
		main.utils = lpwe_components.GetComponent<LPWE_Utils>();
		main.highlights = lpwe_components.GetComponent<LPWE_Highlights>();
		main.selectors = lpwe_components.GetComponent<LPWE_Selectors>();

		main.mesh_editor.SetLPWE(main);
		main.world_painter.SetLPWE(main);
		main.terrain_creator.SetLPWE(main);
		main.utils.SetLPWE(main);
		main.highlights.SetLPWE(main);
		main.selectors.SetLPWE(main);
	}


	void Awake() 
	{
		LPWE main = (LPWE)target;
		if(lpwe_components == null)
		{
			FindProps(main);
		}		
		SetComponents(main);
		if (main.chunks == null) 
		{
			main.terrain_creator.LoadChunkMap ();
		}
		SetComponents(main);
	}


	void FindProps(LPWE main)
	{
		Transform[] childs = main.gameObject.GetComponentsInChildren<Transform>();
		if(childs.Length <= 1)
		{
			lpwe_components = new GameObject();
			lpwe_components.transform.parent = main.gameObject.transform;	
			lpwe_components.name = "lpwe_props";
		}
		else
		{
			foreach(Transform t in childs)
			{
				if(t.gameObject.name == "lpwe_props")
				{
					lpwe_components = t.gameObject;
				}
			}
		}
	}
}
