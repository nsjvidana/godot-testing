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
            spineSprite.TopLevel = true;
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
        var spineGlobal = this.GlobalTransform * spineRelative;

        spineSprite.GlobalTransform = new Transform3D(
            new Basis(spineGlobal.Basis.GetRotationQuaternion()),
            spineGlobal.Origin
        );
        spineSprite.GlobalTransform.Scaled(spineSprite.GlobalTransform.Basis.Scale);
    }


}
