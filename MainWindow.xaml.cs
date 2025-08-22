using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
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
using WPFCurrencyConverter.Models;
using WPFCurrencyConverter.Utils;

namespace WPFCurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ManagerSelectedId { get; set; }
        class WindowDataContext
        {
            public List<string> Currencies { get; set; }
            public int IdFromCurrency { get; set; }
            public int IdToCurrency { get; set; }
            public double AmountToConvert { get; set; }

            public double ManagerRateAmount { get; set; }
            public string ManagerFromCode { get; set; }
            public string ManagerToCode { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            //var currencies = new List<string>(ServiceConverter.CURRENCIES);
            //currencies.Sort();
            using (CurrencyContext ctx = new CurrencyContext())
            {
                DataContext = new WindowDataContext()
                {
                    Currencies = ctx.GetDistinctCodes(),
                    IdFromCurrency = 0,
                    IdToCurrency = 1,
                    AmountToConvert = 0.0
                };
                List<Currency> managerCurrencies = ctx.Currencies.OrderBy(x => x.Code).ToList();
                DGMain.ItemsSource = managerCurrencies;
            }
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

        private void OnSelectedCell(object sender, SelectedCellsChangedEventArgs e)
        {
            //Debug.WriteLine($"{DGMain.SelectedItem.ToString()}");
            try
            {
                Currency rowSelected = (Currency) DGMain.CurrentItem;
                if (rowSelected == null) return;

                ManagerSelectedId = rowSelected.Id;

                if (DGMain.SelectedCells.Count > 0 && DGMain.SelectedCells[0].Column.DisplayIndex == 1)
                {
                    OnDelete();
                    return;
                }
                //MessageBox.Show(rowSelected.Id);
                LoadManagerData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void OnDelete()
        {
            if (string.IsNullOrEmpty(ManagerSelectedId)) return;

            CurrencyContext ctx = new CurrencyContext();

            var askRes = MessageBox.Show("Are you sure you want to delete the data?", "Warning", MessageBoxButton.OKCancel);

            if (askRes == MessageBoxResult.OK)
            {
                Currency? data = ctx.Currencies.SingleOrDefault(x => x.Id == ManagerSelectedId);
                if (data == null) return;

                ctx.Currencies.Remove(data);
                ctx.SaveChanges();
            }

            List<Currency> managerCurrencies = ctx.Currencies.OrderBy(x => x.Code).ToList();
            DGMain.ItemsSource = managerCurrencies;
        }

        private void OnManagerCancel(object sender, RoutedEventArgs e)
        {
            ClearManagerData();
        }

        private void OnManagerSave(object sender, RoutedEventArgs e)
        {
            var dataContext = (WindowDataContext)DataContext;
            //MessageBox.Show($"{dataContext.ManagerRateAmount}{dataContext.ManagerFromCode} -> {dataContext.ManagerToCode}");
            CurrencyContext? ctx = null;
            try
            {
                ctx = new CurrencyContext();
                var cur = new Currency(dataContext.ManagerFromCode, dataContext.ManagerRateAmount, dataContext.ManagerToCode);
                if (!string.IsNullOrEmpty(ManagerSelectedId))
                {
                    cur.Id = ManagerSelectedId;
                    ctx.Currencies.Attach(cur);
                    ctx.Entry(cur).State = EntityState.Modified;
                    // early return to skip update
                    if (MessageBox.Show("Do you want to update the data?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;

                    ctx.Currencies.AddOrUpdate(cur);
                }
                else
                {
                    ctx.Currencies.Add(cur);
                }

                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ClearManagerData();

                if (ctx != null)
                {
                    List<Currency> managerCurrencies = ctx.Currencies.OrderBy(x => x.Code).ToList();
                    DGMain.ItemsSource = managerCurrencies;
                }
            }
        }

        private void ClearManagerData()
        {
            var dataCtx = (WindowDataContext)DataContext;
            InpManagerToRateCode.Text = "";
            InpManagerFromRateCode.Text = "";
            InpManagerRateAmount.Text = "0";
            dataCtx.ManagerToCode = "";
            dataCtx.ManagerFromCode = "";
            dataCtx.ManagerRateAmount = 0;
            ManagerSelectedId = "";
        }

        private void LoadManagerData()
        {
            //List<TextBox> inputs = [InpManagerFromRateCode, InpManagerRateAmount, InpManagerToRateCode];
            var dataCtx = (WindowDataContext)DataContext;
            using (CurrencyContext ctx = new CurrencyContext())
            {
                List<Currency> data = ctx.Currencies.Where(x => x.Id == ManagerSelectedId)
                    .ToList();
                if (data.Count == 0)
                {
                    ClearManagerData();
                    return;
                }

                InpManagerToRateCode.Text = data[0].ToCode;
                InpManagerFromRateCode.Text = data[0].Code;
                InpManagerRateAmount.Text = $"{data[0].Rate}";

                dataCtx.ManagerRateAmount = data[0].Rate;
                dataCtx.ManagerFromCode = data[0].Code;
                dataCtx.ManagerToCode = data[0].ToCode;
            }
        }
    }
}