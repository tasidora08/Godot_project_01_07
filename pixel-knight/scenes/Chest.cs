using Godot;
using System;

public partial class Chest : Area2D
{
	[Export] public int sebzesNoveles = 20;
	private bool playerNearby = false;
	private Player playerRef;

	public override void _Process(double delta)
	{

		if (playerNearby && Input.IsActionJustPressed("ui_select"))
		{
			Felvetel();
		}
	}

	private void Felvetel()
	{
		playerRef.SebzesNov(sebzesNoveles);
		QueueFree(); 
	}


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
