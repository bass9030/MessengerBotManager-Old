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

            //
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
                Console.WriteLine("asdf");
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
        }

        private void HighlightingThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(HighlightingThemes.SelectedIndex == 2)
            {
                VistaOpenFileDialog fileopendialog = new VistaOpenFileDialog()
                {
                    DefaultExt = "xshd"
                };
                if ((bool)fileopendialog.ShowDialog())
                {
                    ((ComboBoxItem)HighlightingThemes.Items[HighlightingThemes.SelectedIndex]).Content = fileopendialog.FileName;
                    //HighlightingThemes.SelectedIndex = HighlightingThemes.SelectedIndex;
                    HighlightingThemes.Items.Refresh();
                }
            }
        }
    }
}
