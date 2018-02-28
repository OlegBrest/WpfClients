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
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Data.OleDb;

namespace WpfClients
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSet vDs = new DataSet();
        public MainWindow()
        {
            InitializeComponent();
            OleDbConnection vConn = new OleDbConnection(Properties.Settings.Default.ConnectString);
            vConn.Open();

            string vQuery = "Select * from tblMain";
            OleDbDataAdapter vAdap = new OleDbDataAdapter(vQuery, vConn);
            vAdap.Fill(vDs, "Client");



            //this.dataGrid.DataContext = vDs.Tables["Client"];
            this.dataGrid.ItemsSource = vDs.Tables["Client"].DefaultView;
            vConn.Close();


        }
    }
}
