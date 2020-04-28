using System.Threading;

namespace TwitchChatBox
{
    class PingSender
    {
        private IRCClient _irc;
        private Thread pingSender;

        // Empty constructor makes instance of Thread
        public PingSender(IRCClient irc)
        {
            _irc = irc;
            pingSender = new Thread(new ThreadStart(this.Run));
        }

        // Starts the thread
        public void Start()
        {
            pingSender.IsBackground = true;
            pingSender.Start();
        }

        // Send PING to irc server every 5 minutes
        public void Run()
        {
            while (true)
            {
                _irc.SendIrcMessage("PING irc.twitch.tv");
                Thread.Sleep(300000); // 5 minutes
            }
        }
    }
}
