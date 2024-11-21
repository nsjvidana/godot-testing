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
		//billboarding
		//not going to use Sprite3D.Billboard because setting the sprite perspective
		//requires that  we know the rotation for billboarding.
		var camPos = GetViewport().GetCamera3D().GlobalTransform.Origin;
			camPos.Y = 0;
		LookAt(camPos, Vector3.Up);

		var rotDiff = Rotation.Y - player.Rotation.Y;
		var rot2Pi = rotDiff < 0 ? rotDiff + 2*Mathf.Pi : rotDiff;

		int index = (int)(rot2Pi / Mathf.Tau * frameCount);
		Texture = perspectives.GetFrameTexture("default", index);
	}
}
