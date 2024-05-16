using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;
using Syncfusion.XlsIO.Interfaces;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using testing_Program.PopUps;
using static testing_Program.Entities.TableContext;

namespace testing_Program
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromelessWindow
    {
        public ICommand Page1Command { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand BlocksCommand { get; set; }
        public ICommand ProdStoreCommand { get; set; }
        public ICommand SprayCommand { get; set; }
        public ICommand WeatherCommand {  get; set; }
        public ICommand EquipmentCommand { get; set; }

        public MainWindow()
        {
            SfSkinManager.ApplyStylesOnApplication = true;
            SfSkinManager.SetVisualStyle(Application.Current.MainWindow, VisualStyles.Lime); 
            SfSkinManager.SetTheme(this, new Theme("Lime", new string[] { "TabControlExt", "SfNavigationDrawer", "ChromelessWindow" }));

            InitializeComponent();
            Page1Command = new RelayCommand(Pests);
            HomeCommand = new RelayCommand(Home);
            BlocksCommand = new RelayCommand(Blocks);
            ProdStoreCommand = new RelayCommand(ProdS);
            SprayCommand = new RelayCommand(Spray);
            WeatherCommand = new RelayCommand(Weath);
            EquipmentCommand = new RelayCommand(Equip);
            DataContext = this;
            Home();
        }

        private void Pests()
        {
            contentFrame.Navigate(new Pests());
            lblHeader.Content = "Pest Monitoring";
        }

        private void Home()
        {
            contentFrame.Navigate(new Home());
            lblHeader.Content = "Home";
        }

        private void Blocks()
        {
            contentFrame.Navigate (new Blocks());
            lblHeader.Content = "Block Management";
        }

        private void ProdS()
        {
            contentFrame.Navigate(new ProdStore());
            lblHeader.Content = "Product Store";
        }

        private void Spray()
        {
            contentFrame.Navigate(new SprayIns());
            lblHeader.Content = "Spray Instructions";
        }

        private void Weath()
        {
            contentFrame.Navigate(new WeatherScreen());
            lblHeader.Content = "Weather";
        }

        private void Equip()
        {
            contentFrame.Navigate(new Equipment());
            lblHeader.Content = "Equipment";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FarmSetup fs = new FarmSetup();
            fs.ShowDialog();
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true; // Can always execute
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }
    }
}