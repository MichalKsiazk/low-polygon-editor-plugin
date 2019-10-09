using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPWE : MonoBehaviour {

	//unused outdated
	[HideInInspector] public Mesh terrain_mesh;
	//unused outdated
	[HideInInspector] public MeshCollider terrain_collider;
	//unused outdated
	[HideInInspector] public MeshRenderer terrain_renderer;

	public Chunk[] chunks;

	//need optimisation!!!
	[SerializeField, HideInInspector]
	public LinkedIndex[] linked_indexes;
	[SerializeField, HideInInspector]
	public Face[] faces;
	[SerializeField, HideInInspector]
	public int[] linked_faces;

	[SerializeField]
	public List<ConnectedVertices>[] connected_vertices;




	[HideInInspector] public LPWE_MeshEditor mesh_editor;
	[HideInInspector] public LPWE_TerrainCreator terrain_creator;
	[HideInInspector] public LPWE_WorldPainter world_painter;
	[HideInInspector] public LPWE_Utils utils;
	[HideInInspector] public LPWE_Selectors selectors;
	[HideInInspector] public LPWE_Highlights highlights;

	[HideInInspector] public HitPoint hit_point;
	[HideInInspector] public DragType event_type;
	[HideInInspector] public HighlightOptions highlight_options;
	[HideInInspector] public Selection selection;

	[HideInInspector] public GameObject chunk_prefab;


	[HideInInspector] public int modulo;



	[HideInInspector] public GameObject map;
	[HideInInspector] public GameObject terrain;
	[HideInInspector] public GameObject entities;


}
