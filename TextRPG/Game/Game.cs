using TextRPG.Art;
using TextRPG.Combat;
using TextRPG.Entities;
using System.Text.Json;

namespace TextRPG.Game;

public class Game
{
    private readonly CombatService _combat = new();

    public void Run()
    {
        GameStats stats = new GameStats(); // Searches JSON for player stats
        
        if (File.Exists("stats.json"))
        {
            var json = File.ReadAllText("stats.json");
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            stats = JsonSerializer.Deserialize<GameStats>(json, options)
                    ?? new GameStats();
        }

        List<Enemy> enemies = [];
        Console.WriteLine("Welcome to TextRPG!");
        
        Ascii.PlayerAvatar(); // GRAPHICS
        var player = CreatePlayer();

        Console.WriteLine("\nWelcome to the dungeon");
        Console.WriteLine($"\n{player.Name}, you will now fight your first enemy...\n" +
                          "Good luck (ENTER to continue):)");
        Console.ReadLine();
        Console.WriteLine(".\n.\n.\n.\n.\n----------");
        
        outer:while (true)
        {
            EnemyEncounter(player, new Monster(), enemies);
            if (player.Health <= 0)
            {
                stats.SaveShowStats(enemies);
                break;
            }
            EnemyEncounter(player, new Giant(), enemies);
            if (player.Health <= 0)
            {
                stats.SaveShowStats(enemies);
                break;
            }
           
            var win = player.Health > 0;
            stats.SaveShowStats(enemies);
            EndScreen(win);
           
            string? choice;
            do // Does player want to play again loop
            {
                Console.WriteLine("Play again? yes or no");
                choice = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;

                while (string.IsNullOrWhiteSpace(choice))
                {
                    Console.WriteLine("Yes or no");
                    choice = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;
                }
                
                inner:switch (choice)
                {
                    case "yes": // Reset stats & current progress
                        player.Potions = 3;
                        player.Damage = 25;
                        player.MaxHealth = 100;
                        player.Health = player.MaxHealth;
                        player.HealMulti = 1;
                        enemies.Clear();
                        break inner;
                    case "no":
                        break outer;
                    default:
                        Console.WriteLine("Only yes or no");
                        continue;
                }
            } 
            while (choice is not ("yes" or "no"));
        }
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
                case "attack": // Display & help calculate attacks from CombatService
                    var flip = _combat.TryAttack();
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
                        Console.WriteLine($"hit {enemy.Name}: {enemy.Health:0.##}HP");
                    }
                    else
                    {
                        Console.WriteLine($"missed {enemy.Name}, {enemy.Health:0.##}HP");
                    }

                    Console.ReadLine();
                    Console.WriteLine(".\n.\n.\n.");
                    return false;

                case "block": // Decrease incoming damage through CombatService
                    Console.WriteLine($"{player.Name} prepares to block.");
                    return true;

                case "heal":

                    if (player.Potions >= 1)
                    {
                        player.Health = _combat.Heal(player.Health, player.MaxHealth, player.HealMulti);
                        player.Potions--;
                        Console.WriteLine($"{player.Name} healed to {player.Health:0.##}HP, {player.Potions} potions left");
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
        var sum = flip.attack3 + flip.attack2 + flip.attack1;

        switch (sum)
        {
            case 6:
                Console.Write($"{enemy.Name} flipped tails 3x, ");
                break;
            case 5:
                Console.Write($"{enemy.Name} flipped tails 2x and heads once, ");
                break;
            case 4:
                Console.Write($"{enemy.Name} flipped heads 2x and tails once, ");
                break;
            case 3:
                Console.Write($"{enemy.Name} flipped heads 3x, ");
                break;
        }
        
        if (flip.hit)
        {
            float dmg = flip.multi * enemy.Damage;
            if (enemy is Giant giant && sum != 6)
            {
                var result = giant.HeavyAttackDmg();
                if (result.heavyHit)
                {
                    dmg = result.Damage;
                    Console.WriteLine($"\nis preparing a HEAVY attack...");
                    Console.ReadLine();
                }
            }
            
            player.Health = isBlocked
                ? _combat.Block(dmg, player.Health)
                : _combat.Attack(dmg, player.Health);
            
            Console.WriteLine($"hit {player.Name}, {player.Health:0.##}HP");
        }
        else
        {
            Console.WriteLine($"missed {player.Name}: {player.Health:0.##}HP");
        }
        Console.ReadLine();
        Console.WriteLine(".\n.\n.\n."); 
    }

    private void EnemyEncounter(Player player, Enemy enemy, List<Enemy> enemies)
    {
        Console.WriteLine($"{enemy.Name} appeared!");
        
        switch (enemy)
        {
            case Monster:
                Ascii.MonsterAvatar(); // GRAPHICS
                break;
            case Giant:
                Ascii.GiantAvatar(); // GRAPHICS
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
            Console.Write($"{player.Name} wins the battle, {player.Health:0.##}HP | ");
            enemies.Add(enemy);
            player.Potions++;
            player.Damage *= 1.1f;
            player.MaxHealth *= 1.1f;
            player.HealMulti *= 1.1f;
            
            Console.WriteLine("Stats+ 10% | Potions +1");
        }
        else
        {
            Console.WriteLine($"{player.Name} was defeated by {enemy.Name}");
        }
    }

    private void EndScreen(bool win)
    {
        Console.WriteLine("----------");

        Console.WriteLine(win ? "You won!" : "You lost!");

        Console.WriteLine("----------");
    }

    private void BattleUi(Player player, Enemy enemy)
    {
        Console.WriteLine("\n------------------------------------------\n" +
                          $"{player.Health:0.##}/{player.MaxHealth:0.##}HP | {player.Potions} potions" +
                          $" | {enemy.Name}: {enemy.Health:0.##}/{enemy.MaxHealth:0.##}" +
                          $"HP\n------------------------------------------" +
                          "\nType an action: 'attack' -- 'block' -- 'heal' -- 'pass'");
    }
}