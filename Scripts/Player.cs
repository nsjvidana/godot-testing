using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;

	SpringArm3D springArm;
	SpritePerspective spritePerspective;
	Node3D characterArmature;

    public override void _Ready()
    {
        springArm = GetNode<SpringArm3D>("SpringArm3D");
        spritePerspective = GetNode<Sprite3D>("Sprite3D") as SpritePerspective;
		characterArmature = GetNode<Node3D>("CharacterArmature");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			direction = direction.Rotated(Vector3.Up, springArm.Rotation.Y).Normalized();
		if (direction != Vector3.Zero) {
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			var lookDir = new Vector2(direction.X, direction.Z);
			var rotation = Quaternion.FromEuler(new Vector3(0f, -new Vector2(0, -1).AngleTo(lookDir), 0f));

			characterArmature.GlobalTransform = new Transform3D(
				new Basis(rotation),
				characterArmature.GlobalTransform.Origin
			).ScaledLocal(characterArmature.GlobalTransform.Basis.Scale);
			// characterArmature.GlobalTransform = Quaternion.FromEuler(new Vector3(0f, -new Vector2(0, -1).AngleTo(lookDir), 0f));
		}
		else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

	}
}
