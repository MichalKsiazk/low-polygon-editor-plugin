using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPWE_TerrainCreator : LPWE_Component
{


	[HideInInspector] public int width;
	[HideInInspector] public int lenght;
	[HideInInspector] public Material default_material;


	public void LoadChunkMap() 
	{		
		Transform[] childs = main.terrain.transform.GetComponentsInChildren<Transform> ();

		main.chunks = new Chunk[width * lenght];

		foreach (Transform t in childs) 
		{
			string name = t.gameObject.name;
			if (!name.Contains("chunk")) 
			{
				continue;
			}

			int index = -1;
			int.TryParse(name.Replace ("chunk: ", ""), out index);
			if(index != -1)
			{
				main.chunks [index] = new Chunk (t.gameObject, index, width);
			}
		}
		Mesh prefab_mesh = main.chunks [0].plane.GetComponent<MeshFilter>().sharedMesh;
		Mesh unique_mesh = main.utils.RetriangulateMesh(prefab_mesh);

		//main.linked_indexes = main.utils.LinkIndexes(prefab_mesh, unique_mesh);

	}


	public void DestroyTerrain() 
	{

		Transform[] childs = main.terrain.transform.GetComponentsInChildren<Transform> ();
		if(childs.Length <= 1)
		{
			return;
		}
		foreach(Transform t in childs) 
		{
			if(t == null)
			{
				continue;
			}
			if (t.gameObject.name == main.terrain.name) 
			{
				continue;
			}
			if	(t.gameObject.name.Contains("chunk"))
			{
				DestroyImmediate (t.gameObject);
			}
		}
	}


	public void ResetTerrain() {
		
		DestroyTerrain ();

		main.chunks = new Chunk[width * lenght];


		for (int z = 0; z < lenght; z++) 
		{
			for (int x = 0; x < width; x++) 
			{

				int index = z * width + x;

				main.chunks [index] = new Chunk (main.chunk_prefab, new Vector3 (5 + x * 10f, 0, 5 + z * 10f), x, z, z * width + x);
				main.chunks [index].plane.transform.parent = main.terrain.transform;
				main.chunks [index].plane.layer = LayerMask.NameToLayer ("Terrain");
				main.chunks [index].chunk_serial_index = index;
				main.chunks [index].plane.name = "chunk: " + index.ToString ();

				main.chunks [index].colors = main.world_painter.NewColorMap();
				main.chunks [index].plane.GetComponent<MeshFilter>().sharedMesh = main.utils.InvertTriangles(main.chunks [index].plane.GetComponent<MeshFilter>().sharedMesh);
				InitEmptyVisualMesh(main.chunks[index]);

				main.chunks[index].visual.GetComponent<MeshFilter>().sharedMesh = main.utils.RetriangulateMesh(main.chunks [index].plane.GetComponent<MeshFilter>().sharedMesh);
				int l = main.chunks[index].visual.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
				main.chunks[index].visual.GetComponent<MeshFilter>().sharedMesh.colors = main.world_painter.NewTriangleColorMap(main.chunks [index].colors, l);

			}
		}
		Mesh prefab_mesh = main.chunks [0].plane.GetComponent<MeshFilter>().sharedMesh;
		Mesh unique_mesh = main.utils.RetriangulateMesh(prefab_mesh);
		if(main.linked_indexes == null)
		{
			main.linked_indexes = main.utils.LinkIndexes(prefab_mesh, unique_mesh);
		}

		main.connected_vertices = main.utils.ConnectEdges();
	}

	void InitEmptyVisualMesh(Chunk chunk)
	{
		chunk.visual = new GameObject();
		chunk.visual.transform.parent = chunk.plane.transform;
		chunk.visual.transform.localPosition = new Vector3(0,0,0);
		chunk.visual.name = "visual";
		chunk.visual.AddComponent<MeshFilter>();
		chunk.visual.AddComponent<MeshRenderer>();
		chunk.visual.GetComponent<MeshRenderer>().sharedMaterial = default_material;
	}
}
