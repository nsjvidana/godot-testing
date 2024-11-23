using Godot;
using System;

public partial class SpritePerspective : Sprite3D {

	[Export]
	public SpriteFrames perspectives;
	[Export]
	public Node3D curr;

	public Quaternion spriteRot = Quaternion.Identity;

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
		
		var currDir3d = spriteRot * Vector3.Forward;
		var currDir = new Vector2(currDir3d.X, currDir3d.Z);
		
		var rotDisp = currDir.AngleTo(dirToCamera);
		var rot2Pi = rotDisp < 0 ? (rotDisp + Mathf.Tau) : rotDisp;
			// rot2Pi = (float)Mathf.Snapped(rot2Pi, 0.000001);


		//"interpolate" between frames with a t value
		var t = (float)Mathf.Snapped(rot2Pi / Mathf.Tau, 0.000001);
		//use modulo to prevent an invalid index (rot2pi can be Mathf.Tau sometimes)
		int index = (int)(t * frameCount) % frameCount;
		Texture = perspectives.GetFrameTexture("default", index);
	}
}
