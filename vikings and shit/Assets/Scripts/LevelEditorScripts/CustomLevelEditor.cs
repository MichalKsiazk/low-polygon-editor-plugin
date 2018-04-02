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


	public GameObject debug_cube;

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

	public LinkedIndex[] linked_indexes;


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
			if(t == null)
			{
				continue;
			}
			if (t.gameObject.name == terrain_object.name) 
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

		chunks = new Chunk[width * lenght];
		Mesh prefab_mesh = chunk_prefab.GetComponent<MeshFilter>().sharedMesh;
		Mesh unique_mesh = VisualMesh(prefab_mesh);
		linked_indexes = LinkIndexes(prefab_mesh, unique_mesh);

		for (int z = 0; z < lenght; z++) 
		{
			for (int x = 0; x < width; x++) 
			{

				int index = z * width + x;

				chunks [index] = new Chunk (chunk_prefab, new Vector3 (5 + x * 10f, 0, 5 + z * 10f), x, z, z * width + x);
				chunks [index].plane.transform.parent = terrain_object.transform;
				chunks [index].plane.layer = LayerMask.NameToLayer ("Terrain");
				chunks [index].chunk_serial_index = index;
				chunks [index].plane.name = "chunk: " + index.ToString ();

				chunks [index].colors = NewColorMap();
				//chunks [index].plane.GetComponent<MeshFilter>().sharedMesh = InvertedTriangles(chunks [index].plane.GetComponent<MeshFilter>().sharedMesh);
				InitEmptyVisualMesh(chunks[index]);

				chunks[index].visual.GetComponent<MeshFilter>().sharedMesh = VisualMesh(chunks [index].plane.GetComponent<MeshFilter>().sharedMesh);
				int l = chunks[index].visual.GetComponent<MeshFilter>().sharedMesh.vertices.Length;
				chunks[index].visual.GetComponent<MeshFilter>().sharedMesh.colors = NewTriangleColorMap(chunks [index].colors, l);
			}
		}
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


	#endregion



	#region UTILS

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
				sum += chunks[s.chunk_index].mesh_filter.sharedMesh.vertices[s.vertices_index[i]].y;
			}
		}

		return sum / size;
	}


	public Mesh InvertedTriangles(Mesh mesh)
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

					indices.Add(z * size + x); //3
					indices.Add((z + 1) * size + x); //2
					indices.Add((z + 1) * size + x + 1); //1					
				}
				t = !t;
            }
			t = !t;
        }
		mesh.triangles = indices.ToArray();
		return mesh;
	}
		

	#endregion



	#region PAINTER


	//instantiate all triangles as unique from parameter mesh 

	public Mesh VisualMesh(Mesh original_mesh) 
	{

		Mesh mesh = Instantiate<Mesh>(original_mesh);

		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		var newVertices = new Vector3[mesh.triangles.Length];
		var newUV = new Vector2[mesh.triangles.Length];
		var newNormals = new Vector3[mesh.triangles.Length];
		var newTriangles = new int[mesh.triangles.Length];

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

	public void UpdateTexture(Chunk chunk)
	{
		Mesh mesh = chunk.plane.GetComponent<MeshFilter>().sharedMesh;
		Vector3[] newVertices = new Vector3[mesh.triangles.Length];	
		for (int i = 0; i < mesh.triangles.Length; i++) 
		{
			newVertices[i] = mesh.vertices[mesh.triangles[i]];
		}
		chunk.visual.GetComponent<MeshFilter>().sharedMesh.vertices = newVertices;
	}

	public void UpdateMesh(SelectedVertices sv)
	{	
		Vector3[] plane_mesh = chunks[sv.chunk_index].plane.GetComponent<MeshFilter>().sharedMesh.vertices;
		Vector3[] visual_mesh = chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.vertices;

		foreach(int i in sv.vertices_index)
		{
			foreach(int a in linked_indexes[i].unique_indexes)
			{
				visual_mesh[a] = plane_mesh[i];
			}
		}
		chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.vertices = visual_mesh;
	}

	Color[] NewColorMap()
	{
		Color[] new_colors = new Color[200];
		for(int i = 0; i < 200; i++)
		{
			new_colors[i] = getRandomColor();
		}
		//new_colors[0] = Color.red;
		return new_colors;

	}

	Color[] NewTriangleColorMap(Color[] original_colors, int lenght)
	{
		Color[] colors = new Color[lenght];
		for (int i = 0; i < colors.Length; i+=3)
		{
			for(int x = 0; x < 3; x++)
			{
				colors[i + x] = original_colors[i / 3];
			}
		}
		return colors;
	}
     
	private Color getRandomColor()
	{
		float red = Random.Range(0.0f, 0.10f);
		float green = Random.Range(0.2f, 0.25f);
		float blue = Random.Range(0, 0.0f);
		return new Color(red, green, blue);
	}


	#endregion

}
	
public struct LinkedIndex
{
	public int normal_index;
	public List<int> unique_indexes;
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
	
