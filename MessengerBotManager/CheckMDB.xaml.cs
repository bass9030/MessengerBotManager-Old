using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mdb;
using Ookii.Dialogs.Wpf;

namespace MessengerBotManager
{
    /// <summary>
    /// CheckMDB.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CheckMDB : Window
    {
        Process process;
        public CheckMDB()
        {
            InitializeComponent();
            Loaded += CheckMDB_Loaded;
        }

        private void CheckMDB_Loaded(object sender, RoutedEventArgs e)
        {
            this.Activate();
            if(Properties.Settings.Default.MessengerBotRPath == "" || 
                Properties.Settings.Default.DataSavePath == "")
            {
                Window window = new FirstSettings();
                window.Closed += Window_Closing;
                window.Show();
            }
            else
            {
                Window_Closing(null, null);
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            new Thread(new ParameterizedThreadStart(delegate
            {
                for (int i = 1; i < 20; i++)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        progress.Value = i;
                    }));
                    Thread.Sleep(10);
                }
            })).Start();

            if(!File.Exists("adb.exe") || !File.Exists("AdbWinApi.dll"))
            {
                Title = "adb 추출중...";
                label.Content = Title;
                File.WriteAllBytes("adb.exe", Properties.Resources.adb);
                File.WriteAllBytes("AdbWinApi.dll", Properties.Resources.AdbWinApi);
                new Thread(new ParameterizedThreadStart(delegate
                {
                    for (int i = 20; i < 40; i++)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            progress.Value = i;
                        }));
                        Thread.Sleep(10);
                    }
                })).Start();
            }

            Title = "파일 동기화중...";
            label.Content = Title;

            process = new Process();
            ProcessStartInfo proinfo = new ProcessStartInfo();
            proinfo.FileName = "adb.exe";
            proinfo.Arguments = $"pull {Properties.Settings.Default.MessengerBotRPath} {Properties.Settings.Default.DataSavePath}";
            proinfo.RedirectStandardOutput = true;
            proinfo.UseShellExecute = false;
            proinfo.CreateNoWindow = true;
            process.StartInfo = proinfo;
            process.Start();
            process.WaitForExit();
            Process_Exited(null, null);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(process.ExitCode);
            if(process.ExitCode != 0)
            {
                TaskDialog taskDialog = new TaskDialog();
                taskDialog.MainIcon = TaskDialogIcon.Error;
                taskDialog.WindowTitle = "동기화 실패";
                if (process.StandardOutput.ReadToEnd().IndexOf("no devices/emulators found") == -1)
                {
                    taskDialog.ExpandedInformation = process.StandardOutput.ReadToEnd();
                    TaskDialogButton button1 = new TaskDialogButton();
                    button1.ButtonType = ButtonType.Retry;
                    TaskDialogButton button2 = new TaskDialogButton();
                    button2.ButtonType = ButtonType.Cancel;
                    taskDialog.Buttons.Add(button1);
                    taskDialog.Buttons.Add(button2);
                    taskDialog.Content = "adb에서 0이 아닌 값을 반환했습니다.\n다시 시도하시겠습니까?";
                    if (button1 == taskDialog.Show()) Window_Closing(null, null);
                }
                else
                {
                    taskDialog.Content = "휴대폰을 인식할 수 없습니다.\n휴대폰 연결 및 USB 디버깅 허용후 다시 시도해주세요.";
                    taskDialog.ExpandedInformation = process.StandardOutput.ReadToEnd();
                    TaskDialogButton button1 = new TaskDialogButton();
                    button1.ButtonType = ButtonType.Retry;
                    TaskDialogButton button2 = new TaskDialogButton();
                    button2.ButtonType = ButtonType.Cancel;
                    taskDialog.Buttons.Add(button1);
                    taskDialog.Buttons.Add(button2);
                    if (button1 == taskDialog.Show()) Window_Closing(null, null);
                }
            }
            Title = "완료됨!";
            label.Content = Title;
            new Thread(new ParameterizedThreadStart(delegate
            {
                for (int i = 40; i <= 100; i++)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        progress.Value = i;
                    }));
                    Thread.Sleep(10);
                }
                Dispatcher.Invoke(new Action(delegate
                {
                    Close();
                }));
            })).Start();
        }
    }
}
