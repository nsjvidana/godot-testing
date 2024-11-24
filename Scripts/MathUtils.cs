using Godot;
using System;

public partial class MathUtils : Node
{
    public static Vector3 ProjectOnPlane(Vector3 toProject, Vector3 planeNormal) {
		var dot = toProject.Dot(planeNormal);
		var projectedOnNormal = planeNormal * dot;
		return toProject - projectedOnNormal;
	}
}
