using Godot;
using System;

public partial class SpriteArmatureee : Skeleton3D
{
    
    [Export]
    public PackedScene spriteMultimeshScene;
    [Export]
    public Texture2D spritesheet;
    [Export]
    public int numPerspectives = 16;
    [Export]
    public float spriteScale = 1f;

    int headIdx = -1;
    MultiMeshInstance3D multimeshInstance;

    public override void _Ready()
    {
        if(spriteMultimeshScene == null) {
            GD.PrintErr("Sprite Multimesh Scene is missing for node " + this + "!");
            this.QueueFree();
            return;
        }
        if(spritesheet == null) {
            GD.PrintErr("Spritesheet is missing for node " + this + "!");
            this.QueueFree();
            return;
        }

        // Setting up sprite multimesh
        var sceneInstance = spriteMultimeshScene.Instantiate();
            AddChild(sceneInstance);
        multimeshInstance = sceneInstance.GetNode<MultiMeshInstance3D>("MultiMeshInstance3D");
            multimeshInstance.TopLevel = true;
        
        var mat = this.multimeshInstance.Multimesh.Mesh.SurfaceGetMaterial(0).Duplicate() as Material;
		if(mat is ShaderMaterial) {
			var shader = mat as ShaderMaterial;
			shader.SetShaderParameter("spritesheet", spritesheet);
			shader.SetShaderParameter("num_perspectives", numPerspectives);
		}
        this.multimeshInstance.Multimesh.Mesh.SurfaceSetMaterial(0, mat);
        this.multimeshInstance.Multimesh.InstanceCount = 1;

        var boneNames = this.GetConcatenatedBoneNames().ToString().Split(',');
        int idx = 0;
        foreach(var boneName in boneNames) {
            if(boneName.EndsWith("Head")) {
                headIdx = idx;
                break;
            }
            idx++;
        }

        if(headIdx == -1) {
            GD.PrintErr("Couldn't find head of skeleton node " + this);
            this.QueueFree();
            return;
        }
    }

    public override void _Process(double delta)
    {
        LookAtFromPosition(Vector3.Zero, Vector3.Forward);

        var headRelative = GetBoneGlobalPose(headIdx);
        var headGlobal = this.GlobalTransform * headRelative;
        var head_rot = headGlobal.Basis.GetRotationQuaternion();
        var headTransformGlobal = Transform3D.Identity
            .Translated(headRelative.Origin)
            .RotatedLocal(Quaternion.GetAxis(), Quaternion.GetAngle())
            .Scaled(new Vector3(spriteScale, spriteScale, spriteScale));
        GD.Print(headGlobal.Origin);

        multimeshInstance.Multimesh.SetInstanceTransform(0, headTransformGlobal);

        multimeshInstance.Scale = Vector3.One;
    }


}
