﻿using System;
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

namespace SharpFabric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CefSharp.Wpf.ChromiumWebBrowser myBrowser;

        public MainWindow()
        {
            InitializeComponent();

            CefSharp.CefSettings settings = new CefSharp.CefSettings()
            {
                //PackLoadingDisabled = true,
            };

            CefSharp.Cef.Initialize(settings);

            myBrowser = new CefSharp.Wpf.ChromiumWebBrowser();
           //myBrowser.Width = browserHolder.Width - 300; 
            //myBrowser.Height = browserHolder.Height - 30;
            //myBrowser.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //myBrowser.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            // Seems WebBrowser is fine - it's canvas that's a fixed size. Rescale automatically?
            // http://stackoverflow.com/questions/1664785/resize-html5-canvas-to-fit-window
            myBrowser.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);

            browserHolder.Children.Add(myBrowser);


            myBrowser.LoadHtml(GetResourceString("SharpFabric.JavaScript.CanvBody.html"), "http://junky/");

            // Initialize fabric.js after the DOM is fully loaded. Not sure whether this is the recommended way to ensure this.
            myBrowser.FrameLoadEnd += delegate 
            { 
                myBrowser.ExecuteScriptAsync(GetResourceString("SharpFabric.JavaScript.fabric.js")); 
                myBrowser.ExecuteScriptAsync(GetResourceString("SharpFabric.JavaScript.qFunctions.js")); 
            };

            myBrowser.RegisterJsObject("callbackObj", new CallbackObjectForJs() { win = this });
        }

        public class CallbackObjectForJs
        {
            public MainWindow win;

            public void tellMe(string msg)
            {
           //     System.Diagnostics.Debug.WriteLine("got message " + msg);
                win.CallbackFromJS(msg);
            }
            public void tellMeNum(double x)
            {
                //System.Diagnostics.Debug.WriteLine("got message " + msg);
                win.CallbackFromJS(x);
            }
        }


        public void CallbackFromJS(string msg)
        {
            System.Diagnostics.Debug.WriteLine("I also got message " + msg);
        }

        public void CallbackFromJS(double x)
        {
            QMove("follower", (int) x,  200);
        }



        string GetResourceString(string id)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            string result; 

            string[] names = assembly.GetManifestResourceNames();

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(id))
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }


        int times = 0;
        private void test_Click(object sender, RoutedEventArgs e)
        {
            string uid = "lkj3klj";

            if (times == 0)
            {
                QCircle(uid, 10, 10 + 10 * times);
                QCircle("follower", 100, 100);
            }
            else if (times == 20)
                myBrowser.LoadHtml(GetResourceString("SharpFabric.JavaScript.GreenSockTest.html"), "http://junky/");
            else if (times > 22)
                myBrowser.ExecuteScriptAsync(string.Format("tl.pause(); tl.progress({0})", ((double)times-22)/40));

            else if (times > 10)
                QDelete(uid);
            else
                QMove(uid, 50, 10 + 10 * times);

            if (times == 3)
                QSetDrawingMode();


            times++;
        }


        void QCircle(string uid, int x, int y)
        {
            myBrowser.ExecuteScriptAsync(string.Format("qJsCircle(\"{0}\",{1},{2})",uid,x,y));
        }

        void QMove(string uid, int x, int y)
        {
            myBrowser.ExecuteScriptAsync(string.Format("qJsMove(\"{0}\",{1},{2})",uid,x,y));

        }

        void QDelete(string uid)
        {
            myBrowser.ExecuteScriptAsync(string.Format("qJsDelete(\"{0}\")",uid));
        }

        void QSetDrawingMode()
        {
            myBrowser.ExecuteScriptAsync("qJsSetDrawingMode()");
        }


    }
}
