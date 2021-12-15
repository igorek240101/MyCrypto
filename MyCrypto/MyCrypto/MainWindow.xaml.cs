using System;
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
using Microsoft.Win32;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace MyCrypto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "txt files (*.txt)|*.txt|docx files (*.docs)|*.docx";
            fileDialog.ShowDialog();
            WebClient client = new WebClient();
            if (fileDialog.FileName.Split('.')[^1] == "txt")
            {
                result.Text = Encoding.UTF8.GetString(client.UploadFile("https://localhost:5001/Crypto/Decrypt/" + Key.Text, fileDialog.FileName));
                SaveBtn.IsEnabled = true;
            }
            else
            {
                byte[] array = client.UploadFile("https://localhost:5001/Crypto/Decrypt/" + Key.Text, fileDialog.FileName);
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "docx files (*.docs)|*.docx";
                saveFile.ShowDialog();
                File.WriteAllBytes(saveFile.FileName, array);
                Process process = new Process();
                process.StartInfo.FileName = saveFile.FileName;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "txt files (*.txt)|*.txt|docx files (*.docs)|*.docx";
            fileDialog.ShowDialog();
            WebClient client = new WebClient();
            if (fileDialog.FileName.Split('.')[^1] == "txt")
            {
                result.Text = Encoding.UTF8.GetString(client.UploadFile("https://localhost:5001/Crypto/Encrypt/" + Key.Text, fileDialog.FileName));
                SaveBtn.IsEnabled = true;
            }
            else
            {
                byte[] array = client.UploadFile("https://localhost:5001/Crypto/Encrypt/" + Key.Text, fileDialog.FileName);
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "docx files (*.docs)|*.docx";
                saveFile.ShowDialog();
                File.WriteAllBytes(saveFile.FileName, array);
                Process process = new Process();
                process.StartInfo.FileName = saveFile.FileName;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "txt files (*.txt)|*.txt";
            saveFile.ShowDialog();
            File.WriteAllText(saveFile.FileName, result.Text);
        }
    }
}
