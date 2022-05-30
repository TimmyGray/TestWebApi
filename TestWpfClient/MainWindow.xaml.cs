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
using System.Collections.ObjectModel;

namespace TestWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
          ObservableCollection<DbFile>? files = new ObservableCollection<DbFile>();
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
                message.RequestUri = new Uri("http://localhost:5000/dbfiles");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    
                    var addfiles = JsonConvert.DeserializeObject<ObservableCollection<DbFile>>(await response.Content.ReadAsStringAsync());
                    foreach (var file in addfiles)
                    {
                        files.Add(file);
                    }
                    MessageBox.Show($"Файл(ы) успешно добавлен(ы)");
                    
                }
            }

        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog()==true)
            {
                DbFile downfile = (DbFile)ForFileGrid.SelectedItem;
                saveFile.Title = "Скачать файл";
               
                string path = $"{saveFile.FileName}{downfile.Type}"; 

                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri($"http://localhost:5000/dbfiles/{downfile.Id}");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                   var file = JsonConvert.DeserializeObject<DbFile>(await response.Content.ReadAsStringAsync());
                    using (FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                    {
                        stream.WriteAsync(file.Data, 0, file.Data.Length);

                    }
                    MessageBox.Show("Файл скачен");
                }
            }
           

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DbFile delf = (DbFile)ForFileGrid.SelectedItem;
            
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Delete;
            request.RequestUri = new Uri($"http://localhost:5000/dbfiles/{delf.Id}");
            
            HttpClient client = new HttpClient();
            
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string delfile = await response.Content.ReadAsStringAsync();
                files.Remove(files.FirstOrDefault(f=>f.Id==delf.Id));
                MessageBox.Show($"{delfile} успешно удален");
            }

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ForFileGrid.ItemsSource = files;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri("http://localhost:5000/");
            HttpResponseMessage response = await client.GetAsync("dbfiles");
          
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content=="\"Нужна авторизация!\"")
                {

                    RegWindow regWindow = new RegWindow();
                    regWindow.Show();
                    regWindow.Owner = this;
                    IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("LSAD");
                    var download = JsonConvert.DeserializeObject<ObservableCollection<DbFile>>(
                        await response.Content.ReadAsStringAsync());
                    foreach (var file in download)
                    {
                        files.Add(file);

                    }
                    
                }
               
            }

        }
    }
}
