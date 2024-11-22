using Godot;
using System;

public partial class SpritePerspective : Sprite3D {

	[Export]
	public SpriteFrames perspectives;
	[Export]
	public Node3D curr;

	double dt;
	int spriteIndex;
	int frameCount;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		frameCount = perspectives.GetFrameCount("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var camPos = GetViewport().GetCamera3D().GlobalTransform.Origin;
		var thisPos = this.GlobalTransform.Origin;
		var dirToCamera3d = camPos - thisPos;
		var dirToCamera = new Vector2(dirToCamera3d.X, dirToCamera3d.Z);
		
		var currDir3d = this.Quaternion * Vector3.Forward;
		var currDir = new Vector2(currDir3d.X, currDir3d.Z);
		
		var rotDisp = currDir.AngleTo(dirToCamera);
		var rot2Pi = rotDisp < 0 ? (rotDisp + Mathf.Tau)%Mathf.Tau : rotDisp;

		//the "%Mathf.Tau" when calculating rot2Pi prevents rot2Pi/Mathf.Tau from becoming 1.
		//this lets us multiply by the frameCount with no worries of an out-of-bounds index.
		int index = (int)(rot2Pi / Mathf.Tau * frameCount);
		Texture = perspectives.GetFrameTexture("default", index);
	}
}
