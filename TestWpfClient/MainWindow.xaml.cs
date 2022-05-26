using System;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using TestWebApi.Models;
using System.Text.RegularExpressions;

namespace TestWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ForUploadBut.Click += ForUploadBut_Click;
            
        }

        private void ForUploadBut_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

        }

        private async void ForFileGrid_Loaded(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri("http://localhost:5232/");
            HttpResponseMessage response = await client.GetAsync("dbfiles");
            
            if (response.IsSuccessStatusCode)
            {
                var files = JsonConvert.DeserializeObject<List<DbFile>>(
                     await response.Content.ReadAsStringAsync());
                ForFileGrid.ItemsSource = files;
                

                

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
