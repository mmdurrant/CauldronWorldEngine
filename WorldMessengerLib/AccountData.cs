using System;

namespace WorldMessengerLib
{
    [Serializable]
    public class AccountData
    {
        public string ClientId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CharacterCount { get; set; }
    }
}