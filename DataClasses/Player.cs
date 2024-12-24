using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewZork.DataClasses
{
    public class Player
    {
        public string Name { get; set; }
        public Location CurrentLocation { get; set; }
        public List<Item> Inventory { get; set; } = new List<Item>();

        public Player(string name, Location currentLocation, List<Item> inventory)
        {
            Name = name;
            CurrentLocation = currentLocation;
            Inventory = inventory;
        }
    }
}
