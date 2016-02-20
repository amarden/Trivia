using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HW02.DataObjects
{
    public class HighScore : EntityData
    {
        public string playerId { get; set; }
        public int score { get; set; }
    }
}