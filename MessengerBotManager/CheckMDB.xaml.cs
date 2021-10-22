using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
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
            foreach (string i in Adb_getFiles("/sdcard/msgbot/Bots"))
            {
                Console.WriteLine(i);
            }
            if (Properties.Settings.Default.MessengerBotRPath == "" || 
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

        public string[] Adb_getFiles(string directory)
        {
            //Console.WriteLine(directory + "(0)" + directory);
            List<string> result = new List<string>();
            if (directory.Length == 0) return result.ToArray();
            Process adb = new Process();
            ProcessStartInfo proinfo = new ProcessStartInfo();
            proinfo.FileName = "adb.exe";
            //Console.WriteLine(directory + "(1)" + PathCombine(directory, '/', "*/"));
            proinfo.Arguments = $"shell ls -d {PathCombine(directory, '/', "*/")}";
            proinfo.StandardErrorEncoding = Encoding.UTF8;
            proinfo.StandardOutputEncoding = Encoding.UTF8;
            proinfo.RedirectStandardOutput = true;
            proinfo.RedirectStandardError = true;
            proinfo.UseShellExecute = false;
            proinfo.CreateNoWindow = true;
            adb.StartInfo = proinfo;
            adb.Start();
            adb.WaitForExit();
            string output = adb.StandardOutput.ReadToEnd();
            string error = adb.StandardError.ReadToEnd();
            if (error != "")
            {
                return result.ToArray();
            }
            List<string> Directory = new List<string>();
            Directory.AddRange(output.Split(new string[] { "\r\n" }, StringSplitOptions.None));
            proinfo.Arguments = $"shell ls {directory}";
            adb.Start();
            adb.WaitForExit();
            string[] _tmpResult = adb.StandardOutput.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string i in _tmpResult)
            {
                if (!Array.Exists(_tmpResult, e => e == i) && result.IndexOf(i) == -1 && i != "")
                {
                    result.Add(PathCombine(directory, '/', i));
                }
            }
            foreach(string i in Directory)
            {
                //Console.WriteLine(directory + "(2) " + i);
                result.AddRange(Adb_getFiles(i));
            }

            return result.ToArray();
        }

        //https://stackoverflow.com/a/38121245
        public static string PathCombine(string pathBase, char separator = '/', params string[] paths)
        {
            if (paths == null || !paths.Any())
                return pathBase;

            #region Remove path end slash
            var slash = new[] { '/', '\\' };
            Action<StringBuilder> removeLastSlash = null;
            removeLastSlash = (sb) =>
            {
                if (sb.Length == 0) return;
                if (!slash.Contains(sb[sb.Length - 1])) return;
                sb.Remove(sb.Length - 1, 1);
                removeLastSlash(sb);
            };
            #endregion Remove path end slash

            #region Combine
            var pathSb = new StringBuilder();
            pathSb.Append(pathBase);
            removeLastSlash(pathSb);
            foreach (var path in paths)
            {
                pathSb.Append(separator);
                pathSb.Append(path);
                removeLastSlash(pathSb);
            }
            #endregion Combine

            #region Append slash if last path contains
            if (slash.Contains(paths.Last().Last()))
                pathSb.Append(separator);
            #endregion Append slash if last path contains

            return pathSb.ToString();
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

            if(!Directory.Exists(Properties.Settings.Default.DataSavePath))
            {
                try
                {
                    Directory.CreateDirectory(Properties.Settings.Default.DataSavePath);
                }
                catch(Exception ex)
                {
                    TaskDialog dialog = new TaskDialog();
                    dialog.WindowTitle = "폴더 생성 실패";
                    dialog.Content = "폴더 생성에 실패했습니다. 폴더 제작 권환이 있는지 확인하세요";
                    TaskDialogButton button1 = new TaskDialogButton();
                    button1.ButtonType = ButtonType.Ok;
                    dialog.Buttons.Add(button1);
                    dialog.ExpandedInformation = ex.ToString();
                    dialog.Show();

                }
            }

            if(!File.Exists("adb.exe") || !File.Exists("AdbWinApi.dll") || !File.Exists("AdbWinUsbApi.dll"))
            {
                Title = "adb 추출중...";
                label.Content = Title;
                File.WriteAllBytes("adb.exe", Properties.Resources.adb);
                File.WriteAllBytes("AdbWinApi.dll", Properties.Resources.AdbWinApi);
                File.WriteAllBytes("AdbWinUsbApi.dll", Properties.Resources.AdbWinApi);
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
            proinfo.StandardOutputEncoding = Encoding.UTF8;
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
                string result = process.StandardOutput.ReadToEnd();
                if (result.IndexOf("no devices/emulators found") == -1)
                {
                    taskDialog.ExpandedInformation = result;
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
                    taskDialog.ExpandedInformation = result;
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
