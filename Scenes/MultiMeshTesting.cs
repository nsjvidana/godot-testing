using Godot;
using System;

public partial class MultiMeshTesting : MultiMeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multimesh.InstanceCount = 100;
		for(int i = 0; i < Multimesh.InstanceCount; i++) {
			var transform = Transform3D.Identity.Translated(new Vector3(i*1.5f, 0f, 0f));
			Multimesh.SetInstanceTransform(i, transform);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
