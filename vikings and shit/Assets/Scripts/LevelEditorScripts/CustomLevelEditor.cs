using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLevelEditor : MonoBehaviour 
{

	public GameObject terrain_object;



	[HideInInspector] public Mesh terrain_mesh;
	[HideInInspector] public MeshCollider terrain_collider;
	[HideInInspector] public MeshRenderer terrain_renderer;

	[Range(1,100)]
	public int width;
	[Range(1,100)]
	public int lenght;

	public GameObject chunk_prefab;
	public GameObject highlight;

	public Material default_material;


	[HideInInspector]
	[Range(-1f, 1f)]
	public float delta_height;

	[HideInInspector]
	[Range(0, 10f)]
	public float radius;

	[HideInInspector]
	[Range(0f,1f)]
	public float flat_strenght;

	[HideInInspector]
	public float height;

	public Selection selection;
	public Tools tool;
	public FlatOptions flat_options;
	public DragType event_type;
	public HighlightOptions highlight_options;


	public Chunk[] chunks;



	#region TOOLS_DEFINITIONS


	public void SetHeight(SelectedVertices sv, float y)
	{
		Vector3[] vertices = chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;

		foreach(int i in sv.vertices_index) 
		{
			vertices [i] = new Vector3(vertices[i].x, y, vertices[i].z);
		}
		
		chunks [sv.chunk_index].mesh_filter.sharedMesh.vertices = vertices;
		DestroyImmediate (chunks [sv.chunk_index].plane.GetComponent<MeshCollider> ());
		chunks[sv.chunk_index].plane.gameObject.AddComponent<MeshCollider>();
		chunks[sv.chunk_index].plane.GetComponent<MeshCollider>().sharedMesh = chunks[sv.chunk_index].mesh_filter.sharedMesh;
	}


	public void Flat(SelectedVertices sv, Vector3 hit_point) 
	{
		
		Vector3[] vertices = chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;

		foreach (int i in sv.vertices_index) 
		{
			if (vertices [i].y > hit_point.y) 
			{
				vertices [i] -= new Vector3 (0, flat_strenght, 0);
			} 
			else 
			{
				vertices [i] += new Vector3 (0, flat_strenght, 0);
			}
		}
		chunks [sv.chunk_index].mesh_filter.sharedMesh.vertices = vertices;
		DestroyImmediate (chunks [sv.chunk_index].plane.GetComponent<MeshCollider> ());
		chunks[sv.chunk_index].plane.gameObject.AddComponent<MeshCollider>();
		chunks[sv.chunk_index].plane.GetComponent<MeshCollider>().sharedMesh = chunks[sv.chunk_index].mesh_filter.sharedMesh;
	}



	public void FlatToAverage(SelectedVertices sv, float average) 
	{

		Vector3[] vertices = chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;

		foreach (int i in sv.vertices_index) 
		{
			if (vertices [i].y > average) 
			{
				vertices [i] -= new Vector3 (0, flat_strenght, 0);
			} 
			else 
			{
				vertices [i] += new Vector3 (0, flat_strenght, 0);
			}
		}

		chunks [sv.chunk_index].mesh_filter.sharedMesh.vertices = vertices;
		DestroyImmediate (chunks [sv.chunk_index].plane.GetComponent<MeshCollider> ());
		chunks[sv.chunk_index].plane.gameObject.AddComponent<MeshCollider>();
		chunks[sv.chunk_index].plane.GetComponent<MeshCollider>().sharedMesh = chunks[sv.chunk_index].mesh_filter.sharedMesh;

	}

	public void Raise(SelectedVertices sv, float delta_height) 
	{		
		Vector3[] vertices = chunks[sv.chunk_index].mesh_filter.sharedMesh.vertices;
		foreach (int i in sv.vertices_index) 
		{
			vertices [i] += new Vector3 (0, delta_height, 0);
		}

		chunks [sv.chunk_index].mesh_filter.sharedMesh.vertices = vertices;


		DestroyImmediate (chunks [sv.chunk_index].plane.GetComponent<MeshCollider> ());
		chunks[sv.chunk_index].plane.gameObject.AddComponent<MeshCollider>();
		chunks[sv.chunk_index].plane.GetComponent<MeshCollider>().sharedMesh = chunks[sv.chunk_index].mesh_filter.sharedMesh;
	}


	#endregion



	#region SELECTORS_DEFINITIONS


	public List<SelectedVertices> FindInRange(Vector3 point, float radius, Selection scan_type) 
	{
		
		List<int> nearest_chunks = NearestChunks(point);

		List<SelectedVertices> selected_vertices = new List<SelectedVertices> ();

		foreach (int i in nearest_chunks) 
		{
			switch(scan_type) 
			{
			case Selection.SeaLevel2D:
				selected_vertices.Add (ScanChunkIn2D (chunks [i], point, radius));
				break;
			case Selection.HitPoint3D:
				selected_vertices.Add (ScanChunkIn3D (chunks [i], point, radius));
				break;
			case Selection.HitPoint2D:
				selected_vertices.Add (ScanChunkIn2D (chunks [i], point, radius));
				break;
			}
		}
		return selected_vertices;
	}


	SelectedVertices ScanChunkIn2D (Chunk chunk, Vector3 point, float radius) 
	{

		List<int> vertices = new List<int> ();
		MeshFilter mf = chunk.plane.GetComponent<MeshFilter> ();

		for(int i = 0; i < mf.sharedMesh.vertices.Length; i++)
		{

			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			float vx = real_pos.x;
			float vz = real_pos.z;

			float dst = Vector2.Distance (new Vector2(point.x, point.z), new Vector2(vx, vz)); 

			if (dst < radius) 
			{
				vertices.Add (i);
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}


	SelectedVertices ScanChunkIn3D (Chunk chunk, Vector3 point, float radius) 
	{

		List<int> vertices = new List<int> ();
		MeshFilter mf = chunk.plane.GetComponent<MeshFilter> ();


		for(int i = 0; i < mf.sharedMesh.vertices.Length; i++) 
		{
			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			float dst = Vector3.Distance (point, real_pos); 

			if (dst < radius) 
			{
				vertices.Add (i);
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}


	public int FindNearest(Vector3 point) 
	{
		
		Vector3[] vertices = terrain_mesh.vertices;
		int i = 0;
		float smallest_dst = Vector3.Distance(point, vertices[i]);
		int index = -1;
		i++;
		for (; i < vertices.Length; i++) 
		{
			float dst = Vector3.Distance(point, vertices[i]);
			if (dst < smallest_dst) 
			{
				smallest_dst = dst;
				index = i;
			}
		}
		return index;
	}


	public int FindNearestHorizontal(Vector3 point) 
	{
		
		Vector3[] vertices = terrain_mesh.vertices;
		Vector2[] vertices_2D = new Vector2[vertices.Length];
		int i = 0;
		vertices_2D [i] = new Vector2 (vertices[i].x, vertices[i].z);
		float smallest_dst = Vector2.Distance(new Vector2(point.x, point.z), vertices_2D[i]);
		int index = -1;
		i++;

		for (; i < vertices_2D.Length; i++) 
		{
			vertices_2D [i] = new Vector2 (vertices[i].x, vertices[i].z);
			float dst =  Vector2.Distance(new Vector2(point.x, point.z), vertices_2D[i]);
			if (dst < smallest_dst) 
			{
				smallest_dst = dst;
				index = i;
			}
		}
		return index;
	}		


	List<int> NearestChunks(Vector3 point) 
		{
		
		List<int> selected_chunks = new List<int> ();
		int x_index = (int)point.x / 10;
		int z_index = (int)point.z / 10;

		for (int z = z_index - 1; z <= z_index + 1; z++) 
		{
			for (int x = x_index - 1; x <= x_index + 1; x++) 
			{
				if (x < 0 || z < 0 || z >= lenght || x >= width) 
				{
					continue;
				} 
				else 
				{
					selected_chunks.Add (chunks [z * width + x].chunk_serial_index);
				}
			}
		}

		return selected_chunks;
	}		


	#endregion 



	#region CORE_DEFINITIONS


	public void LoadChunkMap() 
	{		
		Transform[] childs = terrain_object.transform.GetComponentsInChildren<Transform> ();
		chunks = new Chunk[width * lenght];

		foreach (Transform t in childs) 
		{
			string name = t.gameObject.name;
			if (name == "Map") 
			{
				continue;
			}

			int index = int.Parse(name.Replace ("chunk: ", ""));
			chunks [index] = new Chunk (t.gameObject, index, width);
		}
	}


	public void DestroyTerrain() 
	{
		Transform[] childs = terrain_object.transform.GetComponentsInChildren<Transform> ();

		foreach(Transform t in childs) 
		{
			if (t.gameObject.name == terrain_object.name) 
			{
				continue;
			}
			DestroyImmediate (t.gameObject);
		}
	}


	public void ResetTerrain() {
		
		DestroyTerrain ();

		chunks = new Chunk[width * lenght];
		for (int z = 0; z < lenght; z++) 
		{
			for (int x = 0; x < width; x++) 
			{

				int index = z * width + x;

				chunks [index] = new Chunk (chunk_prefab, new Vector3 (5 + x * 10f, 0, 5 + z * 10f), x, z, z * width + x);
				chunks [index].plane.transform.parent = terrain_object.transform;
				chunks [index].plane.GetComponent<Renderer> ().sharedMaterial = default_material;
				chunks [index].plane.layer = LayerMask.NameToLayer ("Terrain");
				chunks [index].chunk_serial_index = index;
				chunks [index].plane.name = "chunk: " + index.ToString ();
				//TestTexture (chunks [index]);
			}
		}
	}


	#endregion



	#region UTILS


	public float Average(List<SelectedVertices> sv) 
	{
		
		float sum = 0.0f;
		float size = 0.0f;

		foreach (SelectedVertices s in sv) 
		{
			size += s.vertices_index.Count;
			for(int i = 0; i < s.vertices_index.Count; i++)
			{
				sum += chunks[s.chunk_index].mesh_filter.sharedMesh.vertices[s.vertices_index[i]].y;
			}
		}

		return sum / size;
	}
		

	#endregion



	#region PAINTER


	void TestTexture(Chunk chunk) 
	{
		Renderer rend = chunk.plane.GetComponent<Renderer>();

		Texture2D texture = new Texture2D (20, 20);
		rend.sharedMaterial.mainTexture = texture;
		Color[] colors = new Color[3];
		colors[0] = Color.red;
		colors[1] = Color.green;
		colors[2] = Color.blue;
		int mipCount = Mathf.Min(3, texture.mipmapCount);

		for (int mip = 0; mip < mipCount; mip++)
		{
			Color[] cols = texture.GetPixels(mip);
			for (int i = 0; i < cols.Length; i++)
			{
				cols [i] = new Color (0.9f,0, 0.1f);
			}
			texture.SetPixels(cols, mip);
		}

		texture.filterMode = FilterMode.Point;
		texture.Apply(false);
	}


	#endregion

}
	



public class SelectedVertices 
{
	
	public int chunk_index;
	public List<int> vertices_index;

	public SelectedVertices(int chunk_index, List<int> vertices_index) 
	{
		this.chunk_index = chunk_index;
		this.vertices_index = vertices_index;
	}
}
	
