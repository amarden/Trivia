using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HW02.DataObjects
{
    public class GameSession : EntityData
    {
        public string playerId { get; set; }
        public Guid gameSessionId { get; set; }
        public string state { get; set; }
    }
}