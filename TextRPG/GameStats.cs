using System.Text.Json;

namespace TextRPG;
public class GameStats
{
    public int EnemiesDefeated {get; set;}
    public int AllTimeBest  {get; set;}
    
    public static GameStats LoadStats()
    {
        GameStats stats = new();

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

        return stats;
    }
    
    public void SaveShowStats(List<Enemy> enemies)
    {
        EnemiesDefeated += enemies.Count; // Fix JSON Saves
        if (enemies.Count > AllTimeBest) // Must be done before play again switch
        {
            AllTimeBest = enemies.Count;
            Console.WriteLine("New high score!");
        } 
           
        var updatedJson = JsonSerializer.Serialize(this);

        File.WriteAllText("stats.json", updatedJson);
            
        Console.WriteLine($"{enemies.Count} enemies defeated | " +
                          $"All time best: {AllTimeBest} | " +
                          $"{EnemiesDefeated} defeated total");
    }
}