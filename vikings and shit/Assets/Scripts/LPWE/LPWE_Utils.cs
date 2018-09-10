using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPWE_Utils : LPWE_Component 
{



	public void ApplyChanges(int chunk_index, ref Vector3[] vertices)
	{
		main.chunks [chunk_index].mesh_filter.sharedMesh.vertices = vertices;
		DestroyImmediate (main.chunks [chunk_index].plane.GetComponent<MeshCollider> ());
		main.chunks[chunk_index].plane.gameObject.AddComponent<MeshCollider>();
		main.chunks[chunk_index].plane.GetComponent<MeshCollider>().sharedMesh = main.chunks[chunk_index].mesh_filter.sharedMesh;
	}


	public LinkedIndex[] LinkIndexes(Mesh original, Mesh instantiated)
	{
		LinkedIndex[] li = new LinkedIndex[original.vertices.Length];

		for(int i = 0; i < original.vertices.Length; i++)
		{
			li[i].normal_index = i;
			int found = 0;
			li[i].unique_indexes = new List<int>();
			for(int a = 0; a < instantiated.vertices.Length; a++)
			{
				if(original.vertices[i] == instantiated.vertices[a])
				{
					li[i].unique_indexes.Add(a);
					found++;
					if(found > 8)
					{
						break;
					}
				}
			}
		}
		return li;
	}


	public float Average(List<SelectedVertices> sv) 
	{		
		float sum = 0.0f;
		float size = 0.0f;

		foreach (SelectedVertices s in sv) 
		{
			size += s.vertices_index.Count;
			for(int i = 0; i < s.vertices_index.Count; i++)
			{
				sum += main.chunks[s.chunk_index].mesh_filter.sharedMesh.vertices[s.vertices_index[i].serial_index].y;
			}
		}

		return sum / size;
	}


	public Mesh RetriangulateMesh(Mesh original_mesh) 
	{

		Mesh mesh = Instantiate<Mesh>(original_mesh);

		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		Vector3[] newVertices = new Vector3[mesh.triangles.Length];
		Vector2[] newUV = new Vector2[mesh.triangles.Length];
		Vector3[] newNormals = new Vector3[mesh.triangles.Length];
		int[] newTriangles = new int[mesh.triangles.Length];

		for (int i = 0; i < mesh.triangles.Length; i++) 
		{
			newVertices[i] = vertices[triangles[i]];
			newUV[i] = mesh.uv[triangles[i]];
			newNormals[i] = mesh.normals[mesh.triangles[i]];
			newTriangles[i] = i;
		}


		mesh.vertices = newVertices;
		mesh.normals = newNormals;
		mesh.triangles = newTriangles;	

		return mesh;
	}

	public List<ConnectedVertices>[] ConnectEdges()
	{
		int length = main.chunk_prefab.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
		List<ConnectedVertices>[] connected_edges = new List<ConnectedVertices>[length];
		for(int i = 0; i < length; i++)
		{
			connected_edges[i] = new List<ConnectedVertices>();
		}
		for(int i = 0; i <= 10; i++)
		{
			connected_edges[i].Add(new ConnectedVertices(-main.terrain_creator.width, i + 110));
		}
		for(int i = 0; i < length; i+=11)
		{
			connected_edges[i].Add(new ConnectedVertices(-1, i + 10));
		}
		for(int i = 10; i < length; i+=11)
		{
			connected_edges[i].Add(new ConnectedVertices(+1, i - 10));
		}
		for(int i = 110; i < length; i++)
		{
			connected_edges[i].Add(new ConnectedVertices(+main.terrain_creator.width, i - 110));
		}
		return connected_edges;
	}


	public Mesh InvertTriangles(Mesh mesh)
	{

		int size = 11;
		List<int> indices = new List<int>();

		bool t = true;
		for(int z = 0; z < size - 1; z++)
		{
            for(int x = 0; x < size - 1; x++)
			{
				if(t)
				{
					indices.Add(z * size + x);
					indices.Add((z + 1) * size + x);
					indices.Add(z * size + x + 1);
					
					indices.Add(z * size + x + 1);
					indices.Add((z + 1) * size + x);
					indices.Add((z + 1) * size + x + 1);
				}
				else
				{
					indices.Add(z * size + x);
					indices.Add((z + 1) * size + x + 1);
					indices.Add(z * size + x + 1);

					indices.Add(z * size + x);
					indices.Add((z + 1) * size + x);
					indices.Add((z + 1) * size + x + 1);					
				}
				t = !t;
            }
			t = !t;
        }
		mesh.triangles = indices.ToArray();
		return mesh;
	}

	public void UpdateMesh(SelectedVertices sv)
	{	
		Vector3[] plane_mesh = main.chunks[sv.chunk_index].plane.GetComponent<MeshFilter>().sharedMesh.vertices;
		Vector3[] visual_mesh = main.chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.vertices;

		foreach(SV i in sv.vertices_index)
		{
			foreach(int a in main.linked_indexes[i.serial_index].unique_indexes)
			{
				visual_mesh[a] = plane_mesh[i.serial_index];
			}
		}
		main.chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.vertices = visual_mesh;
	}

	public bool UpperChunk(int chunk_index)
	{
		return chunk_index + main.terrain_creator.width < main.terrain_creator.lenght * main.terrain_creator.width;
	}
	public bool LowerChunk(int chunk_index)
	{
		return chunk_index - main.terrain_creator.width >= 0;
	}
	public bool LeftChunk(int chunk_index)
	{
		return chunk_index - 1 % main.terrain_creator.width - 1 != 0 && chunk_index - 1 >= 0;
	}
	public bool RightChunk(int chunk_index)
	{
		return chunk_index + 1 % main.terrain_creator.width != 0 && chunk_index + 1 < main.terrain_creator.lenght * main.terrain_creator.width;
	}

}

[System.Serializable]
public struct LinkedIndex
{
	public int normal_index;
	public List<int> unique_indexes;
} 

[System.Serializable]
public class Face
{
	public int[] vi;
}

[System.Serializable]
public class ConnectedVertices
{
	public int chunk_index_shift;
	public int vert_index;

	public ConnectedVertices(int chunk_index_shift, int vert_index)
	{
		this.chunk_index_shift = chunk_index_shift;
		this.vert_index = vert_index;
	}
}



public class SelectedVertices 
{	
	public int chunk_index;
	public List<SV> vertices_index;

	public SelectedVertices(int chunk_index, List<SV> vertices_index) 
	{
		this.chunk_index = chunk_index;
		this.vertices_index = vertices_index;
	}
}

public class SV
{
	public int serial_index;
	public int x_index;
	public int y_index;

	public SV(int x_index, int y_index)
	{
		this.x_index = x_index;
		this.y_index = y_index;
		serial_index = XY_To_Serial(x_index, y_index);
	}

	public SV(int serial_index)
	{
		this.serial_index = serial_index;
		Vector2Int xy = Serial_To_XY(serial_index);
		this.x_index = xy.x;
		this.y_index = xy.y;
	}

	public Vector2Int Serial_To_XY(int serial_index)
	{
		return new Vector2Int(serial_index / 11, serial_index % 11);
	}

	public int XY_To_Serial(int x, int y)
	{
		return x * 11 + y;
	}
}

public class VerticesCopy
{
	public Vector3[] vertices;
	public VerticesCopy(Vector3[] vertices)
	{
		this.vertices = vertices;
	}
}
