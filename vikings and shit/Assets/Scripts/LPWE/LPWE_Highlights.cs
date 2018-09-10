using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPWE_Highlights : LPWE_Component 
{

	public List<SelectedVertices> verts;
	public bool vp = false;

	public void Draw(List<SelectedVertices> v)
	{
		verts = v;
	}

	
		
	void OnDrawGizmos() 
	{


		if (verts == null) 
		{
			return;
		}
		
		//Debug.Log("Draw");
		if(!vp)
		{
			foreach (SelectedVertices sv in verts) 
			{
				foreach(int index in sv.vertices_index) 
				{
					Gizmos.color = Color.magenta;
					Gizmos.DrawCube(main.chunks [sv.chunk_index].mesh_filter.sharedMesh.vertices[index] + main.chunks [sv.chunk_index].plane.transform.position, new Vector3(0.1f,0.1f,0.1f));
				}
			}
		}
		else
		{
			foreach (SelectedVertices sv in verts) 
			{ 
				foreach(int index in sv.vertices_index) 
				{
					Gizmos.color = Color.magenta;
					Gizmos.DrawCube(main.chunks [sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.vertices[index] + main.chunks [sv.chunk_index].visual.transform.position, new Vector3(0.1f,0.1f,0.1f));
				}
			}
		}
	}
}
