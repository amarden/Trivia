namespace HW02.ClientDataObjects
{
    public class ClientStartGameSession
    {
        public string playerId { get; set; }
        public TriviaId [] triviaIds { get; set; }
    }

    public class TriviaId
    {
        public string id { get; set; }
    }
}