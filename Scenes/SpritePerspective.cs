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
		// dt += delta;
		// if(dt > 1.0) {
		// 	Texture = perspectives.GetFrameTexture("default", spriteIndex);
		// 	spriteIndex++;
		// 	spriteIndex %= frameCount;
		// 	dt = 0.0;
		// }

		//billboarding
		//not going to use Sprite3D.Billboard because setting the sprite perspective
		//requires that  we know the rotation for billboarding.
		var camPos = GetViewport().GetCamera3D().GlobalTransform.Origin;
			camPos.Y = 0;
		// var currYPos = GlobalPosition.Y;
		// GlobalPosition = new Vector3(GlobalPosition.X, 0f, GlobalPosition.Z);//temporarily set y pos to 0 for LookAt calculation
		LookAt(camPos, Vector3.Up);
		// GlobalPosition = new Vector3(GlobalPosition.X, currYPos, GlobalPosition.Z);

		//0 - back
		//1 - facing left
		//2 - front
		//3 - facing right
		var rotDiff = Rotation.Y - player.Rotation.Y;
		GD.Print(rotDiff);
		var rotDiffAbs = Mathf.Abs(rotDiff);

		
		for(int i = 0; i < 4; i++) {
			
		}
		if(rotDiffAbs <= Mathf.Pi/4f) 
			Texture = perspectives.GetFrameTexture("default", 0);
		else if(Mathf.Abs(rotDiffAbs-Mathf.Pi/4f) <= Mathf.Pi/4f) 
			Texture = perspectives.GetFrameTexture("default", Mathf.Sign(rotDiff) == 1 ? 1:1);
		else if(Mathf.Abs(rotDiffAbs - 2f*Mathf.Pi/4f) <= Mathf.Pi/4f)
			Texture = perspectives.GetFrameTexture("default", Mathf.Sign(rotDiff) == 1 ? 2:2);
		else if(Mathf.Abs(rotDiffAbs - 3f*Mathf.Pi/4f) <= Mathf.Pi/4f)
			Texture = perspectives.GetFrameTexture("default", Mathf.Sign(rotDiff) == 1 ? 3:3);
		else if(Mathf.Abs(rotDiffAbs - 4f*Mathf.Pi/4f) <= Mathf.Pi/4f) {
			GD.Print("ae");
			Texture = perspectives.GetFrameTexture("default", Mathf.Sign(rotDiff) == 1 ? 4:4);
		}

		
		// else if(Mathf.Abs(rotDiffAbs-Mathf.Pi/2f) <= Mathf.Pi/4f) {
		// 	if(Mathf.Sign(rotDiff) == 1)
		// 		Texture = perspectives.GetFrameTexture("default", 3);
		// 	else
		// 		Texture = perspectives.GetFrameTexture("default", 1);
		// }
		// else
		// 	Texture = perspectives.GetFrameTexture("default", 0);
	}
}
