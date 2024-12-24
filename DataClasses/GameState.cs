using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewZork.DataClasses
{
    public class GameState
    {
        public Player Player { get; set; }
        public GameWorld GameWorld { get; set; }
    }
}
