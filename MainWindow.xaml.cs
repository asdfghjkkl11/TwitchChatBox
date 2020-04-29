using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TwitchChatBox{
    public partial class MainWindow : Window{
        JsonLoader jsonlaoder = new JsonLoader();
        public MainWindow(){
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(Window1_Loaded);
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
                        //Console.WriteLine(message);
                        // Print raw irc messages
                        if (message.Contains("PRIVMSG")){
                            System.Windows.Threading.DispatcherOperation
                            dispatcherOp = canvas.Dispatcher.BeginInvoke(
                                System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(
                                delegate (){
                                    string[] chat = splitMessage(message);
                                    animate(chat);
                                }
                            ));
                        }
                    }
                }
            ));
            thread1.Start();
        }
        public string[] splitMessage(string message)
        {
            // Messages from the users will look something like this (without quotes):
            // Format: "@badge-info=;badges=;color=;display-name=;emotes=;flags=;id=;mod=;room-id=;subscriber=;tmi-sent-ts=;turbo=;user-id=;user-type= :[user]![user]@[user].tmi.twitch.tv PRIVMSG #[channel] :[message]"
            // parse messages
            string[] strs1 = message.Split(';');
            string str = strs1[13];
            if (strs1.Length > 14){
                for (int i = 14; i < strs1.Length; i++){
                    str += strs1[i];
                }
            }
            string[] strs2 = str.Split(':');

            string color = strs1[2].Split('=')[1];
            string displayName = strs1[3].Split('=')[1];
            string userName = strs2[1].Split('!')[0];
            string msg = strs2[2];
            string[] res = new string[3];
            res[0] = displayName + "(" + userName + ")";
            res[1] = ": " + msg;
            res[2] = color;
            return res;
        }
        public void animate(string[] chat)
        {
            TextBlock Text = new TextBlock();
            Brush brush1, brush2;

            brush1 = (chat[2] != "") ? (SolidColorBrush)(new BrushConverter().ConvertFrom(chat[2])) : Brushes.Green;
            brush2 = (jsonlaoder.Color != "") ? (SolidColorBrush)(new BrushConverter().ConvertFrom(jsonlaoder.Color)) : Brushes.White;
    
            Text.Inlines.Clear();
            Text.Inlines.Add(new Run(chat[0]) { Foreground = brush1 });
            Text.Inlines.Add(new Run(chat[1]) { Foreground = brush2 });
            Text.FontSize = 30;
            Canvas.SetTop(Text, 200);
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
            DependencyProperty[] Dproperty = new DependencyProperty[]{
                TextBlock.RenderTransformProperty,
                TranslateTransform.XProperty
            };

            string path = "(0).(1)";

            Storyboard.SetTargetProperty(animation, new PropertyPath(path, Dproperty));
            story.Children.Add(animation);
            story.Begin(Text);
        }
    }
}
