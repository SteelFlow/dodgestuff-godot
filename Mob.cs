using Godot;
using System;
using System.Runtime.InteropServices;

public class Mob : RigidBody2D
{
    [Export] public int MinSpeed = 150;
    [Export] public int MaxSpeed = 250;

    static private Random _random = new Random();
    private string[] _mobTypes = {"walk", "swim", "fly"};

    public override void _Ready()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").Animation = _mobTypes[_random.Next(0, _mobTypes.Length)];
    }

    public void OnVisibilityScreenExited()
    {
        QueueFree();
    }
    
    public void OnStartGame()
    {
        QueueFree();
    }
}