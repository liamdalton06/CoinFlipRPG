namespace TextRPG.Entities;

public class Giant() : Enemy(150, 25, "Giant")
{
    private readonly Random _random = new();

   
   public (float Damage, bool heavyHit) HeavyAttackDmg()
   {
       var rand = _random.Next(1, 6);
       if (rand == 4)
       {
           var heavyDmg = Damage * 1.5f;
           return (heavyDmg, true);
       }

       return (Damage, false);
   }
   
}