
using Godot;
using System;

public partial class Bogyo : Area2D
{
	[Export] public int Gyogyitas = 20;
	private bool playerNearby = false;
	private Player playerRef;

	public override void _Process(double delta)
	{
		// Ha ott vagy a bogyónál ÉS megnyomod az E betűt (vagy Space-t)
		if (playerNearby && Input.IsActionJustPressed("ui_accept"))
		{
			Felvetel();
		}
	}

	private void Felvetel()
	{
		playerRef.Gyogyul(Gyogyitas);
		QueueFree(); // A bogyó eltűnik
	}

	// Ezeket a szignálokat kösd be a Node fülön!
	public void _on_body_entered(Node2D body)
	{
		if (body is Player p) {
			playerNearby = true;
			playerRef = p;
		}
	}

	public void _on_body_exited(Node2D body)
	{
		if (body is Player) {
			playerNearby = false;
			playerRef = null;
		}
	}
}
