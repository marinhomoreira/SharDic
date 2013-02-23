using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GroupLab.Networking;

namespace SharDic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SharedDictionary sd;
        Subscription s;

        public MainWindow()
        {
            InitializeComponent();

            this.sd = new SharedDictionary();
            this.sd.Url = "tcp://localhost:test";
            this.sd.Open();

            this.s = new Subscription();
            this.s.Pattern = "/*";
            this.s.Dictionary = this.sd;
            this.s.Notified += new SubscriptionEventHandler(s_Notified);
        }

        void s_Notified(object sender, SubscriptionEventArgs e)
        {
            Console.WriteLine("Key: "+e.Path + " = " + e.Value+"; because: "+e.Reason);
            this.Dispatcher.Invoke(new Action(delegate()
            {
                this.changesBox.Text += "Key: " + e.Path + " = " + e.Value + "; because: " + e.Reason + "\n";    
            }));
            

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            this.sd[this.keyBox.Text] = this.valueBox.Text;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            this.sd.Remove(this.keyBox.Text);
        }






    }
}
