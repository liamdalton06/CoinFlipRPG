using TextRPG.Combat;
using TextRPG.Entities;

namespace TextRPG.Game;

public class Game
{
    private readonly CombatService _combat = new();

    public void Run()
    {
        Console.WriteLine("Welcome to TextRPG!");

        Player player = CreatePlayer();
        
        bool win = false;

        Console.WriteLine("\nWelcome to the dungeon");
        Console.WriteLine($"\n{player.Name}, you will now fight your first monster...\n" +
                          "Good luck (ENTER to continue):)");
        Console.ReadLine();
        Console.WriteLine(".\n.\n.\n.\n.\n----------");

        while (player.Health > 0)
        {
            EnemyEncounter(player, win);
        }
        Console.WriteLine("GAME OVER");
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

    private bool PlayerTurn(Player player, Monster monster)
    {
        BattleUI(player, monster);

        while (true) 
        {
            string action = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;
            Console.WriteLine("\n.\n.");

            switch (action)
            {
                case "attack":
                    var flip = _combat.TryAttack();

                    Console.Write($"{player.Name} flipped {flip.attack1} and {flip.attack2}, ");

                    if (flip.hit)
                    {
                        monster.Health = _combat.Attack(player.Damage, monster.Health);
                        Console.WriteLine($"hit monster: {monster.Health}HP");
                    }
                    else
                    {
                        Console.WriteLine($"missed monster, {monster.Health}HP");
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

    private void EnemyTurn(Player player, Monster monster, bool isBlocked)
    {
        var flip = _combat.TryAttack();

        Console.Write($"Monster flipped {flip.attack1} and {flip.attack2}, ");
        if (flip.hit)
        {
            player.Health = isBlocked
                ? _combat.Block(monster.Damage, player.Health)
                : _combat.Attack(monster.Damage, player.Health);
            
            Console.WriteLine($"hit {player.Name}, {player.Health}HP");
        }
        else
        {
            Console.WriteLine($"missed {player.Name}: {player.Health}HP");
        }
        Console.ReadLine();
        Console.WriteLine(".\n.\n.\n.");    }

    private void EnemyEncounter(Player player, bool win)
    {
        Monster monster = new();
        Console.WriteLine($"Monster appeared with {monster.Health} HP");
        Console.WriteLine($"Player: {player.Health} HP");
        
        while (monster.Health > 0 && player.Health > 0)
        {
            bool isBlocked = PlayerTurn(player, monster);
            if (monster.Health <= 0 || player.Health <= 0)
            {
                break;
            }
            EnemyTurn(player, monster, isBlocked);
        }
        if (player.Health > monster.Health)
        {
            Console.WriteLine($"{player.Name} wins the battle, {player.Health}HP");
            win = true;
        }
        else
        {
            Console.WriteLine($"{player.Name} was defeated by monster");
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

    private void BattleUI(Player player, Monster monster)
    {
        Console.WriteLine("\n.\n.\n.\n---------------------\n" +
                          $"{player.Health}/{player.MaxHealth}HP | {player.Potions} potions" +
                          $" | Monster: {monster.Health}/{monster.MaxHealth}HP\n---------------------" +
                          "\nType an action: 'attack' -- 'block' -- 'heal' -- 'pass'");
    }
}