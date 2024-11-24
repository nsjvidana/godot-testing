using Godot;
using System;

public partial class CameraSpringArm : SpringArm3D
{
	
	[Export]
	public float mouse_sensitivity = 0.05f;

	public override void _Ready()
	{
		// TopLevel = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseMotion) {
			var mevent = @event as InputEventMouseMotion;
			var xRot = RotationDegrees.X - (mevent.Relative.Y * mouse_sensitivity);
				xRot = Mathf.Clamp(xRot, -90f, 30f);
			var yRot = RotationDegrees.Y - (mevent.Relative.X * mouse_sensitivity);
				yRot = Mathf.Wrap(yRot, 0f, 360f);
			
			RotationDegrees = new Vector3(xRot, yRot, 0);
		}
    }

    public override void _Process(double delta)
	{
	}
}