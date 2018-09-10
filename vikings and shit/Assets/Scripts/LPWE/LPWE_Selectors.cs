using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LPWE_Selectors : LPWE_Component 
{
	[SerializeField] public float radius;

	public SelectionShape shape;

	public Vector3 ColliderRaycastDetection() 
	{
		Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		RaycastHit hit;
 
		if (Physics.Raycast (ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"))) 
		{
			return hit.point;
		}
		return Vector3.zero;
	}

	public Vector3 AxisRaycastDetection() 
	{
		Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000, 1 << LayerMask.NameToLayer("Water"))) 
		{
			return hit.point;
		}
		return Vector3.zero;
	}

	public List<SelectedVertices> FindInRange(Vector3 point, float radius, HitPoint scan_type, bool vp) 
	{
		
		List<int> nearest_chunks = NearestChunks(point);

		List<SelectedVertices> selected_vertices = new List<SelectedVertices> ();
		if(shape == SelectionShape.Circle)
		{
			foreach (int i in nearest_chunks) 
			{

				if(vp)
				{
					selected_vertices.Add(FindFaces3D(main.chunks [i], point, radius));
					continue;
				}

				switch(scan_type) 
				{
					case HitPoint.SeaLevel2D:
						selected_vertices.Add (RoundScan2D (main.chunks [i], point, radius));
						break;
					case HitPoint.HitPoint3D:
						selected_vertices.Add (RoundScan3D (main.chunks [i], point, radius));
						break;
					case HitPoint.HitPoint2D:
						selected_vertices.Add (RoundScan2D (main.chunks [i], point, radius));
						break;
				}
			}
		}
		else if(shape == SelectionShape.Rectangle)
		{
			foreach (int i in nearest_chunks) 
			{
				if(vp)
				{
					selected_vertices.Add(FindFaces3D(main.chunks [i], point, radius));
					continue;
				}


				switch(scan_type) 
				{
					case HitPoint.SeaLevel2D:
						selected_vertices.Add (SquareScan2D (main.chunks [i], point, radius));
						break;
					case HitPoint.HitPoint3D:
						selected_vertices.Add (SquareScan3D (main.chunks [i], point, radius));
						break;
					case HitPoint.HitPoint2D:
						selected_vertices.Add (SquareScan2D (main.chunks [i], point, radius));
						break;
				}
			}
		}
		return selected_vertices;
	}


	SelectedVertices RoundScan2D (Chunk chunk, Vector3 point, float size) 
	{

		List<SV> vertices = new List<SV> ();
		MeshFilter mf = chunk.plane.GetComponent<MeshFilter> ();

		for(int i = 0; i < mf.sharedMesh.vertices.Length; i++)
		{

			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			float vx = real_pos.x;
			float vz = real_pos.z;

			float dst = Vector2.Distance (new Vector2(point.x, point.z), new Vector2(vx, vz)); 

			if (dst < size) 
			{
				vertices.Add (new SV(i));
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}


	SelectedVertices SquareScan2D (Chunk chunk, Vector3 point, float size) 
	{

		List<SV> vertices = new List<SV> ();
		MeshFilter mf = chunk.plane.GetComponent<MeshFilter> ();

		for(int i = 0; i < mf.sharedMesh.vertices.Length; i++)
		{

			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			float vx = real_pos.x;
			float vz = real_pos.z;

			bool bounds_x = real_pos.x > point.x - size && real_pos.x < point.x + size;
			bool bounds_z = real_pos.z > point.z - size && real_pos.z < point.z + size;

			if (bounds_x && bounds_z) 
			{
				vertices.Add (new SV(i));
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}


	SelectedVertices RoundScan3D (Chunk chunk, Vector3 point, float size) 
	{

		List<SV> vertices = new List<SV> ();
		MeshFilter mf = chunk.plane.GetComponent<MeshFilter> ();

		for(int i = 0; i < mf.sharedMesh.vertices.Length; i++) 
		{
			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			float dst = Vector3.Distance (point, real_pos); 

			if (dst < size) 
			{
				vertices.Add (new SV(i));
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}


	SelectedVertices SquareScan3D (Chunk chunk, Vector3 point, float size) 
	{

		List<SV> vertices = new List<SV> ();
		MeshFilter mf = chunk.plane.GetComponent<MeshFilter> ();

		for(int i = 0; i < mf.sharedMesh.vertices.Length; i++) 
		{
			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			bool bounds_x = real_pos.x > point.x - size && real_pos.x < point.x + size;
			bool bounds_y = real_pos.y > point.y - size && real_pos.y < point.y + size; 
			bool bounds_z = real_pos.z > point.z - size && real_pos.z < point.z + size; 

			if (bounds_x && bounds_y && bounds_z)
			{
				vertices.Add (new SV(i));
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}

	SelectedVertices FindFaces3D (Chunk chunk, Vector3 point, float size) 
	{

		List<SV> vertices = new List<SV> ();
		MeshFilter mf = chunk.visual.GetComponent<MeshFilter> ();

		for(int i = 0; i < mf.sharedMesh.vertices.Length; i+=3) 
		{
			Vector3 real_pos = mf.sharedMesh.vertices [i] + chunk.plane.transform.position;

			float dst = Vector3.Distance (point, real_pos); 

			if (dst < size) 
			{
				vertices.Add (new SV(i));
			}
		}
		return new SelectedVertices(chunk.chunk_serial_index, vertices);
	}


	//unused outdated
	public int FindNearest(Vector3 point) 
	{
		
		Vector3[] vertices = main.terrain_mesh.vertices;
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


	//unused outdated
	public int FindNearestHorizontal(Vector3 point) 
	{
		
		Vector3[] vertices = main.terrain_mesh.vertices;
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

		int width = main.terrain_creator.width;
		int lenght = main.terrain_creator.lenght;

		for (int z = z_index - 1; z <= z_index + 1; z++) 
		{
			for (int x = x_index - 1; x <= x_index + 1; x++) 
			{
				if (x < 0 || z < 0 || z >= main.terrain_creator.width || x >= lenght) 
				{
					continue;
				} 
				else 
				{
					selected_chunks.Add (main.chunks [z * lenght + x].chunk_serial_index);
				}
			}
		}

		return selected_chunks;
	}
}
