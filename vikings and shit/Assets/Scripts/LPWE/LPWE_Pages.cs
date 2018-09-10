using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LPWE_Pages {

	static GUILayoutOption[] object_field_params = {GUILayout.Width(200)};
	static GUILayoutOption[] slider_params = {GUILayout.Width(300)};

	public static void TerrainCreatorPage(ref LPWE main, DialogInfo sensitive_buttons)
	{

		GUILayout.Space (10);

		GUILayout.BeginHorizontal();
		GUILayout.Label ("WIDTH:");
		main.terrain_creator.width = EditorGUILayout.IntSlider(main.terrain_creator.width, 1, 200, slider_params);
		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		GUILayout.Label ("LENGHT:");
		main.terrain_creator.lenght = EditorGUILayout.IntSlider(main.terrain_creator.lenght, 1, 200, slider_params);
		GUILayout.EndHorizontal();

		GUILayout.Space (10);


		GUILayout.BeginHorizontal();
		GUILayout.Label ("TERRAIN MATERIAL:");
		main.terrain_creator.default_material = (Material)EditorGUILayout.ObjectField(main.terrain_creator.default_material, typeof(Material), true, object_field_params);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label ("MAP:");
		main.map = (GameObject)EditorGUILayout.ObjectField(main.map, typeof(GameObject), true, object_field_params);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("TERRAIN:");
		main.terrain = (GameObject)EditorGUILayout.ObjectField(main.terrain, typeof(GameObject), true, object_field_params);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("ENTITIES:");
		main.entities = (GameObject)EditorGUILayout.ObjectField(main.entities, typeof(GameObject), true, object_field_params);
		GUILayout.EndHorizontal();

		GUILayout.Space(30);
		GUILayout.BeginHorizontal();
		if(!sensitive_buttons.are_you_sure)
		{
			if (GUILayout.Button ("Reset Terrain")) 
			{
				sensitive_buttons.dialog_option = "reset";
				sensitive_buttons.are_you_sure = true;
			}
			if (GUILayout.Button ("Destroy Terrain")) 
			{
				sensitive_buttons.dialog_option = "destroy";
				sensitive_buttons.are_you_sure = true;
			}
		}
		else
		{
			GUILayout.BeginVertical();
			GUILayout.Label("Are you sure?");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button ("Yes"))
			{
				if(sensitive_buttons.dialog_option == "reset") main.terrain_creator.ResetTerrain();
				else if(sensitive_buttons.dialog_option == "destroy") main.terrain_creator.DestroyTerrain();
				sensitive_buttons.are_you_sure = false;
			}
			if(GUILayout.Button ("No"))
			{
				sensitive_buttons.dialog_option = "";
				sensitive_buttons.are_you_sure = false;
			}
			GUILayout.Space(30);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		GUILayout.EndHorizontal();
	}

	public static void MeshEditorPage(ref LPWE main)
	{
		GUILayout.Space (10);
		GUILayout.BeginHorizontal();
		GUILayout.Label ("SIZE:");
		main.selectors.radius = EditorGUILayout.Slider(main.selectors.radius, 0.0f, 10f, slider_params);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("MODULO:");
		main.modulo = EditorGUILayout.IntSlider(main.modulo, 1, 100, slider_params);
		GUILayout.EndHorizontal();

		GUILayout.Space (10);
		main.selectors.shape = (SelectionShape)EditorGUILayout.EnumPopup("Shape", main.selectors.shape);
		GUILayout.Space (5);
		main.event_type = (DragType)EditorGUILayout.EnumPopup("EventType: ", main.event_type);
		main.selection = (Selection)EditorGUILayout.EnumPopup("Selection: ", main.selection);
		main.hit_point = (HitPoint)EditorGUILayout.EnumPopup("HitPoint: ", main.hit_point);

		GUILayout.Space (10);

		main.mesh_editor.current_used_tool = (MeshTools)EditorGUILayout.EnumPopup("Tool: ", main.mesh_editor.current_used_tool);
		switch(main.mesh_editor.current_used_tool)
		{
			case MeshTools.Raise:
				GUILayout.BeginHorizontal();
				GUILayout.Label ("FORCE:");
				main.mesh_editor.raise_force = EditorGUILayout.Slider(main.mesh_editor.raise_force, -1.0f, 1.0f, slider_params);
				GUILayout.EndHorizontal();
				break;
			case MeshTools.Flat:
				GUILayout.BeginHorizontal();
				GUILayout.Label ("FORCE:");
				main.mesh_editor.flat_force = EditorGUILayout.Slider(main.mesh_editor.flat_force, 0.0f, 1.0f, slider_params);
				GUILayout.EndHorizontal();
				break;
			case MeshTools.FlatRaise:
				GUILayout.BeginHorizontal();
				GUILayout.Label ("FORCE:");
				main.mesh_editor.fr_force = EditorGUILayout.Slider(main.mesh_editor.fr_force, -1.0f, 1.0f, slider_params);
				GUILayout.EndHorizontal();
				break;
			case MeshTools.Randomize:
				GUILayout.BeginHorizontal();
				GUILayout.Label ("RANDOM:");
				main.mesh_editor.random_range = EditorGUILayout.Slider(main.mesh_editor.random_range, 0.0f, 0.2f, slider_params);
				GUILayout.EndHorizontal();
				break;
			case MeshTools.SetHeight:
				GUILayout.BeginHorizontal();
				GUILayout.Label ("HEIGHT:");
				main.mesh_editor.s_height = EditorGUILayout.Slider(main.mesh_editor.s_height, 0.0f, 30.0f, slider_params);
				GUILayout.EndHorizontal();
				break;
		}
	}

	public static void WorldPainterPage(ref LPWE main)
	{
		GUILayout.Space (10);
		GUILayout.BeginHorizontal();
		GUILayout.Label ("RADIUS:");
		main.selectors.radius = EditorGUILayout.Slider(main.selectors.radius, 0.0f, 10f, slider_params);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("MODULO:");
		main.modulo = EditorGUILayout.IntSlider(main.modulo, 1, 100, slider_params);
		GUILayout.EndHorizontal();

		GUILayout.Space (10);

		main.world_painter.tools = (PaintTools)EditorGUILayout.EnumPopup("TOOL: ", main.world_painter.tools);

		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
		GUILayout.Label ("COLOR:");
		main.world_painter.temp_color = EditorGUILayout.ColorField(main.world_painter.temp_color, null);
		GUILayout.EndHorizontal();
	}


}




public class DialogInfo
{
	public string dialog_option;
	public bool are_you_sure;

	public DialogInfo()
	{
		dialog_option = "";
		are_you_sure = false;
	}

	public DialogInfo(string dialog_option, bool are_you_sure)
	{
		this.dialog_option = dialog_option;
		this.are_you_sure = are_you_sure;
	}
}
