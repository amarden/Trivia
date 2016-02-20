using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HW02.DataObjects
{
    public class PlayerProgress : EntityData
    {
        public string playerId { get; set; }
        public Guid gameSessionId{ get; set; }

        public string proposedAnswer { get; set; }
        public string answerEvaluation { get; set; }
        public string triviaQuestionId { get; set; }
        public TriviaQuestion triviaQuestion { get; set; }
    }
}