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
	SpriteArmature armature;

    public override void _Ready()
    {
        springArm = GetNode<SpringArm3D>("SpringArm3D");
		armature = GetNode<Node3D>("CharacterArmature") as SpriteArmature;
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
			armature.animation.Play("Run");
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			var lookDir = new Vector2(direction.X, direction.Z);
			var rotation = Quaternion.FromEuler(new Vector3(0f, -new Vector2(0, -1).AngleTo(lookDir), 0f));

			armature.GlobalTransform = new Transform3D(
				armature.GlobalTransform.Basis.Orthonormalized().Slerp(new Basis(rotation), (float)delta/0.1f),
				armature.GlobalTransform.Origin
			).ScaledLocal(armature.GlobalTransform.Basis.Scale);
		}
		else {
			armature.animation.Play("Idle");
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

	}
}
