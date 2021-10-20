using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                try
                {
                    Process process = new Process();
                    ProcessStartInfo proinfo = new ProcessStartInfo();
                    proinfo.FileName = "adb.exe";
                    proinfo.Arguments = $"ls {Path.Text}";
                    proinfo.RedirectStandardOutput = true;
                    proinfo.UseShellExecute = false;
                    proinfo.CreateNoWindow = true;
                    process.StartInfo = proinfo;
                    process.Start();
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    if(output.IndexOf("editor_shortcuts.txt") != -1)
                    {
                        TaskDialog dialog = new TaskDialog();
                        dialog.WindowTitle = "경고";
                        dialog.Content = $"메신저봇 R의 루트 경로를 고른것 같습니다.{Path.Text}\n봇폴더로 변경하시겠습니까?\n(기존: {Path.Text} → 변경: {System.IO.Path.Combine(Path.Text, "Bots")}";
                        TaskDialogButton button1 = new TaskDialogButton();
                        button1.ButtonType = ButtonType.Yes;
                        TaskDialogButton button2 = new TaskDialogButton();
                        button2.ButtonType = ButtonType.No;
                        dialog.Buttons.Add(button1);
                        dialog.Buttons.Add(button2);
                    }
                }
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
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult result = dialog.ShowDialog();
            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok) Path.Text = dialog.FileName;
        }
    }
}
