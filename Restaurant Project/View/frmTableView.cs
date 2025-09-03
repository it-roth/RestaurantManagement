using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restaurant_Project.Model;

namespace Restaurant_Project.View
{
    public partial class frmTableView: SampleView
    {
        public frmTableView()
        {
            InitializeComponent();
        }
        public void GetData()
        {
            string qry = "Select * From tables where tname like '%" + txtSearch.Text + "%' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);
            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void frmTableView_Load(object sender, EventArgs e)
        {
            GetData();
        }
        public override void btnAdd_Click(object sender, EventArgs e)
        {
            //frmTableAdd frm = new frmTableAdd();
            //frm.ShowDialog();
            MainClass.BlurBackground(new frmTableAdd());
            GetData();

        }

        public override void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                frmCategoryAdd frm = new frmCategoryAdd();
                frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvName"].Value);
                frm.ShowDialog();
                GetData(); // Refresh after editing
            }
            //else if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvdel")
            //{
            //    // Confirm before deleting
            //    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
            //    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;

            //    if (guna2MessageDialog1.Show("Are you sure you want to delete?") == DialogResult.Yes)
            //    {
            //        int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
            //        string qry = "Delete from tables where tid = " + id;

            //        Hashtable ht = new Hashtable();
            //        MainClass.SQl(qry, ht); // Execute the delete

            //        guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
            //        guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
            //        guna2MessageDialog1.Show("Deleted successfully");


            //        GetData(); // Refresh grid after deletion
            //    }
            //}

            else if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvdel")
            {
                // Confirm before deleting using standard MessageBox
                DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "Delete from tables where tid = " + id;

                    Hashtable ht = new Hashtable();
                    MainClass.SQl(qry, ht); // Execute the delete

                    MessageBox.Show("Deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    GetData(); // Refresh grid after deletion
                }
            }

        }
    }
}
