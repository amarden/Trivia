using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class AppData
    {
        private string playerId { get; set; }
        private string gameSessionId { get; set; }
        private Windows.Storage.ApplicationDataContainer localStorage = Windows.Storage.ApplicationData.Current.LocalSettings;
        
        public AppData()
        {
            this.playerId = (string)localStorage.Values["playerId"];
            this.gameSessionId = (string)localStorage.Values["gameSession"];
        }

        public void setPlayerId(string playerId)
        {
            localStorage.Values["playerId"] = playerId;
            this.playerId = playerId;
        }

        public void setGameSession(string gameSession)
        {
            localStorage.Values["gameSession"] = gameSession;
            this.gameSessionId = gameSession;
        }

        public string getGameSession()
        {
            return this.gameSessionId;
        }

        public string getPlayerId()
        {
            return this.playerId;
        }

        public void clearGameSession()
        {
            localStorage.Values["gameSession"] = null;
            this.gameSessionId = null;
        }
    }
}
