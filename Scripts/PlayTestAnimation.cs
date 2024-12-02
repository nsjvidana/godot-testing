using Godot;
using System;

public partial class PlayTestAnimation : AnimationPlayer
{
    [Export]
    public int animationIndex = 0;
    public override void _Ready()
    {
        this.Play(this.GetAnimationList()[animationIndex]);
    }
}
