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
using WPFCurrencyConverter.Utils;

namespace WPFCurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        struct WindowDataContext
        {
            public string[] Currencies { get; set; }
            public int IdFromCurrency { get; set; }
            public int IdToCurrency { get; set; }
            public double AmountToConvert { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowDataContext()
            {
                Currencies = ServiceConverter.CURRENCIES,
                IdFromCurrency = 0,
                IdToCurrency = 1,
                AmountToConvert = 0.0
            };
        }

        private async void OnConvert(object sender, RoutedEventArgs e)
        {
            WindowDataContext ctx = (WindowDataContext)DataContext;
            //MessageBox.Show($"{ctx.AmountToConvert} {ctx.Currencies[ctx.IdFromCurrency]} -> {ctx.Currencies[ctx.IdToCurrency]}");

            var converter = new ServiceConverter(ctx.AmountToConvert, ctx.Currencies[ctx.IdFromCurrency], ctx.Currencies[ctx.IdToCurrency]);
            BtnConvert.IsEnabled = false;
            BtnClear.IsEnabled = false;
            try
            {
                double converted = await converter.Convert();
                InpToAmount.Text = $"{converted}";
                LbError.Content = "";
            }
            catch (Exception ex)
            {
                LbError.Content = ex.Message;
                InpToAmount.Text = "";
            }
            finally
            {
                BtnConvert.IsEnabled = true;
                BtnClear.IsEnabled = true;
            }
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            WindowDataContext ctx = (WindowDataContext)DataContext;
            InpFromAmount.Text = "0";
            InpToAmount.Text = "";
            ctx.AmountToConvert = 0;
        }
    }
}