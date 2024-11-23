using Godot;
using System;

public partial class PlayIdleAnimation : AnimationPlayer
{
    public override void _Ready()
    {
        this.Play("Idle");
    }
}
