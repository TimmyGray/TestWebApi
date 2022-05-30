using System;
using Newtonsoft.Json;
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
using System.Windows.Shapes;
using TestWebApi.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TestWpfClient
{
    /// <summary>
    /// Interaction logic for RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        private async void ForRegBut_Click(object sender, RoutedEventArgs e)
        {
            AuthorizeModel authorizeModel = new AuthorizeModel();
            authorizeModel.Login = ForLoginBox.Text;
            authorizeModel.Password = ForPassBox.Text;
            var content = JsonConvert.SerializeObject(authorizeModel);
            
            HttpRequestMessage request = new HttpRequestMessage();
            
            request.RequestUri = new Uri("http://localhost:5000/users");
            request.Method = HttpMethod.Post;
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                
            HttpClient client = new HttpClient();
            
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var reguser = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show(reguser);
                Owner.IsEnabled = true;
                Close();
            }
           
            

            
            
        
        }

        private void ForEnterBut_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
