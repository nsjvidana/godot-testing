using Godot;
using System;

public partial class SpringArm3d : SpringArm3D  {

	public const float mouseSensitivity = 0.05f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		TopLevel = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
