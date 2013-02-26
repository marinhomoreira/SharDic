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

using System.IO;

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
        Subscription svideo;

        public MainWindow()
        {
            InitializeComponent();

            this.sd = new SharedDictionary();
            this.sd.Url = "tcp://localhost:test";
            this.sd.Open();

            this.s = new Subscription();
            this.s.Pattern = "/*/text";
            this.s.Dictionary = this.sd;
            this.s.Notified += new SubscriptionEventHandler(s_Notified);

            this.svideo = new Subscription();
            this.svideo.Pattern = "/*/video";
            this.svideo.Dictionary = this.sd;
            this.svideo.Notified += new SubscriptionEventHandler(svideo_Notified);
        }

        void s_Notified(object sender, SubscriptionEventArgs e)
        {
            //Console.WriteLine("Key: "+e.Path + " = " + e.Value+"; because: "+e.Reason);
            this.Dispatcher.Invoke(new Action(delegate()
            {
                this.changesBox.Text += "Key: " + e.Path + " = " + e.Value + "; because: " + e.Reason + "\n";    
            }));
            

        }
        
        private FileTransfer ft;

        void svideo_Notified(object sender, SubscriptionEventArgs e)
        {

            ft = e.Value as FileTransfer;
            ft.SaveAs("rcv" + ft.FileName);

            



            //Console.WriteLine("Key: " + e.Path + " = " + e.Value + "; because: " + e.Reason);
            this.Dispatcher.Invoke(new Action(delegate()
            {
                //this.changesBox.Text += "Key: " + e.Path + " = " + e.Value + "; because: " + e.Reason + "\n";

                Stream imageStreamSource = new FileStream("rcv" + ft.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                this.receivedImg.Source = bitmapSource;

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

        private void SubmitImage_Click(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                ft = new FileTransfer(dlg.FileName);
                

                // Open a Stream and decode a JPEG image
                Stream imageStreamSource = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                this.sd["/m/video"] = ft;

                // Draw the Image
                this.submittedImg.Source = bitmapSource;
                //myImage.Stretch = Stretch.None;
                //myImage.Margin = new Thickness(20);
            }

        }


        #region encode and decode

        void decode(string filename)
        {
            Stream imageStreamSource = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];
        }


        void encode()
        {
            int width = 320;
            int height = 240;
            int stride = width / 8;
            byte[] pixels = new byte[height * stride];

            // Define the image palette
            BitmapPalette myPalette = BitmapPalettes.Halftone256;

            // Creates a new empty image with the pre-defined palette
            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);

            FileStream stream = new FileStream("new.jpg", FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            TextBlock myTextBlock = new TextBlock();
            myTextBlock.Text = "Codec Author is: " + encoder.CodecInfo.Author.ToString();
            encoder.FlipHorizontal = true;
            encoder.FlipVertical = false;
            encoder.QualityLevel = 30;
            encoder.Rotation = Rotation.Rotate90;
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }
        #endregion








    }
}
