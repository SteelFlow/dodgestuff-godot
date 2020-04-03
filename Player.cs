using Godot;
using System;
using System.Diagnostics.Contracts;
using System.IO;

public class Player : Area2D
{
    [Signal]
    public delegate void Hit();

    [Export] public int Speed = 400;

    private Vector2 _screenSize;

    private Vector2 _target;
    public override void _Ready()
    {
        _screenSize = GetViewport().Size;
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch eventMouseButton && eventMouseButton.Pressed)
        {
            _target = (@event as InputEventScreenTouch).Position;
        }
    }

    public override void _Process(float delta)
    {
        var velocity = GetVelocity();
        var animateSprite = GetNode<AnimatedSprite>("AnimatedSprite");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animateSprite.Play();
        }
        else
            animateSprite.Stop();

        Position += velocity * delta;
        Position = ClampToScreen(Position, _screenSize);

        if (velocity.x != 0)
        {
            animateSprite.Animation = "right";
            animateSprite.FlipV = false;

            animateSprite.FlipH = velocity.x < 0;
        }
        else if (velocity.y != 0)
        {
            animateSprite.Animation = "up";
            animateSprite.FlipV = velocity.y > 0;
        }
    }

    public void Start(Vector2 pos)
    {
        Position = pos;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public void OnPlayerBodyEntered(PhysicsBody2D body)
    {
        Hide();
        EmitSignal("Hit");
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }

    private Vector2 ClampToScreen(Vector2 position, Vector2 screenSize)
    {
        return new Vector2(
            x: Mathf.Clamp(position.x, 0, screenSize.x),
            y: Mathf.Clamp(position.y, 0, screenSize.y));
    }

    private Vector2 GetVelocity()
    {
        var velocity = new Vector2();

        if (Position.DistanceTo(_target) > 10)
        {
            velocity = (_target - Position).Normalized() * Speed;
        }
        else
        {
            velocity = new Vector2();
        }

        
        if (Input.IsActionPressed(InputKeys.Right))
            velocity.x += 1;
        if (Input.IsActionPressed(InputKeys.Left))
            velocity.x -= 1;
        if (Input.IsActionPressed(InputKeys.Down))
            velocity.y += 1;
        if (Input.IsActionPressed(InputKeys.Up))
            velocity.y -= 1;
        return velocity;
    }
}

public struct InputKeys
{
    public const string Right = "ui_right";
    public const string Left = "ui_left";
    public const string Up = "ui_up";
    public const string Down = "ui_down";
}