namespace TextRPG.Entities;

public class Player(float health = 100)
{
    public string Name { get; set; } = string.Empty;
    public float Health { get; set; } = health;
    public float MaxHealth { get; set; } = health;
    public float Damage { get; set; } = 25;
    public int Potions  { get; set; } = 3;
}
