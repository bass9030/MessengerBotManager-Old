using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Shapes;

namespace MessengerBotManager
{
    /// <summary>
    /// FirstSettings.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FirstSettings : Window
    {
        public FirstSettings()
        {
            InitializeComponent();
            Loaded += FirstSettings_Loaded;
        }

        private void FirstSettings_Loaded(object sender, RoutedEventArgs e)
        {
            this.Activate();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.MessengerBotRPath == "")
            {
                Properties.Settings.Default.MessengerBotRPath = Path.Text;
                label.Content = "MessengerBotManager가 데이터를 저장할 폴더를 지정해주세요.";
                Find.Visibility = Visibility.Visible;
                Next.Content = "마침";
                Path.Text = "";
            }
            else
            {
                Properties.Settings.Default.DataSavePath = Path.Text;
                Properties.Settings.Default.Save();
                Close();
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok) Path.Text = dialog.FileName;
        }
    }
}
