using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HW02.DataObjects
{
    public class GameSession
    {
        public string playerId { get; set; }
        public string gameSessionId { get; set; }
        public string state { get; set; }
    }
}