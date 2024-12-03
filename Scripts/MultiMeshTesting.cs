using Godot;
using System;

public partial class MultiMeshTesting : MultiMeshInstance3D
{
	
	[Export]
	public Texture2D spritesheet;
	[Export]
	public int numPerspectives = 16;

	public override void _Ready()
	{
		var mat = Multimesh.Mesh.SurfaceGetMaterial(0).Duplicate() as Material;
		if(mat is ShaderMaterial) {
			var shader = (mat as ShaderMaterial);
			shader.SetShaderParameter("spritesheet", spritesheet);
			shader.SetShaderParameter("num_perspectives", numPerspectives);
		}
		Multimesh.Mesh.SurfaceSetMaterial(0, mat);
		Multimesh.InstanceCount = 1;
		for(int i = 0; i < Multimesh.InstanceCount; i++) {
			var transform = Transform3D.Identity.Translated(new Vector3(i*1.5f, 0f, 0f));
			Multimesh.SetInstanceTransform(i, transform);
		}
	}
}
