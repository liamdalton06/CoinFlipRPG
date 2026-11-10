using TextRPG.Entities;

namespace TextRPG.Combat;

public class CombatService
{
    private readonly Random _random = new();

    public int Attack(int damage, int receiverHealth)
    {
        receiverHealth -= damage;
        
        if (receiverHealth < 0)
        {
            return 0;
        }
        return receiverHealth;
    }

    public (int attack1, int attack2, bool hit) TryAttack()
    {
        int attack1 = _random.Next(1, 3);
        int attack2 = _random.Next(1, 3);
        
        bool hit = attack1 == attack2;
        return (attack1, attack2, hit);
    }
    
    public int Block(int damage, int receiverHealth)
    {
        receiverHealth -= damage / 3;
        
        if (receiverHealth < 0) 
        { 
            return 0;
        }
        return receiverHealth;
    }

    public int Heal(int health, int maxHealth)
    {
        int heal = health + 25;
        
        if (heal  > maxHealth)
        {
            return maxHealth;
        }
        
        return heal;
    }
}
