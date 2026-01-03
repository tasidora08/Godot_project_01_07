using Godot;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	[Export] private float MoveSpeed = 200f;

	private AnimatedSprite2D animatedsprite;
	private bool canAttack = true;

	public bool HasKey = false;

	public override void _Ready()
	{
		animatedsprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	private void HandleAnimation(Vector2 direction)
	{
		if (direction == Vector2.Zero)
		{
			animatedsprite.Stop();
			return;
		}

		string anim = "";

		if (direction.X != 0)
			anim = direction.X > 0 ? "walkright" : "walkleft";
		else if (direction.Y != 0)
			anim = direction.Y > 0 ? "walkdown" : "walkup";

		if (animatedsprite.Animation != anim)
			animatedsprite.Play(anim);
	}

	private void GetInput()
	{
		Vector2 inputDirection = Vector2.Zero;

		if (Input.IsActionPressed("ui_right"))
			inputDirection.X += 1;
		if (Input.IsActionPressed("ui_left"))
			inputDirection.X -= 1;
		if (Input.IsActionPressed("ui_up"))
			inputDirection.Y -= 1;
		if (Input.IsActionPressed("ui_down"))
			inputDirection.Y += 1;

		inputDirection = inputDirection.Normalized();
		Velocity = inputDirection * MoveSpeed;

		HandleAnimation(inputDirection);
	}

	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left) && canAttack)
		{
			Vector2 dir = GetFacingDirection();
			string animName = "attack_down";

			if (dir.X > 0)
				animName = "attack_right";
			else if (dir.X < 0)
				animName = "attack_left";
			else if (dir.Y < 0)
				animName = "attack_up";

			Attack(animName);
		}
	}

	private Vector2 GetFacingDirection()
{
	if (Velocity != Vector2.Zero)
		return Velocity.Normalized();

	return Vector2.Down;
}


	private async void Attack(string animName)
	{
		canAttack = false;

		var sword = GetNode<AnimatedSprite2D>("SwordAnimatedSprite2D");
		sword.Play(animName);

		while (sword.IsPlaying())
			await Task.Delay(10);

		canAttack = true;
	}

	public void PickupKey()
	{
		HasKey = true;
		GD.Print("player obijektum");
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();
	}
}
