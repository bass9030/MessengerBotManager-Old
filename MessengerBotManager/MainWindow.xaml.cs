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

namespace MessengerBotManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool grid1Drag = false;
        bool grid2Drag = false;
        FileSystemWatcher watcher;
        MDB mdb;

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
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = new CheckMDB();
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
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
            //string[] files = GetFiles(Properties.Settings.Default.DataSavePath, )
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
                    BotInfo info = new BotInfo() {
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
            Bots.ItemsSource = botInfos;
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

        private void Grid2Control_MouseDown(object sender, MouseButtonEventArgs e)
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
        }

        private void makeNewBot(object sender, RoutedEventArgs e)
        {
        }

        private void settings(object sender, RoutedEventArgs e)
        {
            new Window1().Show();
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
            if (Properties.Settings.Default.DarkMode) label.Foreground = Brushes.White;
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            Button button = new Button()
            {
                Content = "X",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
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
            frame.Content = page;

            tabItem.Content = frame;


            tab.Items.Add(tabItem);
            tab.SelectedItem = tabItem;

            ((MetroTabItem)tab.Items[tab.SelectedIndex]).Background = ToSolidColorBrush(Properties.Settings.Default.HighlightColor);
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

        public static SolidColorBrush ToSolidColorBrush(string hex_code)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex_code);
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((MetroTabItem)tab.Items[tab.SelectedIndex]).Background = ToSolidColorBrush(Properties.Settings.Default.HighlightColor);
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
