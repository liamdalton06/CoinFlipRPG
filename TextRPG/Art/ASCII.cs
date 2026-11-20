namespace TextRPG.Art;

public abstract class Ascii
{
    public static void PlayerAvatar()
    {
        Console.WriteLine("  ___\n (o o)\n  \\_/\n /| |\\");
    }

    public static void MonsterAvatar()
    {
        Console.WriteLine(" (\\_/) \n (o.o)\n /|_|\\ \n / ^ \\");
    }

    public static void GiantAvatar()
    {
        Console.WriteLine("  /\\/\\/\\\n ( O  O )\n  \\vvvv/\n /|    |\\\n   /  \\");
    }

    public static void SwordAvatar()
    {
        Console.WriteLine("O====|===============>");
    }

    public static void HeadsAvatar()
    {
        Console.WriteLine("  _____\n /     \\\n|  / \\  |\n|  \\ /  |\n \\_____/");
    }
    
    public static void TailsAvatar()
    {
        Console.WriteLine("  _____\n /     \\\n|  \\|/  |\n| --O-- |\n \\_____/");
    }

}