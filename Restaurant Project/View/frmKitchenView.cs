using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurant_Project.View
{
    public partial class frmKitchenView: Form
    {
        public frmKitchenView()
        {
            InitializeComponent();
        }

        private void frmKitchenView_Load(object sender, EventArgs e)
        {
            GetOrders();
        }
        private void GetOrders()
        {
            flowLayoutPanel1.Controls.Clear(); // Clear previous order controls

            //string qry1 = @"Select * from tblMain where status = 'Pending' "; // SQL query
            string qry1 = @"SELECT * FROM tblMain WHERE status = 'Hold' OR status = 'Pending'";


            SqlCommand cmd1 = new SqlCommand(qry1, MainClass.con); // Command using connection
            DataTable dt1 = new DataTable(); // To hold query results
            SqlDataAdapter da = new SqlDataAdapter(cmd1); // Adapter to fetch data
            da.Fill(dt1); // Fill DataTable with results

            FlowLayoutPanel p1; // Declares a panel (likely for later use per order)

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                p1 = new FlowLayoutPanel();
                p1.AutoSize = true;
                p1.Width = 230;
                p1.Height = 350;
                p1.FlowDirection = FlowDirection.TopDown;
                p1.BorderStyle = BorderStyle.FixedSingle;
                p1.Margin = new Padding(10, 10, 10, 10);

                FlowLayoutPanel p2 = new FlowLayoutPanel();
                p2 = new FlowLayoutPanel();
                p2.BackColor = Color.FromArgb(50, 55, 89);
                p2.AutoSize = true;
                p2.Width = 230;
                p2.Height = 125;
                p2.FlowDirection = FlowDirection.TopDown;
                p2.Margin = new Padding(0, 0, 0, 0);

                Label lb1 = new Label();
                lb1.ForeColor = Color.White;
                lb1.Margin = new Padding(10, 10, 3, 0);
                lb1.AutoSize = true;

                Label lb2 = new Label();
                lb2.ForeColor = Color.White;
                lb2.Margin = new Padding(10, 5, 3, 0);
                lb2.AutoSize = true;

                Label lb3 = new Label();
                lb3.ForeColor = Color.White;
                lb3.Margin = new Padding(10, 5, 3, 0);
                lb3.AutoSize = true;

                Label lb4 = new Label();
                lb4.ForeColor = Color.White;
                lb4.Margin = new Padding(10, 5, 3, 10);
                lb4.AutoSize = true;

                // Set label text using DataTable values
                lb1.Text = "Table : " + dt1.Rows[i]["TableName"].ToString();
                lb2.Text = "Waiter Name : " + dt1.Rows[i]["WaiterName"].ToString();
                lb3.Text = "Order Time : " + dt1.Rows[i]["atime"].ToString();
                lb4.Text = "Order Type : " + dt1.Rows[i]["orderType"].ToString();

                p2.Controls.Add(lb1);
                p2.Controls.Add(lb2);
                p2.Controls.Add(lb3);
                p2.Controls.Add(lb4);

                p1.Controls.Add(p2);
                flowLayoutPanel1.Controls.Add(p1);

                int mid = 0;
                //mid = Convert.ToInt32(dt1.Rows[1]["MainID"].ToString());
                if (dt1.Rows.Count >= 1)
                {
                    mid = Convert.ToInt32(dt1.Rows[i]["MainID"].ToString());
                }
                else
                {
                    // Handle the case: show message or use Rows[0] if that's intended
                    MessageBox.Show("Not enough rows returned from query.");
                }

                //string qry2 = @"Select * from tblMain m 
                //inner join tblDetails d on m.MainID = d.MainID 
                //inner join products p on p.PID = d.proID 
                //Where m.MainID = '" + mid + "'";

                string qry2 = @"SELECT * FROM tblMain m 
                INNER JOIN tblDetails d ON m.MainID = d.MainID 
                INNER JOIN products p ON p.PID = d.proID 
                WHERE m.MainID = '" + mid + "'";
                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                DataTable dt2 = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                da2.Fill(dt2);

                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    Label lb5 = new Label();
                    lb5.ForeColor = Color.Black;
                    lb5.Margin = new Padding(10, 5, 3, 0);
                    lb5.AutoSize = true;

                    int no = j + 1;

                    lb5.Text = "" + no + " " + dt2.Rows[j]["pName"].ToString() + " " + dt2.Rows[j]["qty"].ToString();

                    p1.Controls.Add(lb5);
                }
                //add button to change  the order status
                Guna.UI2.WinForms.Guna2Button b = new Guna.UI2.WinForms.Guna2Button();
                b.AutoRoundedCorners = true;
                b.Size = new Size(100, 35);
                b.FillColor = Color.FromArgb(241, 85, 126);
                b.Margin = new Padding(30, 5, 3, 10);
                b.Text = "Complete";
                b.Tag = dt1.Rows[i]["MainID"].ToString(); //store the id

                b.Click += new EventHandler(b_click);
                p1.Controls.Add(b);

                flowLayoutPanel1.Controls.Add(p1);

            }
        }

        private void b_click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((sender as Guna.UI2.WinForms.Guna2Button).Tag.ToString());

            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;

            //if (guna2MessageDialog1.Show(this.FindForm(), "Are you sure you want to delete?") == DialogResult.Yes)
            DialogResult result = MessageBox.Show(this.FindForm(), "Are you sure you want to delete?", "RMS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string qry = @"Update tblMain set status = 'Complete' where MainID = @ID";
                Hashtable ht = new Hashtable();
                ht.Add("@ID", id);

                if (MainClass.SQl(qry, ht) > 0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    //guna2MessageDialog1.Show("Saved Successfully");
                    MessageBox.Show(this.FindForm(), "Saved Successfully", "RMS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                GetOrders();
            }

        }
    }
}
