using Godot;
using System;

public partial class MultiMeshTesting : MultiMeshInstance3D
{
	
	[Export]
	public Texture2D spritesheet;

	public override void _Ready()
	{
		var mat = Multimesh.Mesh.SurfaceGetMaterial(0);
		if(mat is ShaderMaterial) {
			var shader = (mat as ShaderMaterial);
			shader.SetShaderParameter("spritesheet", spritesheet);
			shader.SetShaderParameter("num_perspectives", 16);
		}
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
