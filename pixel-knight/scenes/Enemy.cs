using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public int Health = 100;
	[Export] public int Damage = 10;

	// Ezt a függvényt hívja a Player
	public void TakeDamage(int damage)
	{
		Health -= damage;
		GD.Print($"Ellenség HP: {Health}");
		
		// Vizuális visszajelzés (piros villanás)
		Modulate = Colors.Red;
		GetTree().CreateTimer(0.1f).Timeout += () => Modulate = Colors.White;

		if (Health <= 0) {
			GD.Print("Az ellenség elpusztult!");
			QueueFree();
		}
	}
/////////////////
[Export] public float Speed = 50f;
[Export] public float PatrolDistance = 100f;

private Vector2 _startPosition;
private int _direction = 1; // 1 = Jobbra, -1 = Balra
private AnimatedSprite2D _animatedSprite;

public override void _Ready()
{
	_startPosition = GlobalPosition;
	_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	
	// Itt a korábbi kódod többi része (pl. hitbox elérése) maradhat
}

public override void _PhysicsProcess(double delta)
{
	// Mozgás kiszámítása
	Velocity = new Vector2(_direction * Speed, Velocity.Y);
	MoveAndSlide();

	UpdateAnimation();

	float distanceFromStart = GlobalPosition.X - _startPosition.X;
	if (Mathf.Abs(distanceFromStart) >= PatrolDistance || IsOnWall())
	{
		_direction *= -1;
	}
}

private void UpdateAnimation()
{
	string animNev;

	// Meghatározzuk melyik animáció kell az irány alapján
	if (_direction > 0)
	{
		animNev = "walk_right";
	}
	else
	{
		animNev = "walk_left";
	}

	// CSAK AKKOR indítjuk el, ha nem ez fut éppen!
	// Ez a legfontosabb sor, ettől fog lejátszódni az animáció.
	if (_animatedSprite.Animation != animNev)
	{
		_animatedSprite.Play(animNev);
	}
}


/////////////////
	// Az Enemy Hitboxát ehhez kösd a Node fülön!
	public void OnEnemyHitboxAreaEntered(Area2D area)
	{
		Node parent = area.GetParent();
		// Csak akkor sebez, ha tényleg a Playert érte el
		if (parent is Player player) {
			player.PlayerTakeDamage(Damage);
			GD.Print("Az ellenség megütött!");
		}
	}
}
