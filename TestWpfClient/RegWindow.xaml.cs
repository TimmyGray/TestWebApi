using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using TestWebApi.Models;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace TestWpfClient
{
    /// <summary>
    /// Interaction logic for RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        Label ForLoginLabel;
        HttpClient client;
        ObservableCollection<DbFile> files;
        public RegWindow(Label ForLoginLabel, HttpClient client, ObservableCollection<DbFile> files)
        {
            InitializeComponent();
            this.ForLoginLabel = ForLoginLabel;
            this.client = client;
            this.files = files;
        }

        private async void ForRegBut_Click(object sender, RoutedEventArgs e)
        {

            if (ForLoginBox.Text==null&&ForPassBox.Text==null)
            {
                MessageBox.Show("Поля должны быть заполнены");
            }
            else
            {
                AuthorizeModel authorizeModel = new AuthorizeModel { Login = ForLoginBox.Text, Password = ForPassBox.Text };

                using (HttpResponseMessage response = await client.PostAsJsonAsync("/users/registration", authorizeModel))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var download = JsonConvert.DeserializeObject<ObservableCollection<DbFile>>(response.Content.ReadAsStringAsync().Result);
                        MessageBox.Show($"{authorizeModel.Login} успешно добавлен!");
                        Owner.IsEnabled = true;
                        Owner.Show();
                        ForLoginLabel.Content = authorizeModel.Login;

                        foreach (DbFile file in download)
                        {
                            files.Add(file);
                        }

                        Close();
                        
                    }
                    else
                    {
                        var error = response.Content.ReadAsStringAsync().Result;
                        MessageBox.Show(error);

                    }
                }
                
            }
                
        }

        private async void ForEnterBut_Click(object sender, RoutedEventArgs e)
        {

            if (ForLoginBox.Text == null && ForPassBox.Text == null)
            {
                MessageBox.Show("Все поля должны быть заполнены");

            }
            else
            {
                AuthorizeModel login = new AuthorizeModel { Login = ForLoginBox.Text, Password = ForPassBox.Text };

                using (HttpResponseMessage response = await client.PostAsJsonAsync("/users/login", login))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var download = JsonConvert.DeserializeObject<ObservableCollection<DbFile>>(response.Content.ReadAsStringAsync().Result);
                        Owner.IsEnabled = true;
                       
                        Owner.Show();
                        ForLoginLabel.Content = login.Login;

                        foreach (DbFile file in download)
                        {
                            files.Add(file);
                        }

                        Close();
                     
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(error);
                    
                    }
                }
                
            }

        }
    }
}
