using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;


namespace TwitchChatBox
{
    public partial class MainWindow : Window
    {
        JsonLoader jsonlaoder = new JsonLoader();
        public MainWindow()
        {
            InitializeComponent();
            
            System.Threading.Thread thread1 = new System.Threading.Thread(
            new System.Threading.ThreadStart(
                delegate ()
                {
                    IRCClient irc = new IRCClient(jsonlaoder.Address, jsonlaoder.Ports,
                            jsonlaoder.Nickname, jsonlaoder.Password, jsonlaoder.Channel);
                    PingSender ping = new PingSender(irc);
                    ping.Start();
                    while (true)
                    {
                        // Read any message from the chat room
                        string message = irc.ReadMessage();
                        Console.WriteLine(message);
                        // Print raw irc messages
                        System.Windows.Threading.DispatcherOperation
                            dispatcherOp = tbSettingText.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                            delegate ()
                            {
                                tbSettingText.Text = message;
                            }
                        ));
                        if (message.Contains("PRIVMSG"))
                        {
                            // Messages from the users will look something like this (without quotes):
                            // Format: ":[user]![user]@[user].tmi.twitch.tv PRIVMSG #[channel] :[message]"

                            // Modify message to only retrieve user and message
                            int intIndexParseSign = message.IndexOf('!');
                            string userName = message.Substring(1, intIndexParseSign - 1); // parse username from specific section (without quotes)
                                                                                           // Format: ":[user]!"
                                                                                           // Get user's message
                            intIndexParseSign = message.IndexOf(" :");
                            message = message.Substring(intIndexParseSign + 2);

                            //Console.WriteLine(message); // Print parsed irc message (debugging only)

                        }
                    }
                }
            ));
            thread1.Start();
        }
    }
}
