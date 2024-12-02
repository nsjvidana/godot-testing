using Godot;
using System;

public partial class SpriteArmature : Node3D
{
    [Export]
    public PackedScene spriteMultimeshScene;
    [Export]
    public Texture2D spritesheet;
    [Export]
    public int numPerspectives = 16;
    [Export]
    public int spriteSize = 32;
    [Export]
    public float spriteScale = 1f;
    [Export]
    public Node3D testObj;

    int headIdx = -1;
    BoneAttachment3D headBone;
    MultiMeshInstance3D multimeshInstance;
    Skeleton3D skeleton;

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

        var sceneInstance = spriteMultimeshScene.Instantiate();
            AddChild(sceneInstance);
        multimeshInstance = sceneInstance.GetNode<MultiMeshInstance3D>("MultiMeshInstance3D");
            // multimeshInstance.TopLevel = true;
        skeleton = this.GetNode<Skeleton3D>("Armature/Skeleton3D");
        
        var mat = multimeshInstance.Multimesh.Mesh.SurfaceGetMaterial(0).Duplicate() as Material;
		if(mat is ShaderMaterial) {
			var shader = mat as ShaderMaterial;
			shader.SetShaderParameter("spritesheet", spritesheet);
			shader.SetShaderParameter("num_perspectives", numPerspectives);
			shader.SetShaderParameter("sprite_size", spriteSize);
		}
        multimeshInstance.Multimesh.Mesh.SurfaceSetMaterial(0, mat);
        multimeshInstance.Multimesh.InstanceCount = 1;

        var boneNames = skeleton.GetConcatenatedBoneNames().ToString().Split(',');
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

        //setting bone attachments
        headBone = new BoneAttachment3D();
        skeleton.AddChild(headBone);
        headBone.BoneIdx = headIdx;
    }

    public override void _Process(double delta)
    {
        var pos = multimeshInstance.ToLocal(headBone.GlobalPosition);
        var euler = new Vector3(headBone.GlobalRotation.Z, headBone.GlobalRotation.Y, headBone.GlobalRotation.X);
        var rot = Quaternion.FromEuler(euler);
            rot *= new Basis(rot*Vector3.Up, Mathf.Pi).GetRotationQuaternion();
        testObj.GlobalRotation = euler;
        var transform = Transform3D.Identity
            .Rotated(rot.GetAxis(), rot.GetAngle())
            .Scaled(Vector3.One * spriteScale)
            .Translated(pos);
        // transform.Basis.Column0 = transform.Basis.Column0.Normalized() * Mathf.Pow(spriteScale, 2f);
        // transform.Basis.Column1 = transform.Basis.Column1.Normalized() * spriteScale;
        // transform.Basis.Column2 = transform.Basis.Column2.Normalized() * spriteScale;

        multimeshInstance.Multimesh.SetInstanceTransform(0, transform);
    }

}
