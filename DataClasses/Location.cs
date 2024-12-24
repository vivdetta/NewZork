using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewZork.DataClasses
{
    public class Location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Connections { get; set; } = new Dictionary<string, string>();
        public List<string> Items { get; set; } = new List<string>();
        public bool IsMainRoom { get; set; } // True if the location is a Main room
        public string MainRoomName { get; set; } // Gives the discription of the room you are in

        public bool IsLocked { get; set; } = false; // Indicates if the location is locked
        public string RequiredKey { get; set; }      // The name of the key required to unlock
    }




}
