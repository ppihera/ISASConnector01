using IsasConnectorBase;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISASWPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ISASService iSASService { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            iSASService = new ISASService();
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var response = iSASService.GetOutput(
                    new()
                    {
                        DatabaseName = "OSMOOST",
                        UserName = "pihepa",
                        Password = "Tajne.heslo.3333",
                        Roles = "",
                        SqlQuery = "select ROCNIK from VEC where BC_VEC = 999"
                    }
                );

            MessageBox.Show(response.Records.Count().ToString());
        }




    }
}
