using Godot;
using System;

public class Main : Node
{
    [Export] public PackedScene Mob;

    private int _score;
    private Random _random = new Random();

    private float RandomRange(float min, float max) =>
        (float) _random.NextDouble() * (max - min) + min;

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<HUD>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame()
    {
        _score = 0;
        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();
        GetNode<AudioStreamPlayer>("Music").Play();

        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");
        
    }

    public void OnMobTimerTimeout()
    {
        var spawnLoc = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        spawnLoc.Offset = _random.Next();

        var mobInstance = (Mob) Mob.Instance();
        AddChild(mobInstance);

        var direction = spawnLoc.Rotation + Mathf.Pi / 2;
        mobInstance.Position = spawnLoc.Position;

        direction += RandomRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mobInstance.Rotation = direction;

        mobInstance.LinearVelocity =
            new Vector2(
                x: RandomRange(mobInstance.MinSpeed, mobInstance.MaxSpeed),
                y: 0).Rotated(direction);

        GetNode<HUD>("HUD").Connect("StartGame", mobInstance, "OnStartGame");
    }


    public void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    public void OnScoreTimerTimeout()
    {
        _score += 1;
        GetNode<HUD>("HUD").UpdateScore(_score);
    }
}