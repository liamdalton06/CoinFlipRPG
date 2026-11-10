namespace TextRPG.Entities;

public class Player(int health = 100)
{
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; } = health;
    public int MaxHealth { get; set; } = health;
    public int Damage { get; set; } = 25;
    public int Potions  { get; set; } = 3;
}
