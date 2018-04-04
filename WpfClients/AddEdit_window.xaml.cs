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
using System.Data;
using System.Data.OleDb;

namespace WpfClients
{
    /// <summary>
    /// Логика взаимодействия для AddEdit_window.xaml
    /// </summary>
    public partial class AddEdit_window : Window
    {
        DataGrid main_dg = new DataGrid();
        int selected_indx = -1;
        DataGrid main_tmp_dg = new DataGrid();
        public AddEdit_window(int inx, DataGrid dg)
        {
            this.selected_indx = inx;
            this.main_tmp_dg = dg;
            this.main_dg.AutoGenerateColumns = true;
            this.main_dg.Loaded += Main_dg_Loaded;
            InitializeComponent();
            this.main_grid.Children.Add(this.main_dg);
            
        }


        private void Main_dg_Loaded(object sender, RoutedEventArgs e)
        {
            List<DataItem> result = new List<DataItem>();
            DataRowView drv = this.main_tmp_dg.Items[this.selected_indx] as DataRowView;
            for (int i = 1; i < this.main_tmp_dg.Columns.Count; i++)
            {
               result.Add( new DataItem(this.main_tmp_dg.Columns[i].Header.ToString(),drv[i]));
            }
            this.main_dg.ItemsSource = result;
          /*  for (int i = 1; i < this.main_dg.Items.Count; i++)
            {
                DataGridCell dgc = new DataGridCell();
                dgc.ty = new DataGridCheckBoxColumn();
            }*/
        }

        public class DataItem
        {
            public DataItem(string c, string v)
            {
                this.column = c;
                this.value = v;
            }

            public DataItem(string c, bool v)
            {
                this.column = c;
                this.value = v;
            }

            public DataItem(string c, double v)
            {
                this.column = c;
                this.value = v;
            }

            public DataItem(string c, object v)
            {
                this.column = c;
                this.value = v;
            }

            public string column { get; set; }
            public object value { get; set; }
        }

        private void ok_bttn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_bttn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
