using Godot;
using System;

public partial class TestSpriteScript : Sprite3D
{
	[Export]
	public ShaderMaterial shaderMaterial;
	
	public override void _Ready()
	{
		shaderMaterial.SetShaderParameter("sprite_texture", this.Texture);
		this.MaterialOverride = shaderMaterial;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
