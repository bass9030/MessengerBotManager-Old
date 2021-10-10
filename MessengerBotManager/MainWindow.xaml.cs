using System.Windows;
using System.Windows.Input;

namespace MessengerBotManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        bool grid1Drag = false;
        bool grid2Drag = false;


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = new CheckMDB();
            window.Show();
            window.Activate();
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
                double newX = grid.ColumnDefinitions[0].Width.Value + mp.X - 15;
                double codeX = grid.ColumnDefinitions[1].Width.Value - mp.X + 15;
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
                double newX = grid.ColumnDefinitions[2].Width.Value - mp.X + 15;
                double codeX = grid.ColumnDefinitions[1].Width.Value + mp.X - 15;
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
    }
}
