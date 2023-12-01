namespace DiscordBot
{
    public class BotConfig
    {
        public string Token { get; init; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                throw new ArgumentException("Token must be set");
            }
        }
    }
}
