using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Selection 
{
	HitPoint2D,
	HitPoint3D,
	SeaLevel2D
}

public enum Tools 
{
	SetHeight,
	Raise,
	Flat,
	Paint,
	Randomize
}

public enum FlatOptions 
{
	ToHitPoint, 
	ToMedianPoint
}

public enum HighlightOptions 
{
	None,
	Mesh
}

public enum DragType
{
	OnClick,
	Drag
}