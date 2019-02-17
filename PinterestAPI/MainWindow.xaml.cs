using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml;
using System.Xml.Serialization;
using PinterestAPI.models;
using System.ServiceModel;
using System.Web;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Collections.Specialized;

namespace PinterestAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Profile userdata = new Profile();
        ServiceHost Host;
        PinterestService.PinterestService Pin = new PinterestService.PinterestService();

        bool StartHost;
        string xmlprofile;
        string tokenResponse;

        public MainWindow()
        {
            InitializeComponent();
            StartHost = true;
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Login m = new Login();            
            m.ShowDialog();
            tokenResponse = m.access.Access_token;

            // builds the  request
            string userinfoRequestURI = "https://api.pinterest.com/v1/me/?access_token=" + tokenResponse + "&fields=url%2Cbio%2Ccounts%2Cfirst_name%2Cusername%2Clast_name";
            try
            {
                // sends the request
                HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
                userinfoRequest.Method = "GET";

                // gets the response
                WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
                using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
                {
                    // reads response body
                    string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                    var node = JsonConvert.DeserializeXNode(userinfoResponseText, "Profile");
                    xmlprofile = node.ToString();
                    Pin.getPinterestUser(xmlprofile);

                    XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                    XmlReader reader2 = XmlReader.Create(new StringReader(xmlprofile));
                    userdata = (Profile)serializer.Deserialize(reader2);

                    textuser.Text = userdata.data.username;
                    textfirst.Text = userdata.data.Fname;
                    textLast.Text = userdata.data.Lname;
                    textbio.Text = userdata.data.bio;
                    textPins.Text = userdata.data.counts.pins;
                    textfollow.Text = userdata.data.counts.followers;
                    textfollowing.Text = userdata.data.counts.following;
                    textBoard.Text = userdata.data.counts.boards;
                }
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Pinterest Log In Error");
            }
            
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.pinterest.com/v1/pins/?access_token=" + tokenResponse);

                string postPin = string.Format("board={0}&note={1}&image_url={2}", 440649213480166546, textPin.Text, "https://i.pinimg.com/60x60_RS/c4/ce/43/c4ce43b844e6b059145fba4d82f14678.jpg");
                var pin = Encoding.ASCII.GetBytes(postPin);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                
                byte[] bytes = Encoding.ASCII.GetBytes(postPin);
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var x = AuthResponse.get(responseString);
                if (x != null)
                {                  
                    MessageBox.Show("Pin Successful");
                    textPin.Clear();
                }
                
            }
            catch (Exception ex)
            {
                textPin.Text = ex.ToString();
            }
                     
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }

        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
            if (StartHost)
            {
                Host = new ServiceHost(typeof(PinterestService.PinterestService));
                Host.Open();
                btnHost.Content = "Stop Service";

            }
            else
            {
                Host.Close();
                btnHost.Content = "Host Service";
            }
            StartHost = !StartHost;
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }
    }
}
