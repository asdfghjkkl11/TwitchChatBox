using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;

namespace TwitchChatBox{
    public partial class MainWindow : Window{
        JsonLoader jsonlaoder = new JsonLoader();
        DependencyProperty[] Dproperty = new DependencyProperty[]{
                TextBlock.RenderTransformProperty,
                TranslateTransform.XProperty
            };
        public MainWindow(){
            InitializeComponent();
            System.Threading.Thread thread1 = new System.Threading.Thread(
            new System.Threading.ThreadStart(
                delegate (){
                    IRCClient irc = new IRCClient(jsonlaoder.Address, jsonlaoder.Ports,
                            jsonlaoder.Nickname, jsonlaoder.Password, jsonlaoder.Channel);
                    PingSender ping = new PingSender(irc);
                    ping.Start();
                    while (true){
                        // Read any message from the chat room
                        string message = irc.ReadMessage();
                        // Print raw irc messages
                        if (message.Contains("PRIVMSG")){
                            canvas.Dispatcher.BeginInvoke(
                                System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(
                                delegate (){
                                    animate(splitMessage(message));
                                }
                            ));
                        }
                    }
                }
            ));
            thread1.Start();
        }
        public Dictionary<string, string> splitMessage(string ircMessage)
        {
            // Messages from the users will look something like this (without quotes):
            // Format: "@badge-info=;badges=;color=;display-name=;emotes=;flags=;id=;mod=;room-id=;subscriber=;tmi-sent-ts=;turbo=;user-id=;user-type= :[user]![user]@[user].tmi.twitch.tv PRIVMSG #[channel] :[message]"
            // parse messages
            Console.WriteLine(ircMessage);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] strs0 = ircMessage.Split('@');
            string[] strs1 = strs0[1].Split(' ');
            string[] strs2 = strs1[0].Split(';');
            string[] strs3 = strs1[1].Split('!');
            string nickname = strs3[1];
            string message = strs0[2].Replace(nickname+ ".tmi.twitch.tv PRIVMSG #"+jsonlaoder.Channel, "");

            for (int i=0;i<strs2.Length;i++){
                string[] strs = strs2[i].Split('=');
                dictionary.Add(strs[0],strs[1]);
            }
            dictionary.Add("message", message);
            dictionary.Add("nickname", nickname);
            return dictionary;
        }
        public void animate(Dictionary<string, string> parsedMessage)
        {
            TextBlock Text = new TextBlock();
            Brush brush1, brush2;
            string name = parsedMessage["display-name"]+"("+ parsedMessage["nickname"] + ")";
            string message = parsedMessage["message"];
            string color = parsedMessage["color"];
            brush1 = (color != "") ? (SolidColorBrush)(new BrushConverter().ConvertFrom(color)) : Brushes.Green;
            brush2 = (jsonlaoder.Color != "") ? (SolidColorBrush)(new BrushConverter().ConvertFrom(jsonlaoder.Color)) : Brushes.White;
    
            Text.Inlines.Clear();
            Text.Inlines.Add(new Run(name) { Foreground = brush1 });
            Text.Inlines.Add(new Run(message) { Foreground = brush2 });
            Text.FontSize = 30;
            Canvas.SetTop(Text, random());
            canvas.Children.Add(Text);
            double right = 0;

            FrameworkElement parent = Text.Parent as FrameworkElement;

            // 텍스트의 길이가 부모 패널을 넘어갈 때 
            if (Text.ActualWidth > parent.ActualWidth){
                right = Text.ActualWidth - parent.ActualWidth;
            }
            // TranslateTransform을 생성해야 애니메이션 적용
            Text.RenderTransform = new TranslateTransform();

            Storyboard story = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = parent.ActualWidth;
            animation.To = -(parent.ActualWidth + right);
            animation.Duration = TimeSpan.FromSeconds(10);

            // TranslateTransform.XProperty 값 설정
            string path = "(0).(1)";

            Storyboard.SetTargetProperty(animation, new PropertyPath(path, Dproperty));
            story.Children.Add(animation);
            story.Completed += (s, e) => canvas.Children.Remove(Text);
            story.Begin(Text);
            
        }
        double random(){
            Random r = new Random();
            FrameworkElement parent = canvas.Parent as FrameworkElement;
            int height = (int)parent.ActualHeight;
            int rand = r.Next(0, height * 2);
            Console.WriteLine(rand);
            if (rand < height/10)
            {
                return rand;
            }else if (rand > height * 1.9)
            {
                return rand - (height * 1.1);
            }else{
                rand *= 8;
                rand += height;
                return rand/18;
            }
        }
    }
}
