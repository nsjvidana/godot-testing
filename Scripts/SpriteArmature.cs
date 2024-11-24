using Godot;
using System;

public partial class SpriteArmature : Skeleton3D
{
    
    [Export]
    public PackedScene sprite3dScene;
    [Export]
    public Texture2D spriteTexture;

    int spineIdx = -1;
    Sprite3D spineSprite;

    public override void _Ready()
    {
        if(sprite3dScene == null) {
            GD.PrintErr("Sprite3D Scene is missing for node " + this + "!");
            this.QueueFree();
            return;
        }
        if(spriteTexture == null) {
            GD.PrintErr("Sprite Texture is missing for node " + this + "!");
            this.QueueFree();
            return;
        }

        var sceneInstance = sprite3dScene.Instantiate();
            AddChild(sceneInstance);
        spineSprite = sceneInstance.GetNode<Sprite3D>("Sprite3D");
        spineSprite.Texture = spriteTexture;

        var boneNames = this.GetConcatenatedBoneNames().ToString().Split(',');
        int idx = 0;
        foreach(var boneName in boneNames) {
            if(boneName.EndsWith("Spine")) {
                spineIdx = idx;
                break;
            }
            idx++;
        }

        if(spineIdx == -1) {
            GD.PrintErr("Couldn't find spine for skeleton node " + this);
            this.QueueFree();
            return;
        }
    }

    public override void _Process(double delta)
    {
        var spineRelative = GetBoneGlobalPose(spineIdx);
        var newSpineRot = this.GlobalTransform.Basis.GetRotationQuaternion() * spineRelative.Basis.GetRotationQuaternion();
        var newSpineOrigin = this.GlobalTransform.Origin + spineRelative.Origin;

        var currScale = this.GlobalTransform.Basis.Scale;
        var invScale = new Vector3(1f/currScale.X, 1f/currScale.Y, 1f/currScale.Z);
        spineSprite.GlobalTransform = new Transform3D(
            new Basis(newSpineRot),
            newSpineOrigin
        );
        // spineSprite.Scale = spineSprite.Transform.Basis.Scale;
        GD.Print(spineSprite.GlobalTransform.Basis.Scale);

    }


}
