using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public int Health = 100;

	public void TakeDamage(int damage)
	{
		Health -= damage;
		GD.Print($"Ellenség megütve! Maradék HP: {Health}");

		// Vizuális visszajelzés: villanjon meg az ellenség
		VisualFeedback();

		if (Health <= 0)
		{
			Die();
		}
	}

	private void VisualFeedback()
	{
		// Pirosra színezzük az ellenséget egy pillanatra
		Modulate = Colors.Red;
		GetTree().CreateTimer(0.1f).Timeout += () => Modulate = Colors.White;
	}

	private void Die()
	{
		
		QueueFree(); // Törli az ellenséget a játékból
	}
}
