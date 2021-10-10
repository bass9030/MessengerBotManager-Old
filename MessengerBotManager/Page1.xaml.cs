﻿using System;
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
        public Page1()
        {
            // Dark Mode
            // Background = "#FF1E1E1E" Foreground = "#FFF1F2F3" LineNumbersForeground = "#FF2B91AF" CurrentLineBackground = "#7F0F0F0F"

            // White Mode
            // Background = "#FFFFFFFF" Foreground = "#FF000000" LineNumbersForeground = "#FF808080" CurrentLineBackground = "#FFFCF3AE"

            InitializeComponent();
            foldingManager = FoldingManager.Install(Editor.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            var test = HL.Manager.HighlightingLoader.LoadXshd(XmlReader.Create(new StringReader(Encoding.Default.GetString(Properties.Resources.JavaScript_Dark))));
            Editor.SyntaxHighlighting = HighlightingLoader.Load(test, HighlightingManager.Instance);
            Editor.TextArea.TextView.BackgroundRenderers.Add(
                new HighlightCurrentLineBackgroundRenderer(Editor, (Color)ColorConverter.ConvertFromString("#FFDFD991")));
            Editor.TextChanged += Editor_TextChanged;
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("Update");
            foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);
        }

        public string Text
        {
            get { return Editor.Text; }
        }
    }
}
