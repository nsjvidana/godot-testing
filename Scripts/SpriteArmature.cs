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
    BoneAttachment3D headBoneHead;
    BoneAttachment3D headBoneTail;
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

        skeleton = this.GetNode<Skeleton3D>("Armature/Skeleton3D");
        var sceneInstance = spriteMultimeshScene.Instantiate();
            AddChild(sceneInstance);
        multimeshInstance = sceneInstance.GetNode<MultiMeshInstance3D>("MultiMeshInstance3D");
            // multimeshInstance.TopLevel = true;
        
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
        headBoneHead = new BoneAttachment3D();
            skeleton.AddChild(headBoneHead);
            headBoneHead.BoneIdx = headIdx;
        headBoneTail = new BoneAttachment3D();
            skeleton.AddChild(headBoneTail);
            headBoneTail.BoneIdx = headIdx+1;
    }

    public override void _Process(double delta)
    {
        var pos = multimeshInstance.ToLocal(headBoneTail.GlobalPosition);
        var euler = new Vector3(headBoneTail.GlobalRotation.Z, headBoneTail.GlobalRotation.Y, headBoneTail.GlobalRotation.X);
        var rot = Quaternion.FromEuler(euler);
        var up = (headBoneTail.GlobalPosition - headBoneHead.GlobalPosition).Normalized();
        var right = up.Cross(rot * Vector3.Forward);
        var fwd = right.Cross(up);
        var final_rot = new Basis(right, up, fwd);
        testObj.GlobalRotation = final_rot.GetRotationQuaternion().GetEuler();
        var transform = new Transform3D(
            final_rot,
            pos
        );
        // GD.Print(
        //     $"{(headBoneTail.GlobalPosition - headBoneHead.GlobalPosition).Normalized()}\n{headBoneHead.GlobalBasis.GetRotationQuaternion() * Vector3.Up}"
        // );
        // Transform3D.Identity
        //     .RotatedLocal(rot.GetAxis(), rot.GetAngle())
        //     .ScaledLocal(Vector3.One * spriteScale)
        //     .TranslatedLocal(pos);
        transform.Basis.Column0 = transform.Basis.Column0.Normalized() * Mathf.Pow(spriteScale, 2f);
        transform.Basis.Column1 = transform.Basis.Column1.Normalized() * spriteScale;
        transform.Basis.Column2 = transform.Basis.Column2.Normalized() * spriteScale;

        multimeshInstance.Multimesh.SetInstanceTransform(0, transform);
    }

}
