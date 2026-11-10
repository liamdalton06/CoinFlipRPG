namespace TextRPG.Entities;

public class Monster(int health = 100)
{
    public int MaxHealth { get; set; } = health;
    public int Health { get; set; } = health;
    public int Damage { get; set; } = 15;
}
