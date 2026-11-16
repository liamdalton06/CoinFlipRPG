namespace TextRPG;

public abstract class Enemy(float maxHealth, float damage, string name)
{
    public string Name { get; set; } = name;
    public float MaxHealth { get; set; } = maxHealth;
    public float Health { get; set; } = maxHealth;
    public float Damage { get; set; } = damage;
}