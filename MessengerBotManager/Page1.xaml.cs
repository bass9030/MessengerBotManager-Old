using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using HL;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace MessengerBotManager
{
    /// <summary>
    /// Page1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page1 : Page
    {
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;

        public Page1(string code)
        {
            // Dark Mode
            // Background = "#FF1E1E1E" Foreground = "#FFF1F2F3" LineNumbersForeground = "#FF2B91AF" CurrentLineBackground = "#7F0F0F0F"

            // White Mode
            // Background = "#FFFFFFFF" Foreground = "#FF000000" LineNumbersForeground = "#FF808080" CurrentLineBackground = "#FFFCF3AE"

            InitializeComponent();

            // 중괄호 폴딩
            foldingManager = FoldingManager.Install(Editor.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            Editor.TextChanged += Editor_TextChanged;

            // 문법 하이라이팅
            var test = HL.Manager.HighlightingLoader.LoadXshd(XmlReader.Create(new StringReader(Encoding.Default.GetString(
                Properties.Settings.Default.DarkMode ? Properties.Resources.JavaScript_Dark : Properties.Resources.JavaScript_White))));
            Editor.SyntaxHighlighting = HighlightingLoader.Load(test, HighlightingManager.Instance);

            // 에디터 백/포그라운드
            Editor.Foreground = ToSolidColorBrush(Properties.Settings.Default.ForegroundColor);
            Editor.Background = ToSolidColorBrush(Properties.Settings.Default.BackgroundColor);

            // 라인넘버
            Editor.LineNumbersForeground = ToSolidColorBrush(Properties.Settings.Default.LineNumberForegroundColor);

            // 선택라인 하이라이팅(글자가 하이라이팅에 묻혀서 사용 보류)
            /*HighlightCurrentLineBackgroundRenderer backgroundRenderer = 
                new HighlightCurrentLineBackgroundRenderer(Editor, (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.CurrentLineBackground));
            Editor.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);*/

            Editor.Text = code;
        }

        public static SolidColorBrush ToSolidColorBrush(string hex_code)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex_code);
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("Update");
            foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);
        }
    }
}
