using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW02.ClientDataObjects
{
    public class ClientStartGameSession
    {
        public string playerId { get; set; }
        public string [] triviaIds { get; set; }
    }
}