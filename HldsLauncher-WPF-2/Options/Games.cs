using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Options
{
    public static class Games
    {
        public static IList<Game> GoldSourceGamesList = new List<Game>(new Game[]
        {
            new Game { GameArg = "cstrike", GameName = "Counter-Strike 1.6" },
            new Game { GameArg = "czero", GameName = "Counter-Strike: Condition Zero" },
            new Game { GameArg = "dmc", GameName = "Deathmatch Classic" },
            new Game { GameArg = "dod", GameName = "Day of Defeat" },
            new Game { GameArg = "gearbox", GameName = "Half-Life: Opposing Force" },
            new Game { GameArg = "ricochet", GameName = "Ricochet" },
            new Game { GameArg = "tfc", GameName = "Team Fortress Classic" },
            new Game { GameArg = "valve", GameName = "Half-Life" }
        });
        public static IList<Game> SourceGamesList = new List<Game>(new Game[]
        {
            new Game { GameArg = "cstrike", GameName = "Counter-Strike: Source" },
            new Game { GameArg = "dod", GameName = "Day of Defeat: Source" },
            new Game { GameArg = "hl1mp", GameName = "Half-Life Deathmatch: Source" },
            new Game { GameArg = "hl2mp", GameName = "Half-Life 2: Deathmatch" },
            new Game { GameArg = "tf", GameName = "Team Fortress 2" },
            new Game { GameArg = "left4dead", GameName = "Left 4 Dead" },
            new Game { GameArg = "left4dead2", GameName = "Left 4 Dead 2" },
            //new Game { GameArg = "garrysmod", GameName = "Garry’s Mod" }
        });
    }
}
