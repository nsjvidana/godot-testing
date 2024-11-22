using Godot;
using System;

public partial class SpritePerspective : Sprite3D {

	[Export]
	public SpriteFrames perspectives;
	[Export]
	public Node3D player;

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
		
		var playerDir3d = player.Quaternion * Vector3.Forward;
		var playerDir = new Vector2(playerDir3d.X, playerDir3d.Z);
		
		var rotDisp = playerDir.AngleTo(dirToCamera);
		var rot2Pi = rotDisp < 0 ? (rotDisp + Mathf.Tau) : rotDisp;

		if(rot2Pi != Mathf.Tau) {
			int index = (int)(rot2Pi / Mathf.Tau * frameCount);
			Texture = perspectives.GetFrameTexture("default", index);
		}
	}
}
