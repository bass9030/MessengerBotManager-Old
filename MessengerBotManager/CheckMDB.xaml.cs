using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using MahApps.Metro.Controls;
using Mdb;
using Ookii.Dialogs.Wpf;

namespace MessengerBotManager
{
    /// <summary>
    /// CheckMDB.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CheckMDB : MetroWindow
    {
        Process process;
        public bool offline = false;
        public CheckMDB()
        {
            InitializeComponent();            
            Loaded += CheckMDB_Loaded;
        }

        private void CheckMDB_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.MessengerBotRPath == "" || 
                Properties.Settings.Default.DataSavePath == "")
            {
                Window window = new FirstSettings();
                window.Closed += Window_Closing;
                window.ShowDialog();
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
            ProcessStartInfo proinfo = new ProcessStartInfo
            {
                FileName = "adb.exe",
                Arguments = $"shell ls -d {PathCombine(directory, '/', "*/")}",
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            adb.StartInfo = proinfo;
            adb.Start();
            adb.WaitForExit();
            string output = adb.StandardOutput.ReadToEnd();
            string error = adb.StandardError.ReadToEnd();
            if (error != "")
            {
                if (error.IndexOf("no devices/emulators found") != -1) return new string[] { "error" };
                else return result.ToArray();
            }
            List<string> Directory = new List<string>();
            Directory.AddRange(output.Split(new string[] { "\r\n" }, StringSplitOptions.None));
            proinfo.Arguments = $"shell ls -d {PathCombine(directory, '/', "*")}";
            adb.Start();
            adb.WaitForExit();
            string[] _tmpResult = adb.StandardOutput.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string i in _tmpResult)
            {
                //Console.WriteLine(i);
                if (Regex.IsMatch(i, "(.+\\..+)$") && result.IndexOf(i) == -1 && i != "")
                {
                    result.Add(i);
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
                if (slash.Contains(path[0])) pathSb.Append(path.Substring(1, path.Length - 1));
                else pathSb.Append(path);
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
                    dialog.ShowDialog();
                }
            }

            if(!File.Exists("adb.exe") || !File.Exists("AdbWinApi.dll") || !File.Exists("AdbWinUsbApi.dll"))
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    description.Content = "adb 다운로드중...";
                }));
                using (WebClient wc = new WebClient())
                {
                    File.WriteAllBytes("tmp.zip", wc.DownloadData("https://dl.google.com/android/repository/platform-tools-latest-windows.zip?hl=ko"));
                    Dispatcher.Invoke(new Action(delegate
                    {
                        description.Content = "adb 압축 해제중...";
                    }));
                    using (ZipArchive archive = ZipFile.OpenRead(".\\tmp.zip"))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if(entry.Name == "adb.exe" || entry.Name.StartsWith("AdbWin"))
                            {
                                entry.ExtractToFile(Path.Combine(".\\", entry.Name));
                            }
                        }
                    }
                }
            }

            Dispatcher.Invoke(new Action(delegate
            {
                description.Content = "파일 동기화중...";
            }));

            if (Process.GetProcessesByName("adb.exe").Length != 0) 
            {
                foreach (Process i in Process.GetProcessesByName("adb.exe")) i.Kill();
            }

            process = new Process();
            ProcessStartInfo proinfo = new ProcessStartInfo();
            proinfo.FileName = "adb.exe";
            proinfo.StandardOutputEncoding = Encoding.UTF8;
            proinfo.RedirectStandardOutput = true;
            proinfo.UseShellExecute = false;
            proinfo.CreateNoWindow = true;
            process.StartInfo = proinfo;

            Dispatcher.Invoke(new Action(delegate
            {
                description.Content = "adb 연결 확인중...";
            }));
            proinfo.Arguments = "shell ls";
            process.Start();
            new Thread(new ThreadStart(delegate
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    TaskDialog taskDialog = new TaskDialog();
                    taskDialog.MainIcon = TaskDialogIcon.Error;
                    taskDialog.Content = "휴대폰을 인식할 수 없습니다.\n휴대폰 연결 및 USB 디버깅 허용후 다시 시도해주세요.";
                    TaskDialogButton button1 = new TaskDialogButton();
                    button1.ButtonType = ButtonType.Retry;
                    TaskDialogButton button2 = new TaskDialogButton();
                    button2.ButtonType = ButtonType.Cancel;
                    taskDialog.Buttons.Add(button1);
                    taskDialog.Buttons.Add(button2);
                    process.StandardOutput.Close();
                    if (button1 == taskDialog.ShowDialog())
                    {
                        proinfo.Arguments = "kill-server";
                        process.Start();
                        process.WaitForExit();
                        Window_Closing(null, null);
                        return;
                    }
                    else
                    {
                        offline = true;
                        Dispatcher.Invoke(new Action(delegate
                        {
                            Close();
                        }));
                        return;
                    }
                }

                foreach (string i in Adb_getFiles(Properties.Settings.Default.MessengerBotRPath))
                {
                    while (true)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            description.Content = "파일 동기화중..." + i;
                        }));
                        string directory = PathCombine(Properties.Settings.Default.DataSavePath, '\\', Path.GetDirectoryName(i.Replace(Properties.Settings.Default.MessengerBotRPath, ""))).Replace(Properties.Settings.Default.MessengerBotRPath, "");
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        proinfo.Arguments = $"pull {i} {PathCombine(directory, '\\', Path.GetFileName(i))}";
                        process.Start();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            TaskDialog taskDialog = new TaskDialog();
                            taskDialog.MainIcon = TaskDialogIcon.Error;
                            taskDialog.WindowTitle = "동기화 실패";
                            string result = process.StandardError.ReadToEnd();
                            taskDialog.ExpandedInformation = result;
                            TaskDialogButton button1 = new TaskDialogButton();
                            button1.ButtonType = ButtonType.Retry;
                            TaskDialogButton button2 = new TaskDialogButton();
                            button2.ButtonType = ButtonType.Cancel;
                            taskDialog.Buttons.Add(button1);
                            taskDialog.Buttons.Add(button2);
                            taskDialog.Content = $"'{i}'을(를) 동기화 중 adb에서 0이 아닌 값을 반환했습니다.\n다시 시도하시겠습니까?";
                            if (taskDialog.ShowDialog().ButtonType == ButtonType.Retry) continue;
                        }
                        break;
                    }
                }
                Process_Exited(null, null);
            })).Start();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            //Console.WriteLine(process.ExitCode);
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
                    if (button1 == taskDialog.ShowDialog()) Window_Closing(null, null);
                }
                else
                {
                    taskDialog.Content = "휴대폰을 인식할 수 없습니다.\n휴대폰 연결 및 USB 디버깅 허용후 다시 시도해주세요.";
                    TaskDialogButton button1 = new TaskDialogButton();
                    button1.ButtonType = ButtonType.Retry;
                    TaskDialogButton button2 = new TaskDialogButton();
                    button2.ButtonType = ButtonType.Cancel;
                    taskDialog.Buttons.Add(button1);
                    taskDialog.Buttons.Add(button2);
                    if (button1 == taskDialog.ShowDialog()) Window_Closing(null, null);
                    else offline = true;
                }
            }

            Dispatcher.Invoke(new Action(delegate
            {
                description.Content = "완료!";
                Close();
            }));
        }
    }
}
