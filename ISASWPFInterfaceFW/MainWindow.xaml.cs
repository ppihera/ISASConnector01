using IsasConnectorBase;
using System.Linq;
using System.Windows;

namespace ISASWPFInterfaceFW
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
                    new InputDTO()
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
