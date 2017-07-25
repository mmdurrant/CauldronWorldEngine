namespace WorldMessengerLib.WorldMessages
{
    public class PlayerClient
    {
        public PlayerClient(string playerName, string playerId, int connectionId)
        {
            PlayerName = playerName;
            PlayerId = playerId;
            ConnectionId = connectionId;
        }

        public string PlayerName { get; set; }
        public string PlayerId { get; }
        public string ClientId { get; set; }
        public int ConnectionId { get; }
    }
}