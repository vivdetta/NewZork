using NewZork.DataClasses;
using NewZork.Services;
using System.Linq;
using System.Numerics;
using System.Text.Json;

namespace NewZork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                GameWorld gameWorld;
                Player player;
                GameEngine commandHandler;

                // Prompt the user to start a new game or load a saved game
                Console.WriteLine("Welcome to The Haunted Shack!");
                Console.WriteLine("Would you like to start a new game or load a saved game?");
                Console.Write("Enter 'new' to start a new game or 'load' to load a saved game: ");

                string choice = Console.ReadLine().Trim().ToLower();

                if (choice == "new")
                {
                    // Start a new game
                    // Load the game world from JSON
                    string filePath = GameWorld.GetJsonFilePath();
                    Console.WriteLine($"Loading game world from: {filePath}");
                    gameWorld = GameWorld.LoadFromJson(filePath);

                    // Check if data loaded successfully
                    if (gameWorld == null)
                    {
                        Console.WriteLine("Failed to load the game world from the JSON file.");
                        return;
                    }

                    Location startingLocation = gameWorld.Locations.FirstOrDefault(loc => loc.Name == "Living Room");

                    if (startingLocation == null)
                    {
                        Console.WriteLine("Starting location 'Living Room' not found in game world.");
                        return;
                    }

                    // Print game introduction
                    Console.WriteLine(gameWorld.GameIntroduction);
                    Console.WriteLine();

                    // Prompt for player name
                    Console.Write("Please enter your name: ");
                    string playerName = Console.ReadLine();
                    player = new Player(playerName, startingLocation, new List<Item>());

                    Console.WriteLine($"\nWelcome {player.Name}!\nI hope you are ready for adventure!\n");
                    Console.WriteLine(player.CurrentLocation.Description);

                    // Create the game engine
                    commandHandler = new GameEngine(gameWorld, player);
                }
                else if (choice == "load")
                {
                    // Load a saved game
                    Console.Write("Please enter your save file name (e.g., 'PlayerName_save.json'): ");
                    string saveFileName = Console.ReadLine().Trim();

                    // Build the file path using your existing approach
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string solutionFolder = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\JsonFiles"));
                    string filePath = Path.Combine(solutionFolder, saveFileName);

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            // Read the JSON from the file
                            var json = File.ReadAllText(filePath);

                            // Deserialize the JSON back into a GameState object
                            var gameState = JsonSerializer.Deserialize<GameState>(json);

                            // Restore the player and game world state
                            player = gameState.Player;
                            gameWorld = gameState.GameWorld;

                            // Update references using GameWorld method
                            gameWorld.UpdatePlayerReferences(player);

                            Console.WriteLine($"\nGame loaded successfully from {filePath}.");
                            Console.WriteLine(player.CurrentLocation.Description);

                            // Create the game engine
                            commandHandler = new GameEngine(gameWorld, player);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nAn error occurred while loading the game: {ex.Message}");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nThe save file '{filePath}' does not exist.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please restart the game and enter 'new' or 'load'.");
                    return;
                }

                // Main game loop
                while (true)
                {
                    Console.WriteLine("\nWhat do you want to do?");
                    string input = Console.ReadLine().Trim().ToLower();
                    commandHandler.ProcessCommand(input);
                }
            }
        }
    }
}