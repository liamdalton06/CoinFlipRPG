using TextRPG.Entities;

namespace TextRPG.Combat;

public class CombatService
{
    private readonly Random _random = new();
    
    public float Attack(float damage, float receiverHealth)
    {
        receiverHealth -= damage;
        
        if (receiverHealth < 0)
        {
            return 0;
        }
        return receiverHealth;
    }

    public (int attack1, int attack2, int attack3, float multi, bool hit) TryAttack()
    {
        int attack1 = _random.Next(1, 3);
        int attack2 = _random.Next(1, 3);
        int attack3 = _random.Next(1, 3);

        var attackDmg = attack1 + attack2 + attack3;
        var hit = false;
        float multi = 0;

        switch (attackDmg)
        {
            case 6:
                multi = 1.6f;
                hit = true;
                break;
            case 5:
                multi = 0.6f;
                hit  = true;
                break;
            case 4:
            case 3:
                hit = false;
                break;
            default:
                Console.WriteLine("Error");
                break;
        }
        
       return (attack1, attack2, attack3, multi, hit);
    }
    
    public float Block(float damage, float receiverHealth)
    {
        receiverHealth -= damage / 3;
        
        if (receiverHealth < 0) 
        { 
            return 0;
        }
        return receiverHealth;
    }

    public float Heal(float health, float maxHealth)
    {
        float heal = health + 30;
        
        if (heal  > maxHealth)
        {
            return maxHealth;
        }
        
        return heal;
    }
}
