using System;
using System.Collections.Generic;
using System.IO;
using Mdb;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;
using Ookii.Dialogs.Wpf;

namespace MessengerBotManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool grid1Drag = false;
        //bool grid2Drag = false;
        public bool adsf = false;
        bool isBulitinEdit = false;
        FileSystemWatcher watcher;
        MDB mdb;
        CheckMDB window;
        List<string> changeFiles = new List<string>();
        string currentPath;

        public class BotInfo
        {
            public string Name { get; set; }
            public bool Power { get; set; }
            public bool IsCompiled { get; set; }
            public string Path { get; set; }
        }

        public BotInfo SelectedBot { get; set; }
        public List<BotInfo> botInfos = new List<BotInfo>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Activated += MainWindow_Activated;
            //GlowBrush = ToSolidColorBrush(Properties.Settings.Default.ForegroundColor);
            //WindowTitleBrush = ToSolidColorBrush(Properties.Settings.Default.BackgroundColor);
            //App.Current.Resources["MahApps.Brushes.ThemeBackground"] = ToSolidColorBrush(Properties.Settings.Default.BackgroundColor);
            //App.Current.Resources["MahApps.Brushes.IdealForeground"] = ToSolidColorBrush(Properties.Settings.Default.FontColor);
            //App.Current.Resources["MahApps.Brushes.ThemeForeground"] = ToSolidColorBrush(Properties.Settings.Default.ForegroundColor);
            //App.Current.Resources["MahApps.Brushes.Text"] = ToSolidColorBrush(Properties.Settings.Default.FontColor);
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            string expandString = "";
            foreach (string name in changeFiles)
            {
                expandString += name + '\n';
                Console.WriteLine(name);
            }
            TaskDialog dialog = new TaskDialog();
            dialog.WindowTitle = "파일이 외부에서 변경됨";
            dialog.Content = "일부 파일이 변경되었어요.\n다시 불러올까요?";
            dialog.ExpandFooterArea = true;
            dialog.ExpandedInformation = expandString;
            TaskDialogButton button1 = new TaskDialogButton();
            button1.ButtonType = ButtonType.Yes;
            TaskDialogButton button2 = new TaskDialogButton();
            button2.ButtonType = ButtonType.No;
            dialog.Buttons.Add(button1);
            dialog.Buttons.Add(button2);
            dialog.ShowDialog();
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if(isBulitinEdit)
            {
                isBulitinEdit = false;
                return;
            }
            FileAttributes attr = File.GetAttributes(e.FullPath);
            if (attr.HasFlag(FileAttributes.Directory)) return;
            changeFiles.AddRange(new string[] { e.FullPath });
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (isBulitinEdit)
            {
                isBulitinEdit = false;
                return;
            }
            FileAttributes attr = File.GetAttributes(e.FullPath);
            if (attr.HasFlag(FileAttributes.Directory)) return;
            changeFiles.AddRange(new string[] { e.FullPath });
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            window = new CheckMDB();
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //if(window.offline)
            Console.WriteLine("asdf");
            mdb = new MDB(new ADB("adb.exe"));
            watcher = new FileSystemWatcher()
            {
                Path = Properties.Settings.Default.DataSavePath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                Filter = "*.*",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
            Bots.ItemsSource = botInfos;
            //string[] files = GetFiles(Properties.Settings.Default.DataSavePath, )
            refrashBots();
        }

        private void refrashBots()
        {
            foreach (string i in Directory.GetDirectories(Properties.Settings.Default.DataSavePath))
            {
                Console.WriteLine(Path.Combine(Properties.Settings.Default.DataSavePath,
                    i.Split('\\')[i.Split('\\').Length - 1],
                    i.Split('\\')[i.Split('\\').Length - 1] + ".js"));
                if (File.Exists(Path.Combine(Properties.Settings.Default.DataSavePath,
                    i.Split('\\')[i.Split('\\').Length - 1],
                    i.Split('\\')[i.Split('\\').Length - 1] + ".js")))
                {
                    Console.WriteLine(i);
                    JObject bot = JObject.Parse(File.ReadAllText(Path.Combine(i, "bot.json")));
                    BotInfo info = new BotInfo()
                    {
                        IsCompiled = false,
                        Name = i.Split('\\')[i.Split('\\').Length - 1],
                        Path = Path.Combine(Properties.Settings.Default.DataSavePath,
                            i.Split('\\')[i.Split('\\').Length - 1],
                            i.Split('\\')[i.Split('\\').Length - 1] + ".js"),
                        Power = bot["option"]["scriptPower"].ToObject<bool>()
                    };
                    botInfos.Add(info);
                }
            }
            Bots.Items.Refresh();
        }

        private string[] GetFiles(string path, string partten = "*.*")
        {
            List<string> result = new List<string>();
            result.AddRange(Directory.GetFiles(path));
            foreach(string i in Directory.GetDirectories(path))
            {
                result.AddRange(GetFiles(i));
            }

            return result.ToArray();
        }

        private void Grid1Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(Grid1Control);
            grid1Drag = true;
        }

        private void Grid1Control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            grid1Drag = false;
            Mouse.Capture(null);
            this.Cursor = Cursors.Arrow;
        }

        private void Grid1Control_MouseMove(object sender, MouseEventArgs e)
        {
            if(grid1Drag)
            {
                Point mp = e.GetPosition(Grid1Control);
                double newX = grid.ColumnDefinitions[0].Width.Value + mp.X - 7.5;
                double codeX = grid.ColumnDefinitions[1].Width.Value - mp.X + 7.5;
                if (codeX < 0) codeX = 0;
                grid.ColumnDefinitions[1].Width = new GridLength(codeX, GridUnitType.Star);
                if (newX < 0) newX = 0;
                grid.ColumnDefinitions[0].Width = new GridLength(newX, GridUnitType.Star);
            }
        }

        /*private void Grid2Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(Grid2Control);
            grid2Drag = true;
        }

        private void Grid2Control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            grid2Drag = false;
            Mouse.Capture(null);
            this.Cursor = Cursors.Arrow;
        }

        private void Grid2Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (grid2Drag)
            {
                Point mp = e.GetPosition(Grid2Control);
                double newX = grid.ColumnDefinitions[2].Width.Value - mp.X + 7.5;
                double codeX = grid.ColumnDefinitions[1].Width.Value + mp.X - 7.5;
                if (codeX < 0) codeX = 0;
                grid.ColumnDefinitions[1].Width = new GridLength(codeX, GridUnitType.Star);
                if (newX < 0) newX = 0;
                grid.ColumnDefinitions[2].Width = new GridLength(newX, GridUnitType.Star);
            }
        }*/

        private void makeNewBot(object sender, RoutedEventArgs e)
        {
        }

        private void settings(object sender, RoutedEventArgs e)
        {
            new Window1().ShowDialog();
        }

        private void Bots_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Bots.SelectedIndex == -1) return;
            List<MetroTabItem> _list = new List<MetroTabItem>();
            _list.AddRange(tab.Items.OfType<MetroTabItem>());
            int result = _list.FindIndex(f => f.Name == ((BotInfo)Bots.SelectedItem).Name);
            if (result != -1)
            {
                tab.SelectedIndex = result;
                ((MetroTabItem)tab.Items[result]).Background = ToSolidColorBrush(Properties.Settings.Default.ForegroundColor);
                int index = 0;
                foreach(MetroTabItem item in tab.Items)
                {
                    if (index == result)
                    {
                        index++;
                        continue;
                    }
                    item.Background = ToSolidColorBrush(Properties.Settings.Default.BackgroundColor);
                    index++;
                }
                return;
            }

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(90, GridUnitType.Star)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(10, GridUnitType.Star)
            });

            TextBlock label = new TextBlock()
            {
                Text = botInfos[Bots.SelectedIndex].Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                FontSize = 15
            };
            label.Foreground = ToSolidColorBrush(Properties.Settings.Default.FontColor);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            Button button = new Button()
            {
                Content = "X",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Name = botInfos[Bots.SelectedIndex].Name
            };
            button.Click += Button_Click;
            Grid.SetColumn(button, 1);
            grid.Children.Add(button);

            MetroTabItem tabItem = new MetroTabItem()
            {
                Header = grid,
                Name = botInfos[Bots.SelectedIndex].Name
            };
            //tabItem.Background = Brushes.Gray;
            Frame frame = new Frame();
            Page page = new Page1(File.ReadAllText(botInfos[Bots.SelectedIndex].Path));
            page.PreviewKeyDown += Page_PreviewKeyDown;
            frame.Content = page;

            tabItem.Content = frame;


            tab.Items.Add(tabItem);
            tab.SelectedItem = tabItem;

            ((MetroTabItem)tab.Items[tab.SelectedIndex]).Background = ToSolidColorBrush(Properties.Settings.Default.ForegroundColor);
            int index1 = 0;
            foreach (MetroTabItem item in tab.Items)
            {
                if (index1 == tab.SelectedIndex)
                {
                    index1++;
                    continue;
                }
                item.Background = ToSolidColorBrush(Properties.Settings.Default.BackgroundColor);
                index1++;
            }
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Console.WriteLine(((Page1)((Frame)sender).Content).Editor.Text);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < tab.Items.Count; i++)
            {
                MetroTabItem item = (MetroTabItem)tab.Items[i];
                if(item.Name == ((Button)e.OriginalSource).Name)
                {
                    tab.Items.RemoveAt(i);
                }
            }
        }

        public static SolidColorBrush ToSolidColorBrush(string hex_code)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex_code);
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab.SelectedIndex == -1) return;
            ((MetroTabItem)tab.Items[tab.SelectedIndex]).Background = ToSolidColorBrush(Properties.Settings.Default.ForegroundColor);
            int index1 = 0;
            foreach (MetroTabItem item in tab.Items)
            {
                if (index1 == tab.SelectedIndex)
                {
                    index1++;
                    continue;
                }
                item.Background = ToSolidColorBrush(Properties.Settings.Default.BackgroundColor);
                index1++;
            }
        }
    }
}
