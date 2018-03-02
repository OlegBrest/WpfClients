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
        int user_ID = 0;
        public MainWindow(int i)
        {
            this.user_ID = i;
            InitializeComponent();
            OleDbConnection vConn = new OleDbConnection(Properties.Settings.Default.ConnectString);
            vConn.Open();

            string vQuery = "Select * from tblSettings where SettingName = '" + user_ID.ToString() + ".Menu'";
            OleDbDataAdapter vAdap = new OleDbDataAdapter(vQuery, vConn);
            vAdap.Fill(vDs, "User_Menu");
            vConn.Close();

            string Main_Menu_fullstring = vDs.Tables["User_Menu"].Rows[0]["SettingValue"].ToString();
            string[] Main_Menu_substring = Main_Menu_fullstring.Split('\n');
            // Забьём элементы меню
            foreach (string item in Main_Menu_substring)
            {
                string main_item = "";
                string[] items_subitems = item.Split('|');
                if (!items_subitems[0].Contains("Items"))
                {
                    main_item = items_subitems[0];
                    MenuItem mi = new MenuItem();
                    mi.Name = main_item;
                    mi.Visibility = items_subitems[1] == "1" ? Visibility.Visible : Visibility.Hidden;
                    mi.IsEnabled = items_subitems[2] == "1" ? true : false;
                    mi.Header = items_subitems[3];
                    this.Main_menu.Items.Add(mi);
                    foreach (string subitem in Main_Menu_substring)
                    {
                        if ((subitem.Contains(main_item)) && (subitem.Contains("Items")))
                        {
                            string[] subsub_item;
                            subsub_item = subitem.Split('|');
                            MenuItem msi = new MenuItem();
                            msi.Name = main_item;
                            msi.Visibility = subsub_item[1] == "1" ? Visibility.Visible : Visibility.Hidden;
                            msi.IsEnabled = subsub_item[2] == "1" ? true : false;
                            msi.Header = subsub_item[3];
                            if (subsub_item[3] != "-") mi.Items.Add(msi);
                            else mi.Items.Add(new Separator());
                        }
                    }
                }
            }


            // Закладки главного меню
            vConn.Open();
            vQuery = "Select * from tblSettings where SettingName like '" + user_ID.ToString() + ".Tabs'";
            vAdap = new OleDbDataAdapter(vQuery, vConn);
            vAdap.Fill(vDs, "User_VisTabs");
            vConn.Close();
            vConn.Open();
            vQuery = "Select * from tblSettings where SettingName like '%Caption%'";
            vAdap = new OleDbDataAdapter(vQuery, vConn);
            vAdap.Fill(vDs, "Captions");
            vConn.Close();
            DataTable dtCapt = vDs.Tables["Captions"];

            string Main_Tabs_fullstring = vDs.Tables["User_VisTabs"].Rows[0]["SettingValue"].ToString();
            Main_Tabs_fullstring = Main_Tabs_fullstring.Replace("~", "");
            Main_Tabs_fullstring = Main_Tabs_fullstring.Replace("|1", "\n");
            string[] Main_Tabs_substring = Main_Tabs_fullstring.Split('\n');
            foreach (string Tabs in Main_Tabs_substring)
            {
                TabItem Ti = new TabItem();
                Ti.Name = Tabs;
                foreach (DataRow drCap in dtCapt.Rows)
                {
                    if (drCap["SettingName"].ToString() == (Ti.Name + ".Caption"))
                    {
                        Ti.Header = drCap["SettingValue"].ToString();
                        break;
                    }
                }
                this.mainTab.Items.Add(Ti);
            }

        }

    }
}
