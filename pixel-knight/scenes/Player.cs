using Godot;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	[Export] private float MoveSpeed = 200f;

	private AnimatedSprite2D _animatedSprite;
	private AnimatedSprite2D _swordSprite;
	
	private bool _isAttacking = false;
	private Vector2 _lastDirection = Vector2.Down; // Tároljuk az utolsó irányt

	public bool HasKey = false;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_swordSprite = GetNode<AnimatedSprite2D>("SwordAnimatedSprite2D");
		
		// Kezdéskor rejtsük el a kardot
		_swordSprite.Hide();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Ha épp támad, ne tudjon mozogni (opcionális, játékstílustól függ)
		if (_isAttacking) return;

		HandleInput();
		MoveAndSlide();
		HandleAnimation();
	}

	private void HandleInput()
	{
		// Godot 4 beépített függvény az irányok lekérésére (kezelve a normalizálást)
		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		Velocity = inputDirection * MoveSpeed;

		// Frissítjük az utolsó irányt, ha van mozgás
		if (inputDirection != Vector2.Zero)
		{
			_lastDirection = inputDirection;
		}

		// Támadás indítása
		if (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Attack();
		}
	}

	private void HandleAnimation()
	{
		if (Velocity == Vector2.Zero)
		{
			_animatedSprite.Stop();
			return;
		}

		string anim = "walkdown";
		
		// Meghatározzuk, melyik irányba néz a karakter leginkább
		if (Mathf.Abs(_lastDirection.X) > Mathf.Abs(_lastDirection.Y))
			anim = _lastDirection.X > 0 ? "walkright" : "walkleft";
		else
			anim = _lastDirection.Y > 0 ? "walkdown" : "walkup";

		if (_animatedSprite.Animation != anim)
			_animatedSprite.Play(anim);
	}

	private async void Attack()
{
	if (_isAttacking) return;

	_isAttacking = true;
	_animatedSprite.Stop();

	string animName = GetAttackAnimationName();
	
	// --- ÚJ RÉSZ: Kapcsoljuk be a Hitboxot ---
	var hitboxShape = GetNode<CollisionShape2D>("Hitbox/CollisionShape2D");
	hitboxShape.Disabled = false; // Mostantól tud sebezni
	// -----------------------------------------

	_swordSprite.Show();
	_swordSprite.Play(animName);

	await ToSignal(_swordSprite, AnimatedSprite2D.SignalName.AnimationFinished);

	// --- ÚJ RÉSZ: Kapcsoljuk ki a Hitboxot ---
	hitboxShape.Disabled = true; // A támadás végén már ne sebezzen
	// -----------------------------------------

	_swordSprite.Hide();
	_isAttacking = false;
}

	private string GetAttackAnimationName()
	{
		if (Mathf.Abs(_lastDirection.X) > Mathf.Abs(_lastDirection.Y))
			return _lastDirection.X > 0 ? "attack_right" : "attack_left";
		
		return _lastDirection.Y > 0 ? "attack_down" : "attack_up";
	}
	
	
	public void OnHitboxAreaEntered(Area2D area)
{
	Node parent = area.GetParent();

	// 2. Ha az ellenségnek van 'TakeDamage' függvénye, hívjuk meg
	if (parent.HasMethod("TakeDamage"))
	{
		parent.Call("TakeDamage", 20); 
		GD.Print("Találat az ellenségen!");
		
		//  ütés hang
	}
}
	

	public void PickupKey()
	{
		HasKey = true;
		GD.Print("Kulcs felvéve!");
	}
}
