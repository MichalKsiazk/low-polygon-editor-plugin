using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum HitPoint
{
	HitPoint2D,
	HitPoint3D,
	SeaLevel2D
}

public enum Selection
{
	Dynamic,
	Static
}

public enum MeshTools 
{
	SetHeight,
	Raise,
	Flat,
	Randomize,
	FlatRaise
}

public enum SelectionShape
{
	Circle,
	Rectangle
}

public enum PaintTools
{
	Paint
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

public enum Extremes
{
	Minimum,
	Maximum
}


