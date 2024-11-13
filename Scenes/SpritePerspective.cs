using Godot;
using System;

public partial class SpritePerspective : Sprite3D {

	[Export]
	public SpriteFrames perspectives;

	double dt;
	int spriteIndex;
	int frameCount;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		frameCount = perspectives.GetFrameCount("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		dt += delta;
		if(dt > 1.0) {
			Texture = perspectives.GetFrameTexture("default", spriteIndex);
			spriteIndex++;
			spriteIndex %= frameCount;
			dt = 0.0;
		}
	}
}
