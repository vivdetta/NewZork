using NewZork.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewZork.Services
{
    public class GameEngine
    {
        private GameWorld _gameWorld;
        private Player _player;

        public GameEngine(GameWorld gameWorld, Player player)
        {
            _gameWorld = gameWorld;
            _player = player;
        }

        public void ProcessCommand(string input)
        {
            var commandParts = input.Split(' ', 2);
            var command = commandParts[0];

            switch (command)
            {
                case "move":
                    if (commandParts.Length > 1)
                        Move(commandParts[1]);
                    else
                        Console.WriteLine("You need to choose a specific direction");
                    break;
                case "take":
                    if (commandParts.Length > 1)
                        TakeItem(commandParts[1]);
                    else
                        Console.WriteLine("You need to choose a specific item to take");
                    break;
                case "look":
                    Look();
                    break;
                case "inventory":
                    ShowInventory();
                    break;
                case "help":
                    ShowHelp();
                    break;
                case "save":
                    Save();
                    break;
                case "quit":
                    QuitGame();
                    break;
                default:
                    Console.WriteLine("I don't understand that command.");
                    break;
            }
        }
        private void Move(string direction)
        {
            if (_player.CurrentLocation.Connections.TryGetValue(direction, out string nextLocationName))
            {
                // Find the next location
                Location nextLocation = _gameWorld.Locations.FirstOrDefault(loc => loc.Name.Equals(nextLocationName, StringComparison.OrdinalIgnoreCase));

                if (nextLocation == null)
                {
                    Console.WriteLine("You can't go that way.");
                    return;
                }

                // Check if the location is locked
                if (nextLocation.IsLocked)
                {
                    // Check if the player has the required key
                    bool hasKey = _player.Inventory.Any(item => item.Name.Equals(nextLocation.RequiredKey, StringComparison.OrdinalIgnoreCase));

                    if (hasKey)
                    {
                        // Unlock the location
                        nextLocation.IsLocked = false;
                        Console.WriteLine(nextLocation.Description);
                        Console.WriteLine($"You use the {nextLocation.RequiredKey} to unlock the door. You step through the locked door");

                        // Now get the location beyond the door in the same direction
                        if (nextLocation.Connections.TryGetValue(direction, out string beyondLocationName))
                        {
                            // Find the location beyond the door
                            Location beyondLocation = _gameWorld.Locations.FirstOrDefault(loc => loc.Name.Equals(beyondLocationName, StringComparison.OrdinalIgnoreCase));
                            if (beyondLocation != null)
                            {
                                // Move the player to the location beyond the door
                                _player.CurrentLocation = beyondLocation;
                                // Display the description of the new location
                                Console.WriteLine(_player.CurrentLocation.Description);
                                BossFight();
                            }
                            else
                            {
                                // If beyond location not found, stay at current location
                                Console.WriteLine("You can't go that way.");
                            }
                        }
                        else
                        {
                            // If no connection in that direction, stay at current location
                            Console.WriteLine("You can't go that way.");
                        }
                    }
                    else
                    {
                        _player.CurrentLocation = nextLocation;
                        Console.WriteLine(_player.CurrentLocation.Description);
                        Console.WriteLine("The door is locked. You need a key to open it.");
                        return;
                    }
                }
                else
                {
                    // Move the player to the next location
                    _player.CurrentLocation = nextLocation;
                    Console.WriteLine(_player.CurrentLocation.Description);
                }
            }
            else
            {
                Console.WriteLine("You can't go that way.");
            }
        }
        
        private void TakeItem(string itemName)
        {
            // Find the item name in the current location's items
            var itemNameInLocation = _player.CurrentLocation.Items
                .FirstOrDefault(ite => ite.Equals(itemName, StringComparison.OrdinalIgnoreCase));

            if (itemNameInLocation != null)
            {
                // Find the corresponding Item object in the game world's Items list
                var item = _gameWorld.Items.FirstOrDefault(ite => ite.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

                if (item != null && item.IsCollectable)
                {
                    _player.Inventory.Add(item);
                    _player.CurrentLocation.Items.Remove(itemNameInLocation);
                    Console.WriteLine($"\nYou have taken the {item.Name}.");
                }
                else
                {
                    Console.WriteLine($"\nThe item '{itemName}' cannot be collected.");
                }
            }
            else
            {
                Console.WriteLine($"\nThere is no '{itemName}' here.");
            }
        }

        private void Look()
        {
            var location = _player.CurrentLocation;

            Console.WriteLine();

            if (location.IsMainRoom)
            {
                // Display the main room's description
                Console.WriteLine(location.Description);
            }
            else
            {
                // Player is in a sub-location, get the main room
                var mainRoom = _gameWorld.Locations.FirstOrDefault(loc => loc.Name == location.MainRoomName);

                if (mainRoom != null)
                {
                    // Display the main room's description
                    Console.WriteLine(mainRoom.Description);
                }
                else
                {
                    Console.WriteLine(location.Description);
                }
            }

        }

        private void ShowInventory()
        {
            if (_player.Inventory.Any())
            {
                Console.WriteLine("\nYou are carrying:");
                foreach (var item in _player.Inventory)
                {
                    Console.WriteLine($"- {item.Name}: {item.Description}");
                }
            }
            else
            {
                Console.WriteLine("\nYour inventory is empty.");
            }
        }
        private void BossFight()
        {
            Console.WriteLine("\nAs you step into the chamber, the temperature drops suddenly.");
            Console.WriteLine("From the shadows emerges a towering creature with glowing red eyes and razor-sharp claws.");
            Console.WriteLine("Its roar echoes through the room, shaking the very ground beneath your feet.");
            Console.WriteLine("This is it—the ultimate challenge awaits you!\n");

            int successChance = CalculateSuccessChance();

            Console.WriteLine($"You grip your {GetPlayerWeapon()}, preparing for battle.");
            Console.WriteLine($"You have a {successChance}% chance to defeat the monster.\n");

            // Simulate the fight
            Random random = new Random();
            int roll = random.Next(1, 101); // Generates a number between 1 and 100

            if (roll <= successChance)
            {
                Console.WriteLine("The monster lunges at you with ferocious speed!");
                Console.WriteLine("But with swift reflexes, you dodge its attack and counter with a decisive blow.");
                Console.WriteLine("You strike true, and the creature lets out a final, piercing scream before collapsing.");
                Console.WriteLine("You have defeated the monster! Congratulations!\n");

                // Handle victory (e.g., grant rewards, allow progression)
                //_player.CurrentLocation.BossDefeated = true;
                // Perhaps the player can now take the Ancient Artifact
            }
            else
            {
                Console.WriteLine("The monster charges toward you, and despite your best efforts, its sheer power overwhelms you.");
                Console.WriteLine("You feel a sharp pain as darkness surrounds you.");
                Console.WriteLine("You have been defeated by the monster...\n");
                Console.WriteLine("Game Over.");
                QuitGame();
            }
        }

        private int CalculateSuccessChance()
        {
            bool hasMagicKnife = _player.Inventory.Any(item => item.Name.Equals("Magic Knife", StringComparison.OrdinalIgnoreCase));
            bool hasSword = _player.Inventory.Any(item => item.Name.Equals("Sword", StringComparison.OrdinalIgnoreCase));
            bool hasShield = _player.Inventory.Any(item => item.Name.Equals("Shield", StringComparison.OrdinalIgnoreCase));

            if (hasMagicKnife && hasShield)
            {
                return 90;
            }
            else if (hasSword && hasShield)
            {
                return 60;
            }
            else if (hasShield)
            {
                return 30;
            }
            else
            {
                return 10;
            }
        }
        private string GetPlayerWeapon()
        {
            if (_player.Inventory.Any(item => item.Name.Equals("Magic Knife", StringComparison.OrdinalIgnoreCase)))
                return "Magic Knife";
            else if (_player.Inventory.Any(item => item.Name.Equals("Sword", StringComparison.OrdinalIgnoreCase)))
                return "Sword";
            else
                return "bare hands";
        }

        private void ShowHelp()
        {
            Console.WriteLine("\nAvailable commands:");

            Console.WriteLine("- move [direction]");
            Console.WriteLine("- take [item]");
            Console.WriteLine("- look");
            Console.WriteLine("- inventory");
            Console.WriteLine("- help");
            Console.WriteLine("- quit");
        }
        private void Save()
        {

            try
            {
                // Create a GameState object containing both the player and game world
                var gameState = new GameState
                {
                    Player = _player,
                    GameWorld = _gameWorld
                };

                var json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });

                string savePlayerName = string.Concat(_player.Name.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{savePlayerName}_save.json";

                // Build the file path using your existing approach
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string solutionFolder = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\JsonFiles"));
                string filePath = Path.Combine(solutionFolder, fileName);

                // Ensure the directory exists
                Directory.CreateDirectory(solutionFolder);

                File.WriteAllText(filePath, json);

                Console.WriteLine($"\nGame saved successfully to {filePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while saving the game: {ex.Message}");
            }


        }

        private void QuitGame()
        {
            Console.WriteLine("\nThanks for playing!");
            Environment.Exit(0);
        }
    }
}
