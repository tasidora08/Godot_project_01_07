using Godot;
using System;
using DialogueManagerRuntime;

public partial class Kiraly : CharacterBody2D
{
	// Ez a változó fog megjelenni az Inspector legalján sikeres Build után
	[Export] public Resource DialogueFile;

	public override void _Ready()
	{
		// Hibakeresés: Ha elfelejtetted behúzni a fájlt az Inspectorban
		if (DialogueFile == null)
		{
			GD.PrintErr("HIBA: A DialogueFile nincs beállítva a Király Inspectorában!");
		}
	}

	// FONTOS: A szignál bekötésekor pontosan ezt a nevet válaszd!
	public void _on_area_2d_body_entered(Node2D body)
	{
		// Teszt üzenet a konzolra
		GD.Print("Érzékeltem valakit: " + body.Name);

		// Csak akkor indul el, ha a Player lép be
		if (body.Name == "Player" || body.Name == "player")
		{
			GD.Print("A Játékos belépett! Dialógus indítása...");
			DialogueManager.ShowExampleDialogueBalloon(DialogueFile, "start");

			// Az Area2D törlése, hogy ne induljon el újra és újra
			GetNode<Area2D>("Area2D").QueueFree();
		}
	}
}
