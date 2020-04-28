using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwitchChatBox
{
    public partial class MainWindow : Window
    {
        JsonLoader jsonlaoder = new JsonLoader();
        
        public MainWindow()
        {
            InitializeComponent();
            jsonlaoder.GetData();
            tbSettingText.Text = jsonlaoder.Address + '\n';
            tbSettingText.Text += jsonlaoder.Ports + '\n';
            tbSettingText.Text += jsonlaoder.Nickname + '\n';
            tbSettingText.Text += jsonlaoder.Password + '\n';
            tbSettingText.Text += jsonlaoder.Channel + '\n';
        }
    }
}
