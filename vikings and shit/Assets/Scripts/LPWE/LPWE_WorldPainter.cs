using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPWE_WorldPainter : LPWE_Component
{

	public PaintTools tools;
	public Color temp_color;

	public void Paint(SelectedVertices sv, Color color)
	{
		Color[] colors = main.chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.colors;

		//Debug.Log("TL  " + triangles.Length.ToString());
		//Debug.Log("CL  " + colors.Length.ToString()); 
		//Debug.Log("VL  " + main.chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.vertices.ToString()); 

	 
		foreach(SV s in sv.vertices_index)
		{
			Color col = getRandomColor(color);
			if((s.serial_index == 0 || s.serial_index % 3 == 0))
			{
				colors[s.serial_index] = col;
				colors[s.serial_index + 1] = col;
				colors[s.serial_index + 2] = col;
			}
			else
			{
				Debug.Log(colors.Length);
			}
		}
		main.chunks[sv.chunk_index].visual.GetComponent<MeshFilter>().sharedMesh.colors = colors; 
	}


	public void UpdateColors(int chunk_index, Color[] colors)
	{

	}





	public Color[] NewColorMap()
	{
		Color[] colors = new Color[200];
		for(int i = 0; i < 200; i++)
		{
			colors[i] = getRandomColor();
		}
		//new_colors[0] = Color.red;
		return colors;

	}

	public Color[] NewTriangleColorMap(Color[] original_colors, int lenght)
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
		float red = Random.Range(0.0f, 0.0f);
		float green = Random.Range(0.23f, 0.27f);
		float blue = Random.Range(0, 0.0f);
		return new Color(red, green, blue);
	}

	private Color getRandomColor(Color color)
	{
		float rand = Random.Range(-0.02f, 0.02f);
		color.r += rand;
		color.g += rand;
		color.b += rand;
		return color;
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
}
