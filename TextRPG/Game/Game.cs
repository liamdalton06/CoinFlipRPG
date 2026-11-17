using TextRPG.Art;
using TextRPG.Combat;
using TextRPG.Entities;

namespace TextRPG.Game;

public class Game
{
    private readonly CombatService _combat = new();
    private readonly ASCII _ascii = new();


    public void Run()
    {
        List<Enemy> enemies = [];
        Console.WriteLine("Welcome to TextRPG!");
        
        ASCII.PlayerAvatar(); // GRAFIK
        Player player = CreatePlayer();
        
        bool win = false;
        
        Console.WriteLine("\nWelcome to the dungeon");
        Console.WriteLine($"\n{player.Name}, you will now fight your first enemy...\n" +
                          "Good luck (ENTER to continue):)");
        Console.ReadLine();
        Console.WriteLine(".\n.\n.\n.\n.\n----------");

        for (var i = 0; i < 1; i++)
        {
            EnemyEncounter(player, new Monster(), win, enemies);
            if (player.Health <= 0)
            {
                break;
            }
            EnemyEncounter(player, new Giant(), win, enemies);
        }
        Console.WriteLine($"{enemies.Count} enemies defeated");
    }

    private static Player CreatePlayer()
    {
        Console.WriteLine("\nWhat is your name?");
        string name = Console.ReadLine()?.Trim() ?? string.Empty;

        while (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("I said 'your name'..");
            name = Console.ReadLine()?.Trim() ?? string.Empty;
        }

        return new Player { Name = name };
    }

    private bool PlayerTurn(Player player, Enemy enemy)
    {
        BattleUi(player, enemy);

        while (true) 
        {
            string action = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;
            Console.WriteLine("\n.\n.");

            switch (action)
            {
                case "attack":
                    var flip = _combat.TryAttack();

                    var flipText1 = flip.attack1 == 1 ? "heads" : "tails";
                    var flipText2 = flip.attack2 == 1 ? "heads" : "tails";
                    var flipText3 = flip.attack3 == 1 ? "heads" : "tails";
                    var sum = flip.attack3 + flip.attack2 + flip.attack1;

                    switch (sum)
                    {
                        case 6:
                            Console.Write($"{player.Name} flipped tails 3x, ");
                            break;
                        case 5:
                            Console.Write($"{player.Name} flipped tails 2x and heads once, ");
                            break;
                        case 4:
                            Console.Write($"{player.Name} flipped heads 2x and tails once, ");
                            break;
                        case 3:
                            Console.Write($"{player.Name} flipped heads 3x, ");
                            break;
                    }
                    
                    if (flip.hit)
                    {
                        enemy.Health = _combat.Attack(player.Damage * flip.multi, enemy.Health);
                        Console.WriteLine($"hit {enemy.Name}: {enemy.Health}HP");
                    }
                    else
                    {
                        Console.WriteLine($"missed {enemy.Name}, {enemy.Health}HP");
                    }

                    Console.ReadLine();
                    Console.WriteLine(".\n.\n.\n.");
                    return false;

                case "block":
                    Console.WriteLine($"{player.Name} prepares to block.");
                    return true;

                case "heal":

                    if (player.Potions >= 1)
                    {
                        player.Health = _combat.Heal(player.Health, player.MaxHealth);
                        player.Potions--;
                        Console.WriteLine($"{player.Name} healed to {player.Health}HP, {player.Potions} potions left");
                    }
                    else
                    {
                        Console.WriteLine("Out of potions");
                    }
                    return false;

                case "pass":
                    Console.WriteLine($"{player.Name} passed their turn");
                    return false;

                default:
                    Console.WriteLine("That is not a valid action.");
                    break;
            }
        }
    }

    private void EnemyTurn(Player player, Enemy enemy, bool isBlocked)
    {
        var flip = _combat.TryAttack();
        var flipText1 = flip.attack1 == 1 ? "heads" : "tails";

        var flipText2 = flip.attack2 == 1 ? "heads" : "tails";

        Console.Write(flip.attack1 == flip.attack2
            ? $"{enemy.Name} flipped {flipText1} twice and "
            : $"{enemy.Name} flipped {flipText1} and {flipText2}, ");
        if (flip.hit)
        {
            float dmg = enemy.Damage;
            if (enemy is Giant giant)
            {
                var result = giant.HeavyAttackDmg();
                if (result.heavyHit)
                {
                    dmg = result.Damage;
                    Console.WriteLine($"\nis preparing a HEAVY attack...");
                }
            }
            
            player.Health = isBlocked
                ? _combat.Block(dmg, player.Health)
                : _combat.Attack(dmg, player.Health);
            
            Console.WriteLine($"hit {player.Name}, {player.Health}HP");
        }
        else
        {
            Console.WriteLine($"missed {player.Name}: {player.Health}HP");
        }
        Console.ReadLine();
        Console.WriteLine(".\n.\n.\n."); 
    }

    private void EnemyEncounter(Player player, Enemy enemy, bool win, List<Enemy> enemies)
    {
        Console.WriteLine($"{enemy.Name} appeared!");
        
        switch (enemy)
        {
            case Monster:
                ASCII.MonsterAvatar(); // GRAFIK
                break;
            case Giant:
                ASCII.GiantAvatar(); // GRAFIK
                break;
        }

        while (enemy.Health > 0 && player.Health > 0)
        {
            bool isBlocked = PlayerTurn(player, enemy);
            if (enemy.Health <= 0 || player.Health <= 0)
            {
                break;
            }
            EnemyTurn(player, enemy, isBlocked);
        }
        if (player.Health > enemy.Health)
        {
            Console.WriteLine($"{player.Name} wins the battle, {player.Health}HP");
            enemies.Add(enemy);
            win = true;
        }
        else
        {
            Console.WriteLine($"{player.Name} was defeated by {enemy.Name}");
            win = false;
        }
        EndScreen(win);
    }

    private void EndScreen(bool win)
    {
        Console.WriteLine("----------");
        
        if (win)
        {
            Console.WriteLine("You won!");
        }
        else
        {
            Console.WriteLine("You lost!");
        }
        
        Console.WriteLine("----------");
    }

    private void BattleUi(Player player, Enemy enemy)
    {
        Console.WriteLine("\n------------------------------------------\n" +
                          $"{player.Health}/{player.MaxHealth}HP | {player.Potions} potions" +
                          $" | {enemy.Name}: {enemy.Health}/{enemy.MaxHealth}" +
                          $"HP\n------------------------------------------" +
                          "\nType an action: 'attack' -- 'block' -- 'heal' -- 'pass'");
    }
}