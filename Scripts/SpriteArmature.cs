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
    public int numParts = 2;
    [Export]
    public float headScale = 1f;
    [Export]
    public float torsoScale = 1f;
    [Export]
    public Node3D testObj;

    BoneAttachment3D headBoneHead;
    BoneAttachment3D headBoneTail;
    BoneAttachment3D spineBoneHead;
    BoneAttachment3D spineBoneTail;
    MultiMeshInstance3D multimeshInstance;
    Skeleton3D skeleton;

    public override void _Ready()
    {
        if(spriteMultimeshScene == null) {
            GD.PrintErr("Sprite Multimesh Scene is missing for node " + this.Name + "!");
            this.QueueFree();
            return;
        }
        if(spritesheet == null) {
            GD.PrintErr("Spritesheet is missing for node " + this.Name + "!");
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
			shader.SetShaderParameter("num_parts", numParts);
		}
        multimeshInstance.Multimesh.Mesh.SurfaceSetMaterial(0, mat);
        multimeshInstance.Multimesh.InstanceCount = 2;

        int idx = 0;
        int headIdx = -1;
        int torsoIdx = -1;
        int boneCount = skeleton.GetBoneCount();
        for(int i = 0; i < boneCount; i++) {
            var boneName = skeleton.GetBoneName(i);
            if(headIdx == -1 && boneName.StartsWith("Head"))
                headIdx = idx;
            else if(torsoIdx == -1 && boneName.StartsWith("Spine"))
                torsoIdx = idx;
            idx++;
        }

        if(headIdx == -1) {
            GD.PrintErr("Couldn't find head of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(torsoIdx == -1) {
            GD.PrintErr("Couldn't find spine of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }

        //setting bone attachments
        headBoneHead = new();
        headBoneTail = new();
        spineBoneHead = new();
        spineBoneTail = new();
        skeleton.AddChild(headBoneHead);
            headBoneHead.BoneIdx = headIdx;
        skeleton.AddChild(headBoneTail);
            headBoneTail.BoneIdx = headIdx+1;
        skeleton.AddChild(spineBoneHead);
            spineBoneHead.BoneIdx = torsoIdx;
        skeleton.AddChild(spineBoneTail);
            spineBoneTail.BoneIdx = torsoIdx+1;
    }

    public override void _Process(double delta)
    {
        multimeshInstance.Multimesh.SetInstanceTransform(0, CalculateSpriteTransform(headBoneHead, headBoneTail, headScale).Translated(new Vector3(0, 0, -0.01f)));
        multimeshInstance.Multimesh.SetInstanceTransform(1, CalculateSpriteTransform(spineBoneHead, spineBoneTail, torsoScale));
        GD.Print(multimeshInstance.Multimesh.GetInstanceTransform(0).Origin);
        GD.Print(multimeshInstance.Multimesh.GetInstanceTransform(1).Origin);
    }

    Transform3D CalculateSpriteTransform(BoneAttachment3D boneHead, BoneAttachment3D boneTail, float scale) {
        var pos = multimeshInstance.ToLocal((boneHead.GlobalPosition + boneTail.GlobalPosition)/2);
        var euler = new Vector3(boneTail.GlobalRotation.Z, boneTail.GlobalRotation.Y, boneTail.GlobalRotation.X);
        var rot = Quaternion.FromEuler(euler);
        var up = (boneTail.GlobalPosition - boneHead.GlobalPosition).Normalized();
        var right = up.Cross(rot * Vector3.Forward);
        var fwd = right.Cross(up);
        var final_rot = new Basis(right, up, fwd);
        var transform = new Transform3D(
            final_rot,
            pos
        );
        transform.Basis.Column0 = transform.Basis.Column0.Normalized() * Mathf.Pow(scale, 2f);
        transform.Basis.Column1 = transform.Basis.Column1.Normalized() * scale;
        transform.Basis.Column2 = transform.Basis.Column2.Normalized() * scale;
        return transform;
    }

}
