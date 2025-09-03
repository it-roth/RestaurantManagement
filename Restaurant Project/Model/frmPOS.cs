using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Restaurant_Project.View;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Restaurant_Project.Model
{
    public partial class frmPOS : Form
    {
        public frmPOS()
        {
            InitializeComponent();
        }

        public int MainID = 0;
        public string OrderType = "";
        public int id = 0;
        public int detailID = 0;
        public int driverID = 0;
        public string customerName = "";
        public string customerPhone = "";



        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            AddCategory();
            productPanel.Controls.Clear();
            LoadProducts();
        }

        private void AddCategory()
        {
            string qry = "Select * from Category";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            categoryPanel.Controls.Clear();

            foreach (DataRow row in dt.Rows)
            {
                Guna2Button b = new Guna2Button
                {
                    FillColor = Color.FromArgb(50, 55, 89),
                    Size = new Size(197, 45),
                    ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton,
                    Text = row["catName"].ToString()
                };
                b.Click += b_Click;
                categoryPanel.Controls.Add(b);
            }
        }

        private void b_Click(object sender, EventArgs e)
        {
            var b = (Guna2Button)sender;
            foreach (UcProduct pro in productPanel.Controls)
            {
                pro.Visible = pro.PCategory.ToLower().Contains(b.Text.Trim().ToLower());
            }
        }

        private void AddItems(string id, string proID, string name, string cat, string price, Image pimage)
        {
            var w = new UcProduct()
            {
                PName = name,
                PPrice = price,
                PCategory = cat,
                PImage = pimage,
                id = Convert.ToInt32(proID)
            };

            productPanel.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    if (Convert.ToInt32(item.Cells["dgvproID"].Value) == w.id)
                    {
                        item.Cells["dgvQty"].Value = Convert.ToInt32(item.Cells["dgvQty"].Value) + 1;
                        item.Cells["dgvAmount"].Value = Convert.ToInt32(item.Cells["dgvQty"].Value) * Convert.ToDouble(item.Cells["dgvPrice"].Value);
                        GetTotal();
                        return;
                    }
                }
                guna2DataGridView1.Rows.Add(0, 0, w.id, w.PName, 1, w.PPrice, w.PPrice);
                GetTotal();
            };
        }

        private void LoadProducts()
        {
            string qry = "SELECT * FROM products INNER JOIN category ON catID = CategoryID";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                byte[] imageByteArray = (byte[])item["pImage"];
                Image productImage = Image.FromStream(new MemoryStream(imageByteArray));
                AddItems("0", item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(), item["pPrice"].ToString(), productImage);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (UcProduct pro in productPanel.Controls)
            {
                pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.Trim().ToLower());
            }
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int count = 0;
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        private void GetTotal()
        {
            double tot = 0;
            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            {
                if (item.Cells["dgvAmount"].Value != null && double.TryParse(item.Cells["dgvAmount"].Value.ToString(), out double value))
                {
                    tot += value;
                }
            }
            lbTotal.Text = tot.ToString("N2");
        }

        private void bntNew_Click_1(object sender, EventArgs e)
        {
            lbTable.Text = lbWaiter.Text = "";
            lbTable.Visible = lbWaiter.Visible = false;
            guna2DataGridView1.Rows.Clear();
            MainID = 0;
            lbTotal.Text = "0.00";
        }

        private void btnDelivery_Click(object sender, EventArgs e)
        {
            OrderType = "Delivery";
            lbTable.Visible = lbWaiter.Visible = false;

            frmAddCustomer frm = new frmAddCustomer();
            frm.mainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);

            if (frm.txtName.Text != "")
            {
                driverID = frm.driverID;
                lblDriverName.Text = "Customer Name: " + frm.txtName.Text + " Phone: " + frm.txtPhone.Text ;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;
            }
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            OrderType = "Take Away";
            lbTable.Visible = lbWaiter.Visible = false;

            frmAddCustomer frm = new frmAddCustomer();
            frm.mainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);

            if (frm.txtName.Text != "")
            {
                driverID = frm.driverID;
                lblDriverName.Text = "Customer Name: " + frm.txtName.Text + " Phone: " + frm.txtPhone.Text + "Driver: " + frm.cbDriver.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;
            }

        }

        private void btnDinIn_Click(object sender, EventArgs e)
        {
            OrderType = "Din in";
            lblDriverName.Visible = false;
            frmTableSelect frm = new frmTableSelect();
            MainClass.BlurBackground(frm);
            lbTable.Text = frm.TableName;
            lbTable.Visible = !string.IsNullOrEmpty(frm.TableName);

            frmWaiterSelect frm2 = new frmWaiterSelect();
            MainClass.BlurBackground(frm2);
            lbWaiter.Text = frm2.WaiterName;
            lbWaiter.Visible = !string.IsNullOrEmpty(frm2.WaiterName);
        }

        private void btnKOT_Click_1(object sender, EventArgs e)
        {
            string qry1 = MainID == 0 ?
                "INSERT INTO tblMain VALUES(@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change,@driverID,@custName,@custPhone); SELECT SCOPE_IDENTITY();" :
                "UPDATE tblMain SET status = @status, total = @total, received = @received, change = @change WHERE MainID = @ID";

            double total = guna2DataGridView1.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells["dgvAmount"].Value != null)
                .Sum(r => Convert.ToDouble(r.Cells["dgvAmount"].Value));

            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aDate", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lbTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lbWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Pending");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@received", 0.0);
            cmd.Parameters.AddWithValue("@change", 0.0);
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@custName", customerName);
            cmd.Parameters.AddWithValue("@custPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            if (MainID == 0) MainID = Convert.ToInt32(cmd.ExecuteScalar()); else cmd.ExecuteNonQuery();
            MainClass.con.Close();

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string qry2 = Convert.ToInt32(row.Cells["dgvId"].Value) == 0 ?
                    "INSERT INTO tblDetails (MainID, proID, qty, price, amount) VALUES (@MainID, @proID, @qty, @price, @amount)" :
                    "UPDATE tblDetails SET proID = @proID, qty = @qty, price = @price, amount = @amount WHERE DetailID = @ID";

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", row.Cells["dgvId"].Value);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                cmd2.ExecuteNonQuery();
                MainClass.con.Close();
            }

            guna2MessageDialog1.Show("Saved Successfully");
            MainID = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lbTable.Text = "";
            lbWaiter.Text = "";
            lbTable.Visible = false;
            lbWaiter.Visible = false;
            lbTotal.Text = "00";
            lblDriverName.Text = "";

        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            frmBillList frm = new frmBillList();
            MainClass.BlurBackground(frm);
            if (frm.MainID >0)
            {
                id = frm.MainID;
                MainID = frm.MainID;
                LoadEntries();
            }
        }

        private void LoadEntries()
        {
            string qry = $@"SELECT m.orderType, m.TableName, m.WaiterName, 
                           d.DetailID, p.pName, d.proID, d.qty, d.price, d.amount
                    FROM tblMain m
                    INNER JOIN tblDetails d ON m.MainID = d.MainID
                    INNER JOIN products p ON p.PID = d.proID
                    WHERE m.MainID = '{id}'";

            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            guna2DataGridView1.Rows.Clear();

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data found for MainID = " + id);
                return;
            }

            // Safe to access Rows[0] now
            string orderType = dt.Rows[0]["orderType"].ToString();
            if (orderType == "Delivery")
            {
                btnDelivery.Checked = true;
                lbWaiter.Visible = false;
                lbTable.Visible = false;
            }
            else if (orderType == "Take away")
            {
                btnTake.Checked = true;
                lbWaiter.Visible = false;
                lbTable.Visible = false;
            }
            else
            {
                btnDinIn.Checked = true;
                lbWaiter.Visible = true;
                lbTable.Visible = true;
            }

            foreach (DataRow item in dt.Rows)
            {
                lbTable.Text = item["TableName"].ToString();
                lbWaiter.Text = item["WaiterName"].ToString();

                string detailId = item["DetailID"].ToString();
                string proName = item["pName"].ToString();
                string proid = item["proID"].ToString();
                string qty = item["qty"].ToString();
                string price = item["price"].ToString();
                string amount = item["amount"].ToString();

                object[] obj = { 0, detailId, proid, proName, qty, price, amount };
                guna2DataGridView1.Rows.Add(obj);
            }

            GetTotal();
        }


        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            frmCheckOut frm = new frmCheckOut();
            frm.MainID = id;
            frm.amt = Convert.ToDouble(lbTotal.Text);
            MainClass.BlurBackground(frm);

            MainID = 0;
            guna2DataGridView1.Rows.Clear();
            lbTable.Text = "";
            lbWaiter.Text = "";
            lbTable.Visible = false;
            lbWaiter.Visible = false;
            lbTotal.Text = "00";

        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            if (OrderType == "")
            {
                guna2MessageDialog1.Show("Please select order type");
                return;
            }

            string qry1 = MainID == 0 ?
                        "INSERT INTO tblMain VALUES(@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change,@driverID,@custName,@custPhone); SELECT SCOPE_IDENTITY();" :
                        "UPDATE tblMain SET status = @status, total = @total, received = @received, change = @change WHERE MainID = @ID";

            double total = guna2DataGridView1.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells["dgvAmount"].Value != null)
                .Sum(r => Convert.ToDouble(r.Cells["dgvAmount"].Value));

            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aDate", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lbTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lbWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Hold");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@received", 0.0);
            cmd.Parameters.AddWithValue("@change", 0.0);
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@custName", customerName);
            cmd.Parameters.AddWithValue("@custPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            if (MainID == 0) MainID = Convert.ToInt32(cmd.ExecuteScalar()); else cmd.ExecuteNonQuery();
            MainClass.con.Close();

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string qry2 = Convert.ToInt32(row.Cells["dgvId"].Value) == 0 ?
                    "INSERT INTO tblDetails (MainID, proID, qty, price, amount) VALUES (@MainID, @proID, @qty, @price, @amount)" :
                    "UPDATE tblDetails SET proID = @proID, qty = @qty, price = @price, amount = @amount WHERE DetailID = @ID";

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", row.Cells["dgvId"].Value);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                cmd2.ExecuteNonQuery();
                MainClass.con.Close();
            }

            guna2MessageDialog1.Show("Saved Successfully");
            MainID = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lbTable.Text = "";
            lbWaiter.Text = "";
            lbTable.Visible = false;
            lbWaiter.Visible = false;
            lbTotal.Text = "00";
        }
        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (OrderType == "")
            {
                guna2MessageDialog1.Show("Please select order type");
                return;
            }

            // Check if MainID exists
            if (MainID == 0)
            {
                guna2MessageDialog1.Show("No order found to complete.");
                return;
            }

            // Update the status to "Complete"
            string qry1 = "UPDATE tblMain SET status = @status WHERE MainID = @ID";
            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@status", "Complete");

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            // Execute the update query
            cmd.ExecuteNonQuery();
            MainClass.con.Close();

            // Optional: Add logic to notify the user about the status update
            guna2MessageDialog1.Show("Order marked as Complete.");

            // Clear the current order details (if necessary)
            MainID = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lbTable.Text = "";
            lbWaiter.Text = "";
            lbTable.Visible = false;
            lbWaiter.Visible = false;
            lbTotal.Text = "00";
            lblDriverName.Text = "";
        }

    }

}