using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewZork.DataClasses
{
    public class GameWorld
    {
        public string GameIntroduction { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<Item> Items { get; set; } = new List<Item>();

        public static GameWorld LoadFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var gameWorld = JsonSerializer.Deserialize<GameWorld>(json);
                return gameWorld;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return null;
            }
        }


        public static string GetJsonFilePath()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string solutionFolder = Path.Combine(basePath, @"..\..\..\JsonFiles");
            string jsonZorkFilePath = Path.Combine(solutionFolder, "JSONzork.json");

            return jsonZorkFilePath;
        }
        public void UpdatePlayerReferences(Player player)
        {
            // Update the player's current location reference
            player.CurrentLocation = Locations
                .FirstOrDefault(loc => loc.Name == player.CurrentLocation.Name);

            // Update item references in the player's inventory
            var updatedInventory = new List<Item>();
            foreach (var item in player.Inventory)
            {
                var updatedItem = Items.FirstOrDefault(i => i.Name == item.Name);
                if (updatedItem != null)
                {
                    updatedInventory.Add(updatedItem);
                }
            }
            player.Inventory = updatedInventory;
        }

    }
}
