using Godot;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	[Export] private float MoveSpeed = 200f;
	[Export] public int Health = 100;
	[Export] public int Damage = 20;
	
	private AnimatedSprite2D _animatedSprite;
	private AnimatedSprite2D _swordSprite;
	private bool _isAttacking = false;
	private Vector2 _lastDirection = Vector2.Down;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_swordSprite = GetNode<AnimatedSprite2D>("SwordAnimatedSprite2D");
		_swordSprite.Hide();
		// A kard hitboxát alapból kikapcsoljuk
		GetNode<CollisionShape2D>("Hitbox/CollisionShape2D").Disabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isAttacking) return;
		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = inputDirection * MoveSpeed;
		if (inputDirection != Vector2.Zero) _lastDirection = inputDirection;
		MoveAndSlide();
		HandleAnimation();
		if (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left)) Attack();
	}

	private void HandleAnimation()
	{
		if (Velocity == Vector2.Zero) { _animatedSprite.Stop(); return; }
		string anim = (Mathf.Abs(_lastDirection.X) > Mathf.Abs(_lastDirection.Y)) 
			? (_lastDirection.X > 0 ? "walkright" : "walkleft") : (_lastDirection.Y > 0 ? "walkdown" : "walkup");
		_animatedSprite.Play(anim);
	}

	private async void Attack()
	{
		if (_isAttacking) return;
		_isAttacking = true;
		var hitboxShape = GetNode<CollisionShape2D>("Hitbox/CollisionShape2D");
		hitboxShape.Disabled = false;
		_swordSprite.Show();
		_swordSprite.Play(GetAttackAnimationName());
		await ToSignal(_swordSprite, AnimatedSprite2D.SignalName.AnimationFinished);
		hitboxShape.Disabled = true;
		_swordSprite.Hide();
		_isAttacking = false;
	}

	private string GetAttackAnimationName()
	{
		if (Mathf.Abs(_lastDirection.X) > Mathf.Abs(_lastDirection.Y))
			return _lastDirection.X > 0 ? "attack_right" : "attack_left";
		return _lastDirection.Y > 0 ? "attack_down" : "attack_up";
	}

	// A Játékos Hitboxát ehhez kösd a Node fülön!
	public void OnHitboxAreaEntered(Area2D area)
	{
		Node parent = area.GetParent();
		if (parent.HasMethod("TakeDamage")) {
			parent.Call("TakeDamage", Damage);
			GD.Print("Megütötted az ellenséget!");
		}
	}


	public void PlayerTakeDamage(int damage)
	{
		Health -= damage;
		GD.Print($"Játékos élete: {Health}");
		Modulate = Colors.Red;
		GetTree().CreateTimer(0.1f).Timeout += () => Modulate = Colors.White;
		if (Health <= 0) {GD.Print("Meghaltál!");
		var deathLabel= GetNode<Label>("CanvasLayer/Label") ;
		deathLabel.Visible= true;}
	}

	public void Gyogyul(int mennyiseg)
	{
		Health += mennyiseg;
		GD.Print("Bogyó felvéve! Új Életerő: " + Health);
	}


	public void SebzesNov(int novekedes)
	{
		Damage += novekedes;
		GD.Print("Láda kinyitva! Új sebzés: " + Damage);
	}
}
