using Godot;
using System;

public partial class PlayTestAnimation : AnimationPlayer
{
    public override void _Ready()
    {
        this.Play(this.GetAnimationList()[0]);
    }
}
