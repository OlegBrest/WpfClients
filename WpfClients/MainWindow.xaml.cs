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
using System.Drawing;
using System.Resources;
using System.Reflection;

namespace WpfClients
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSet vDs = new DataSet();
        int user_ID = 0;
        string vQuery = "";
        OleDbConnection vConn = new OleDbConnection(Properties.Settings.Default.ConnectString);
        OleDbDataAdapter vAdap = new OleDbDataAdapter();
        Dictionary<string, string> settings_dict;
        public MainWindow(int i)
        {
            this.user_ID = i;
            InitializeComponent();


            this.vConn.Open();
            this.vQuery = "Select * from tblSettings where SettingName = '" + user_ID.ToString() + ".Menu'";
            this.vAdap = new OleDbDataAdapter(this.vQuery, this.vConn);
            this.vAdap.Fill(this.vDs, "User_Menu");
            this.vConn.Close();


            this.vConn.Open();
            this.vQuery = "Select * from tblSettings";
            this.vAdap = new OleDbDataAdapter(this.vQuery, this.vConn);
            this.vAdap.Fill(this.vDs, "Captions");
            this.vConn.Close();
            DataTable dtCapt = this.vDs.Tables["Captions"];
            settings_dict = new Dictionary<string, string>();
            foreach (DataRow dr in dtCapt.Rows) settings_dict.Add(dr[0].ToString(), dr[1].ToString());


            string Main_Menu_fullstring = this.vDs.Tables["User_Menu"].Rows[0]["SettingValue"].ToString();
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
            this.vConn.Open();
            this.vQuery = "Select * from tblSettings where SettingName like '" + user_ID.ToString() + ".Tabs'";
            this.vAdap = new OleDbDataAdapter(this.vQuery, this.vConn);
            this.vAdap.Fill(this.vDs, "User_Tabs");
            this.vConn.Close();


            string Main_Tabs_fullstring = this.vDs.Tables["User_Tabs"].Rows[0]["SettingValue"].ToString();
            Main_Tabs_fullstring = Main_Tabs_fullstring.Replace("~", "");
            Main_Tabs_fullstring = Main_Tabs_fullstring.Replace("|1", "\n");
            string[] Main_Tabs_substring = Main_Tabs_fullstring.Split('\n');


            foreach (string Tabs in Main_Tabs_substring)
            {
                if (Tabs != "")
                {
                    this.vConn.Open();
                    this.vQuery = "Select * from " + Tabs;
                    this.vAdap = new OleDbDataAdapter(this.vQuery, this.vConn);
                    this.vAdap.Fill(this.vDs, Tabs);
                    this.vConn.Close();


                    TabItem Ti = new TabItem();
                    Ti.Name = Tabs;
                    DataGrid dg = new DataGrid();
                    dg.CanUserAddRows = false;
                    dg.Name = "datagrid" + "_" + Tabs;
                    dg.ItemsSource = this.vDs.Tables[Tabs].DefaultView;
                    dg.UpdateLayout();
                    dg.AutoGeneratingColumn += Dg_AutoGeneratingColumn;
                    Menu mnu = new Menu();
                    DockPanel dp = new DockPanel();

                    string toolbarfullstring = "";
                    this.settings_dict.TryGetValue(user_ID + "." + Ti.Name + ".ToolbarFullString", out toolbarfullstring);
                    if (toolbarfullstring == null)
                        this.settings_dict.TryGetValue("1." + Ti.Name + ".ToolbarFullString", out toolbarfullstring);
                    if ((Tabs == "qdfOrders") || (Tabs == "qdfDeals"))
                        this.settings_dict.TryGetValue("1.qdfOrdersProducts.ToolbarFullString", out toolbarfullstring);
                    toolbarfullstring = toolbarfullstring.Replace("|||", "\n");
                    string[] toolbar_buttons = toolbarfullstring.Split('\n');
                    for (int ii = 0; ii < toolbar_buttons.Length; ii++)
                    {
                        string cur_butt = toolbar_buttons[ii];
                        string[] cur_butt_subs = cur_butt.Split('|');
                        if (cur_butt_subs[0] != "")
                        {
                            if (cur_butt.Contains("Separator"))
                            {
                                Separator sep = new Separator();
                                mnu.Items.Add(sep);
                            }
                            else
                            {


                                Button mni = new Button();
                                mni.MinWidth = 20;
                                mni.MinHeight = 20;
                                mni.ToolTip = cur_butt_subs[2];
                                mni.Margin = new Thickness(-5, 0, -5, 0);
                                mni.Name = ("bttn_" + Tabs + "_"+cur_butt_subs[3]);
                                mni.IsEnabled = cur_butt_subs[5] == "1"? true:false;
                                mni.Visibility = cur_butt_subs[4] == "1"? Visibility.Visible:Visibility.Hidden;
                                mni.Click += Mni_Click;
                                try
                                {
                                    ImageBrush imbr = new ImageBrush();
                                    imbr.ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               ((Bitmap)(Properties.Resources.ResourceManager.GetObject(cur_butt_subs[0].Replace("~","")))).GetHbitmap(),
               IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                    mni.Background = imbr;
                                }
                                catch
                                { }
                                if (mni.Visibility == Visibility.Visible)mnu.Items.Add(mni);

                            }
                        }
                    }



                    dp.LastChildFill = true;
                    DockPanel.SetDock(mnu, Dock.Top);
                    DockPanel.SetDock(dg, Dock.Bottom);
                    dp.Children.Add(mnu);
                    dp.Children.Add(dg);
                    Ti.Content = dp;
                    //Ti.Content = dg;
                    string header;
                    this.settings_dict.TryGetValue(Ti.Name + ".Caption", out header);
                    Ti.Header = header;
                    this.mainTab.Items.Add(Ti);
                }
            }

        }

        private void Mni_Click(object sender, RoutedEventArgs e)
        {
            Button some_bttn = sender as Button;
            string bttn_name = some_bttn.Name;
            string Tabs = bttn_name.Split('_')[1];
            string action_str = bttn_name.Split('_')[2];

            if (action_str == "REFRESH")
            {
                this.vDs.Tables[Tabs].Clear();
                this.vConn.Open();
                this.vQuery = "Select * from " + Tabs;
                this.vAdap = new OleDbDataAdapter(this.vQuery, this.vConn);
                this.vAdap.Fill(this.vDs, Tabs);
                this.vConn.Close();
            }
        }

        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataTable dtCapt = this.vDs.Tables["Captions"];
            DataGridColumn dgc = e.Column;
            string need2find = dg.Name.Split('_')[1] + "." + dgc.Header + ".Caption";
            string header = "";
            double col_size = dgc.Width.DisplayValue;
            this.settings_dict.TryGetValue(need2find, out header);
            need2find = user_ID + "." + dg.Name.Split('_')[1] + ".GridColWidth";
            string full_widths = "";
            this.settings_dict.TryGetValue(need2find, out full_widths);
            string[] col_width = full_widths.Split(';');
            foreach (string row in col_width)
            {
                if (row.Contains(dgc.Header.ToString()))
                {
                    col_size = Convert.ToDouble(row.Split('=')[1]) / 10;
                    break;
                }
            }

            need2find = user_ID + "." + dg.Name.Split('_')[1] + ".GridColVisible";
            string visible = "";
            this.settings_dict.TryGetValue(need2find, out visible);
            string[] visions = visible.Split(' ');
            foreach (string row in visions)
            {
                if (row.Contains(dgc.Header.ToString()))
                {
                    string val = row.Split('=')[1];
                    if (val == "1") dgc.Visibility = Visibility.Visible;
                    else dgc.Visibility = Visibility.Hidden;
                    break;
                }
            }

            dgc.Width = col_size;
            dgc.Header = header;
        }
    }
}
