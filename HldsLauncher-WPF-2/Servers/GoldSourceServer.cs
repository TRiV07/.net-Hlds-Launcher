using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Options;
using HldsLauncher.Log;
using System.Globalization;

namespace HldsLauncher.Servers
{
    public class GoldSourceServer : ValveGameServer
    {
        private ILogger _logger;

        public GoldSourceServer()
        {
            _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();
            Options = new GoldSourceServerOptions();
        }

        protected override void ParseFirstLine(string firstLine)
        {
            int fpsFrom = 0;
            int fpsTo = firstLine.IndexOf("fps") - 1;
            int onlinePlayersFrom = fpsTo + 1;
            int onlinePlayersTo = firstLine.IndexOf('/');
            int mapFrom = firstLine.IndexOf("on") + 2;
            int mapTo = firstLine.Length - 1;
            try
            {
                if (onlinePlayersFrom != -1 && onlinePlayersTo != -1 && onlinePlayersTo - onlinePlayersFrom - 4 < 6 && onlinePlayersTo - onlinePlayersFrom - 4 > 0)
                {
                    ServerStatus.Fps = double.Parse(firstLine.Substring(fpsFrom, fpsTo - fpsFrom).Trim(), CultureInfo.GetCultureInfo("en-US"));
                    ServerStatus.OnlinePlayers = int.Parse(firstLine.Substring(onlinePlayersFrom + 4, onlinePlayersTo - onlinePlayersFrom - 4));
                    ServerStatus.Map = firstLine.Substring(mapFrom, mapTo - mapFrom).Trim();
                    ServerStatus.MaxPlayers = (Options as ValveGameServerOptions).MaxPlayers;
                }
            }
            catch (Exception e)
            {
                _logger.DebugException("ParseFirstLine error", e);
            }
        }
    }
}
