using Godot;
using System;
using System.Linq;

public partial class SpriteArmature : Node3D
{
    [Export]
    public PackedScene spriteMultimeshScene;
    [Export]
    public Texture2D spritesheet;
    [Export]
    public int spriteSize = 16;
    [Export]
    public Node3D testObj;
    [Export]
    public float partOffset = 0.015f;

    BoneAttachment3D headBoneHead = new();
    BoneAttachment3D headBoneTail = new();
    BoneAttachment3D spineBoneHead = new();
    BoneAttachment3D spineBoneTail = new();
    
    BoneAttachment3D[] upperLegBoneHeads = {new(), new()};
    BoneAttachment3D[] upperLegBoneTails = {new(), new()};
    BoneAttachment3D[] lowerLegBoneHeads = {new(), new()};
    BoneAttachment3D[] lowerLegBoneTails = {new(), new()};

    BoneAttachment3D[] upperArmBoneHeads = {new(), new()};
    BoneAttachment3D[] upperArmBoneTails = {new(), new()};
    BoneAttachment3D[] lowerArmBoneHeads = {new(), new()};
    BoneAttachment3D[] lowerArmBoneTails = {new(), new()};
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
			shader.SetShaderParameter("sprite_size", spriteSize);
		}
        multimeshInstance.Multimesh.Mesh.SurfaceSetMaterial(0, mat);
        multimeshInstance.Multimesh.InstanceCount = 10;

        int idx = 0;
        int headIdx = -1;
        int torsoIdx = -1;
        int[] upperLegIdxs = {-1, -1};
        int[] lowerLegIdxs = {-1, -1};
        int[] upperArmIdxs = {-1, -1};
        int[] lowerArmIdxs = {-1, -1};
        int boneCount = skeleton.GetBoneCount();
        for(int i = 0; i < boneCount; i++) {
            var boneName = skeleton.GetBoneName(i);
            if(headIdx == -1 && boneName.StartsWith("Head"))
                headIdx = idx;
            else if(torsoIdx == -1 && boneName.StartsWith("Spine"))
                torsoIdx = idx;
            else if(upperLegIdxs[0] == -1 && boneName.Equals("UpperLeg.L"))
                upperLegIdxs[0] = idx;
            else if(upperLegIdxs[1] == -1 && boneName.Equals("UpperLeg.R"))
                upperLegIdxs[1] = idx;
            else if(lowerLegIdxs[0] == -1 && boneName.Equals("LowerLeg.L"))
                lowerLegIdxs[0] = idx;
            else if(lowerLegIdxs[1] == -1 && boneName.Equals("LowerLeg.R"))
                lowerLegIdxs[1] = idx;
            else if(upperArmIdxs[0] == -1 && boneName.Equals("UpperArm.L"))
                upperArmIdxs[0] = idx;
            else if(upperArmIdxs[1] == -1 && boneName.Equals("UpperArm.R"))
                upperArmIdxs[1] = idx;
            else if(lowerArmIdxs[0] == -1 && boneName.Equals("LowerArm.L"))
                lowerArmIdxs[0] = idx;
            else if(lowerArmIdxs[1] == -1 && boneName.Equals("LowerArm.R"))
                lowerArmIdxs[1] = idx;
            idx++;
        }
        
        #region error checking
        if(headIdx == -1) {
            GD.PrintErr("Couldn't find Head of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(torsoIdx == -1) {
            GD.PrintErr("Couldn't find Spine of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(upperLegIdxs[0] == -1) {
            GD.PrintErr("Couldn't find UpperLeg.L of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(upperLegIdxs[1] == -1) {
            GD.PrintErr("Couldn't find UpperLeg.R of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(lowerLegIdxs[0] == -1) {
            GD.PrintErr("Couldn't find LowerLeg.L of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(lowerLegIdxs[1] == -1) {
            GD.PrintErr("Couldn't find LowerLeg.R of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(upperArmIdxs[0] == -1) {
            GD.PrintErr("Couldn't find UpperArm.L of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(upperArmIdxs[1] == -1) {
            GD.PrintErr("Couldn't find UpperArm.R of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(lowerArmIdxs[0] == -1) {
            GD.PrintErr("Couldn't find LowerArm.L of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        if(lowerArmIdxs[1] == -1) {
            GD.PrintErr("Couldn't find LowerArm.R of skeleton node " + this.Name);
            this.QueueFree();
            return;
        }
        #endregion

        #region setting bone attachments
        //head
        skeleton.AddChild(headBoneHead);
        skeleton.AddChild(headBoneTail);
            headBoneHead.BoneIdx = headIdx;
            headBoneTail.BoneIdx = headIdx+1;
        //torso
        skeleton.AddChild(spineBoneHead);
        skeleton.AddChild(spineBoneTail);
            spineBoneHead.BoneIdx = torsoIdx;
            spineBoneTail.BoneIdx = torsoIdx+1;
        //upper legs
        skeleton.AddChild(upperLegBoneHeads[0]);
        skeleton.AddChild(upperLegBoneTails[0]);
            upperLegBoneHeads[0].BoneIdx = upperLegIdxs[0];
            upperLegBoneTails[0].BoneIdx = upperLegIdxs[0]+1;
        skeleton.AddChild(upperLegBoneHeads[1]);
        skeleton.AddChild(upperLegBoneTails[1]);
            upperLegBoneHeads[1].BoneIdx = upperLegIdxs[1];
            upperLegBoneTails[1].BoneIdx = upperLegIdxs[1]+1;
        //lower legs
        skeleton.AddChild(lowerLegBoneHeads[0]);
        skeleton.AddChild(lowerLegBoneTails[0]);
            lowerLegBoneHeads[0].BoneIdx = lowerLegIdxs[0];
            lowerLegBoneTails[0].BoneIdx = lowerLegIdxs[0]+1;
        skeleton.AddChild(lowerLegBoneHeads[1]);
        skeleton.AddChild(lowerLegBoneTails[1]);
            lowerLegBoneHeads[1].BoneIdx = lowerLegIdxs[1];
            lowerLegBoneTails[1].BoneIdx = lowerLegIdxs[1]+1;

        //upper Arms
        skeleton.AddChild(upperArmBoneHeads[0]);
        skeleton.AddChild(upperArmBoneTails[0]);
            upperArmBoneHeads[0].BoneIdx = upperArmIdxs[0];
            upperArmBoneTails[0].BoneIdx = upperArmIdxs[0]+1;
        skeleton.AddChild(upperArmBoneHeads[1]);
        skeleton.AddChild(upperArmBoneTails[1]);
            upperArmBoneHeads[1].BoneIdx = upperArmIdxs[1];
            upperArmBoneTails[1].BoneIdx = upperArmIdxs[1]+1;
        //lower Arms
        skeleton.AddChild(lowerArmBoneHeads[0]);
        skeleton.AddChild(lowerArmBoneTails[0]);
            lowerArmBoneHeads[0].BoneIdx = lowerArmIdxs[0];
            lowerArmBoneTails[0].BoneIdx = lowerArmIdxs[0]+1;
        skeleton.AddChild(lowerArmBoneHeads[1]);
        skeleton.AddChild(lowerArmBoneTails[1]);
            lowerArmBoneHeads[1].BoneIdx = lowerArmIdxs[1];
            lowerArmBoneTails[1].BoneIdx = lowerArmIdxs[1]+1;
        #endregion

        #region setting part offsets
            multimeshInstance.Multimesh.SetInstanceCustomData(3, new Color(2,0,0));
        #endregion
    }

    public override void _Process(double delta)
    {
        multimeshInstance.Multimesh.SetInstanceTransform(0, CalculateSpriteTransform(headBoneHead, headBoneTail, 0).Translated(new Vector3(0, 0, -0.01f)));
            multimeshInstance.Multimesh.SetInstanceCustomData(0, new Color(0,0,0));
        multimeshInstance.Multimesh.SetInstanceTransform(1, CalculateSpriteTransform(spineBoneHead, spineBoneTail, 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(1, new Color(1,0,0));

        //legs
        multimeshInstance.Multimesh.SetInstanceTransform(2, CalculateSpriteTransform(upperLegBoneHeads[0], upperLegBoneTails[0], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(2, new Color(2,partOffset,0));
        multimeshInstance.Multimesh.SetInstanceTransform(3, CalculateSpriteTransform(upperLegBoneHeads[1], upperLegBoneTails[1], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(3, new Color(2,-partOffset,0));

        multimeshInstance.Multimesh.SetInstanceTransform(4, CalculateSpriteTransform(lowerLegBoneHeads[0], lowerLegBoneTails[0], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(4, new Color(3,partOffset*2,0));
        multimeshInstance.Multimesh.SetInstanceTransform(5, CalculateSpriteTransform(lowerLegBoneHeads[1], lowerLegBoneTails[1], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(5, new Color(3,0,0));

        //arms
        multimeshInstance.Multimesh.SetInstanceTransform(6, CalculateSpriteTransform(upperArmBoneHeads[0], upperArmBoneTails[0], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(6, new Color(2,partOffset,0));
        multimeshInstance.Multimesh.SetInstanceTransform(7, CalculateSpriteTransform(upperArmBoneHeads[1], upperArmBoneTails[1], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(7, new Color(2,-partOffset,0));

        multimeshInstance.Multimesh.SetInstanceTransform(8, CalculateSpriteTransform(lowerArmBoneHeads[0], lowerArmBoneTails[0], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(8, new Color(3,partOffset*2,0));
        multimeshInstance.Multimesh.SetInstanceTransform(9, CalculateSpriteTransform(lowerArmBoneHeads[1], lowerArmBoneTails[1], 0));
            multimeshInstance.Multimesh.SetInstanceCustomData(9, new Color(3,0,0));
    }

    Transform3D CalculateSpriteTransform(BoneAttachment3D boneHead, BoneAttachment3D boneTail, float scale) {
        var boneDir = boneTail.GlobalPosition - boneHead.GlobalPosition;

        var pos = multimeshInstance.ToLocal((boneHead.GlobalPosition + boneTail.GlobalPosition)/2);
        var euler = new Vector3(boneHead.GlobalRotation.Z, boneHead.GlobalRotation.Y, boneHead.GlobalRotation.X);
        var rot = Quaternion.FromEuler(euler);
        var up_dir = Mathf.Sign((boneHead.Transform.Basis.GetRotationQuaternion() * Vector3.Up).Y);
        var up = boneDir.Normalized() * (up_dir == 0 ? 1f:up_dir);
        var right = up.Cross(rot * Vector3.Forward);
        var fwd = right.Cross(up);
        
        scale = boneDir.Length()*1.1f;
        var transform = new Transform3D(
            new Basis(right, up, fwd).Scaled(new Vector3(scale, scale, scale)),
            pos
        );
        return transform;
    }

}
