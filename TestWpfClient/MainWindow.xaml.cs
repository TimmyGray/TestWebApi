using System;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.IO;
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

        private async void ForUploadBut_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog()==true)
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                foreach ( var file in openFileDialog.FileNames)
                {
                    FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    string filename = System.IO.Path.GetFileName(file);
                  
                    content.Add(new StreamContent(fileStream), "files", filename);
                }

                HttpRequestMessage message = new HttpRequestMessage();
                message.Content = content;
                message.Method = HttpMethod.Post;
                message.RequestUri = new Uri("http://localhost:5232/dbfiles");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    
                    var files = JsonConvert.DeserializeObject<List<DbFile>>(await response.Content.ReadAsStringAsync());
                    ForFileGrid.Items.Add(files);
                    MessageBox.Show($"Файл(ы) успешно добавлен(ы)");
                    
                }
            }

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
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog()==true)
            {

            }
           

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DbFile delf = (DbFile)ForFileGrid.SelectedItem;
            
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Delete;
            request.RequestUri = new Uri($"http://localhost:5232/dbfiles/{delf.Id}");
            
            HttpClient client = new HttpClient();
            
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"{json} успешно удален");
            }

        }
    }
}
