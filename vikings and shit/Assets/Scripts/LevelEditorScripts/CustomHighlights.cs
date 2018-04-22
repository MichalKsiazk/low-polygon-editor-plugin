using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHighlights : MonoBehaviour 
{

	public GameObject parent_object;
	public GameObject terrain_editor;

	public List<SelectedVertices> verts;
	

	public void Draw(List<SelectedVertices> sv, Chunk[] chunk_map) 
	{
		verts = sv;
	}
		
	void OnDrawGizmos() 
	{

		Chunk[] chunks = terrain_editor.GetComponent<CustomLevelEditor> ().chunks;

		if (verts == null) 
		{
			return;
		}
		
		foreach (SelectedVertices sv in verts) 
		{
			Chunk chunk = chunks [sv.chunk_index]; 

			foreach(int index in sv.vertices_index) 
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(chunk.mesh_filter.sharedMesh.vertices[index] + chunk.plane.transform.position, new Vector3(0.1f,0.1f,0.1f));
			}
		}
	}
}
