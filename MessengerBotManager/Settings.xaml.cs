using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Ookii.Dialogs.Wpf;
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
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window1 : MetroWindow
    {
        int selIndex = 0;

        public bool changed = false;
        public Window1(int _selIndex = 0)
        {
            InitializeComponent();
            selIndex = _selIndex;
            Loaded += Window1_Loaded;
            //App.Current.Resources["MahApps.Brushes.ThemeBackground"] = new SolidColorBrush(HexToColor(Properties.Settings.Default.BackgroundColor));
            //.Current.Resources["MahApps.Brushes.ThemeForeground"] = new SolidColorBrush(HexToColor(Properties.Settings.Default.ForegroundColor));
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            Category.Focus();
            Category.SelectedIndex = selIndex;

            loadSettings();
        }

        private void Category_Selected(object sender, RoutedEventArgs e)
        {
            if (Category.SelectedIndex == -1) return;

            // TODO: 모든 요소 Hidden 처리하기
            // ex.
            editor.Visibility = Visibility.Hidden;
            mdb.Visibility = Visibility.Hidden;
            // info.Visibility = Visibility.Hidden;
            // file.Visibility = Visibility.Hidden;

            switch (Category.SelectedIndex)
            {
                case 0:
                    mdb.Visibility = Visibility.Visible;
                    break;

                case 2:
                    editor.Visibility = Visibility.Visible;
                    break;
            }
        }

        public static Color HexToColor(string hex_code)
        {
            return (Color)ColorConverter.ConvertFromString(hex_code);
        }

        private void Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Dark Mode
            // Background = "#FF1E1E1E"
            // Foreground = "#FFF1F2F3"
            // LineNumbersForeground = "#FF2B91AF"
            // CurrentLineBackground = "#7F0F0F0F"
            // FontColor = #FFFFFFFF
            // EnableColor = #FF6E6E6E

            // White Mode
            // Background = "#FFFFFFFF"
            // Foreground = "#FF000000"
            // LineNumbersForeground = "#FF808080"
            // CurrentLineBackground = "#FFFCF3AE"
            // FontColor = #FF000000
            // EnableColor = #FF6E6E6E
            if (Themes.SelectedIndex == -1 || background == null)
            {
                return;
            }

            switch (Themes.SelectedIndex)
            {
                case 0:
                    custom.IsEnabled = false;
                    background.SelectedColor = HexToColor("#FF1E1E1E");
                    foreground.SelectedColor = HexToColor("#FFF1F2F3");
                    lineNumberForeground.SelectedColor = HexToColor("#FF2B91AF");
                    currnetLineNumber.SelectedColor = HexToColor("#7F0F0F0F");
                    fontColor.SelectedColor = HexToColor("#FF6E6E6E");
                    break;

                case 1:
                    custom.IsEnabled = false;
                    background.SelectedColor = HexToColor("#FFFFFFFF");
                    foreground.SelectedColor = HexToColor("#FF000000");
                    lineNumberForeground.SelectedColor = HexToColor("#FF808080");
                    currnetLineNumber.SelectedColor = HexToColor("#FFFCF3AE");
                    fontColor.SelectedColor = HexToColor("#FF000000");
                    break;

                case 2:
                    custom.IsEnabled = true;
                    background.SelectedColor = HexToColor(Properties.Settings.Default.BackgroundColor);
                    foreground.SelectedColor = HexToColor(Properties.Settings.Default.ForegroundColor);
                    lineNumberForeground.SelectedColor = HexToColor(Properties.Settings.Default.LineNumberForegroundColor);
                    currnetLineNumber.SelectedColor = HexToColor(Properties.Settings.Default.CurrentLineBackground);
                    fontColor.SelectedColor = HexToColor(Properties.Settings.Default.FontColor);
                    break;
            }

            changed = true;
            apply.IsEnabled = changed;
        }

        private void HighlightingThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HighlightingThemes.SelectedIndex == -1) return;

            if(apply != null)
            {
                changed = true;
                apply.IsEnabled = changed;
            }

            switch (HighlightingThemes.SelectedIndex)
            {
                case 2:
                    VistaOpenFileDialog fileopendialog = new VistaOpenFileDialog()
                    {
                        DefaultExt = "xshd"
                    };
                    if ((bool)fileopendialog.ShowDialog())
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = fileopendialog.FileName;
                        HighlightingThemes.Items.Add(item);
                        HighlightingThemes.SelectedItem = item;
                        //HighlightingThemes.SelectedIndex = HighlightingThemes.SelectedIndex;
                        HighlightingThemes.Items.Refresh();
                    }
                    break;
            }
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            if (changed)
            {
                TaskDialog taskDialog = new TaskDialog();
                taskDialog.MainIcon = TaskDialogIcon.Warning;
                taskDialog.WindowTitle = "경고";
                TaskDialogButton button1 = new TaskDialogButton("저장");
                TaskDialogButton button2 = new TaskDialogButton();
                button2.ButtonType = ButtonType.Cancel;
                taskDialog.Buttons.Add(button1);
                taskDialog.Buttons.Add(button2);
                taskDialog.Content = $"변경된 사항이 있습니다. 저장하시겠습니까?";
                if (taskDialog.ShowDialog() == button1) saveSettings();
                else Close();
            }
            else Close();
        }

        private void saveSettings()
        {
            //TODO: Save all settings
        }

        private void loadSettings()
        {
            //TODO: Load all settings
            Themes.SelectedItem = Properties.Settings.Default.themeIndex;
            switch(Properties.Settings.Default.xshdPath)
            {
                case "JavaScript_Dark":
                    HighlightingThemes.SelectedIndex = 0;
                    break;

                case "JavaScript_White":
                    HighlightingThemes.SelectedIndex = 0;
                    break;

                default:
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = Properties.Settings.Default.xshdPath;
                    HighlightingThemes.Items.Add(item);
                    HighlightingThemes.SelectedItem = item;
                    HighlightingThemes.Items.Refresh();
                    break;
            }
            portnum.Text = Properties.Settings.Default.MDBPort.ToString();
            isGroupChat.IsOn = Properties.Settings.Default.isGroupChat;
            sender.Text = Properties.Settings.Default.sender;
            room.Text = Properties.Settings.Default.room;
            packageName.Text = Properties.Settings.Default.packageName;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            Close();
        }
    }
}
