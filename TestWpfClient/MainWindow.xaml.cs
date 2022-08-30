using System;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using TestWebApi.Models;
using System.Collections.ObjectModel;

namespace TestWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<DbFile> files;
        readonly HttpClient client;
       
        public MainWindow()
        {
            InitializeComponent();
            IsEnabled = false;
           

            files = new ObservableCollection<DbFile>();

            client = new HttpClient();
           
            client.BaseAddress = new Uri("http://localhost:5000");

            ForUploadBut.Click += ForUploadBut_Click;
        }
        
        private async void ForUploadBut_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog()==true)
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    foreach (var file in openFileDialog.FileNames)
                    {
                        
                        FileStream fs = File.OpenRead(file);
                        string filename = Path.GetFileName(file);
                        content.Add(new StreamContent(fs),"files",filename);
                       
                    }

                    try
                    {
                        using (HttpResponseMessage response = await client.PostAsync($"/dbfiles", content))
                        {
                            
                            if (response.IsSuccessStatusCode)
                            {
                                var addfiles = JsonConvert.DeserializeObject<ObservableCollection<DbFile>>(await response.Content.ReadAsStringAsync());
                                foreach (var file in addfiles)
                                {
                                   
                                    files.Add(file);
                                    MessageBox.Show($"{file.Id}\n{file.File.Id}");
                                }
                                MessageBox.Show($"Файл(ы) успешно добавлен(ы)");

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string innermes = ex.InnerException.Message;
                        string mes =  ex.Message;
                        string stacktrace = ex.StackTrace;
                        MessageBox.Show($"Что-то пошло не так - {mes}\n{innermes}\n{stacktrace}");
                    }
                   
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
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync($"/dbfiles/{downfile.Id}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var file = JsonConvert.DeserializeObject<DbFile>(await response.Content.ReadAsStringAsync());

                            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                            {

                                await stream.WriteAsync(file.File.Data, 0, file.File.Data.Length);

                            }
                            MessageBox.Show("Файл скачен");
                        }
                    }
                }
                catch (Exception ex)
                {
                    string innermes = ex.InnerException.Message;
                    string mes = ex.Message;
                    string stacktrace = ex.StackTrace;
                    MessageBox.Show($"Что-то пошло не так - {mes}\n{innermes}\n{stacktrace}");
                }
                
               
            }
           

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DbFile delf = (DbFile)ForFileGrid.SelectedItem;

            try
            {
                using (HttpResponseMessage response = await client.DeleteAsync($"/dbfiles/{delf.Id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string delfile = await response.Content.ReadAsStringAsync();
                        files.Remove(files.FirstOrDefault(f => f.Id == delf.Id));
                        MessageBox.Show($"{delfile} успешно удален");
                    }
                }
            }
            catch (Exception ex)
            {
                string innermes = ex.InnerException.Message;
                string mes = ex.Message;
                string stacktrace = ex.StackTrace;
                MessageBox.Show($"Что-то пошло не так - {mes}\n{innermes}\n{stacktrace}");
            }
           
            
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ForFileGrid.ItemsSource = files;

            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.RequestUri = new Uri(client.BaseAddress, "/dbfiles");
            request.Method = HttpMethod.Get;
            try
            {
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        if (content == "\"Нужна авторизация!\"")
                        {
                            RegWindow regWindow = new RegWindow(LoginLabel, client, files);
                            Hide();
                            regWindow.Show();
                            regWindow.Owner = this;

                        }
                        else
                        {
                            Show();
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
            catch (Exception ex)
            {
                string innermes = ex.InnerException.Message;
                string mes = ex.Message;
                string stacktrace = ex.StackTrace;
                MessageBox.Show($"Что-то пошло не так - {mes}\n{innermes}\n{stacktrace}");
            }


        }

        private async void LougOutBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await client.GetAsync("/users/logout");

            }
            catch (Exception ex)
            {
                string innermes = ex.InnerException.Message;
                string mes = ex.Message;
                string stacktrace = ex.StackTrace;
                MessageBox.Show($"Что-то пошло не так - {mes}\n{innermes}\n{stacktrace}");
            }
            LoginLabel.Content = "";
            files.Clear();
            IsEnabled = false;
            RegWindow regWindow = new RegWindow(LoginLabel, client, files);
            Hide();
            regWindow.Show();
            regWindow.Owner = this;

        }
    }
}
