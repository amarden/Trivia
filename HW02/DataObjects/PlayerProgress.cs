using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HW02.DataObjects
{
    public class PlayerProgress : EntityData
    {
        public string playerId { get; set; }
        public string gameSessionId{ get; set; }
        public string proposedAnswer { get; set; }
        public string answerEvaluation { get; set; }
        [ForeignKey("TriviaQuestion")]
        public string triviaQuestionId { get; set; }
        public TriviaQuestion TriviaQuestion { get; set; }
    }
}