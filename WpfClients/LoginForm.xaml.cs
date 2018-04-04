using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

namespace WpfClients
{
    /// <summary>
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        DataSet vDs = new DataSet();
        DataTable users_dataTable = new DataTable();
        int user_ID = 0;
        public LoginForm()
        {
            InitializeComponent();
            OleDbConnection vConn = new OleDbConnection(Properties.Settings.Default.ConnectString);
            vConn.Open();

            string vQuery = "Select ID , UserName , UserPassword  from tblUsers ";
            OleDbDataAdapter vAdap = new OleDbDataAdapter(vQuery, vConn);
            vAdap.Fill(vDs, "Users");

            this.users_dataTable = vDs.Tables["Users"];
            for (int i = 0; i < users_dataTable.Rows.Count; i++)
            {
                this.User_Combo_box.Items.Add(this.users_dataTable.Rows[i]["UserName"].ToString());
            }
        }

        private void Bttn_enter_Click(object sender, RoutedEventArgs e)
        {
            TryEnter();
        }

        private void TryEnter()
        {
            bool can_enter = false;
            foreach (DataRow dr in this.users_dataTable.Rows)
            {
                if ((dr["UserName"].ToString() == this.User_Combo_box.Text) && (dr["UserPassword"].ToString() == this.txtbx_password.Password))
                {
                    can_enter = true;
                    this.user_ID = (int)dr["ID"];
                    break;
                }
            }
            if (can_enter)
            {
                this.Hide();
                MainWindow mw = new MainWindow(this.user_ID);
                mw.Show();
                this.Close();
            }
        }

        private void Bttn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtbx_password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter)) TryEnter();
        }
    }
}
