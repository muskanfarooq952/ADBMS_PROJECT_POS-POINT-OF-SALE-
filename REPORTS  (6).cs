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

namespace POS_ADBMS
{
    public partial class REPORTS___6_ : Form
    {
        // Connection string
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FINAL_POS;Integrated Security=True;";

        public REPORTS___6_()
        {
            InitializeComponent();

            // Manually connect click events (kyunki designer mein events remove kiye hain)
            this.BACK.Click += new System.EventHandler(this.BACK_Click);
            this.NEXT.Click += new System.EventHandler(this.NEXT_Click);
            this.button15.Click += new System.EventHandler(this.button15_Click);
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        }

        // ==================== FORM LOAD ====================
        private void REPORTS___6__Load(object sender, EventArgs e)
        {
            LoadSalesReportToGrid();
            LoadPurchaseHistoryToGrid();
            LoadCustomerNamesToCombo();
            UpdateAllLabels();
        }

        // ==================== LOAD SALES REPORT TO DATAGRIDVIEW 1 ====================
        private void LoadSalesReportToGrid()
        {
            try
            {
                string query = "SELECT ID, ProductName, Quantity, Total, PaymentMethod FROM SalesReport";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvReports1.DataSource = dt;

                    if (dgvReports1.Columns.Count > 0)
                    {
                        dgvReports1.Columns["ID"].HeaderText = "ID";
                        dgvReports1.Columns["ProductName"].HeaderText = "Product Name";
                        dgvReports1.Columns["Quantity"].HeaderText = "Quantity";
                        dgvReports1.Columns["Total"].HeaderText = "Total";
                        dgvReports1.Columns["PaymentMethod"].HeaderText = "Payment Method";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== LOAD PURCHASE HISTORY TO DATAGRIDVIEW 2 ====================
        private void LoadPurchaseHistoryToGrid()
        {
            try
            {
                string query = "SELECT PurchaseID, CustomerName, ProductName, Quantity, Amount FROM Purchase_History";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvReports2.DataSource = dt;

                    if (dgvReports2.Columns.Count > 0)
                    {
                        dgvReports2.Columns["PurchaseID"].HeaderText = "Purchase ID";
                        dgvReports2.Columns["CustomerName"].HeaderText = "Customer Name";
                        dgvReports2.Columns["ProductName"].HeaderText = "Product Name";
                        dgvReports2.Columns["Quantity"].HeaderText = "Quantity";
                        dgvReports2.Columns["Amount"].HeaderText = "Amount";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== LOAD CUSTOMER NAMES TO COMBOBOX ====================
        private void LoadCustomerNamesToCombo()
        {
            try
            {
                string query = "SELECT DISTINCT CustomerName FROM Purchase_History ORDER BY CustomerName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    comboBox1.Items.Clear();

                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr["CustomerName"].ToString());
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== UPDATE ALL LABELS ====================
        private void UpdateAllLabels()
        {
            UpdateTotalSales();
            UpdateTotalTransactions();
            UpdateTotalCustomers();
        }

        // ==================== TOTAL SALES (label10) ====================
        private void UpdateTotalSales()
        {
            try
            {
                string query = "SELECT SUM(Total) FROM SalesReport";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    decimal totalSales = (result == DBNull.Value) ? 0 : Convert.ToDecimal(result);
                    label10.Text = "Total Sales: " + totalSales.ToString("F2") + " rs";
                }
            }
            catch (Exception)
            {
                label10.Text = "Total Sales: 0 rs";
            }
        }

        // ==================== TOTAL TRANSACTIONS (label2) ====================
        private void UpdateTotalTransactions()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM SalesReport";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    label2.Text = "Total Transactions: " + count.ToString();
                }
            }
            catch (Exception)
            {
                label2.Text = "Total Transactions: 0";
            }
        }

        // ==================== TOTAL CUSTOMERS (label3) ====================
        private void UpdateTotalCustomers()
        {
            try
            {
                string query = "SELECT COUNT(DISTINCT CustomerName) FROM Purchase_History";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    label3.Text = "Total Customers: " + count.ToString();
                }
            }
            catch (Exception)
            {
                label3.Text = "Total Customers: 0";
            }
        }

        // ==================== HIGHLIGHT CUSTOMER IN DATAGRIDVIEW 2 ====================
        private void HighlightCustomerInGrid(string customerName)
        {
            foreach (DataGridViewRow row in dgvReports2.Rows)
            {
                if (row.Cells["CustomerName"].Value != null && row.Cells["CustomerName"].Value.ToString() == customerName)
                {
                    dgvReports2.ClearSelection();
                    row.Selected = true;
                    dgvReports2.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        // ==================== COMBOBOX - Customer select karne par highlight ====================
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string customerName = comboBox1.SelectedItem.ToString();
                HighlightCustomerInGrid(customerName);
            }
        }

        // ==================== GENERATE REPORT BUTTON (button15) ====================
        private void button15_Click(object sender, EventArgs e)
        {
            LoadSalesReportToGrid();
            LoadPurchaseHistoryToGrid();
            UpdateAllLabels();
            MessageBox.Show("✅ Report Generated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== VIEW INVENTORY REPORT BUTTON (button1) ====================
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT ProductName, Stock FROM Products ORDER BY ProductName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    StringBuilder inventoryReport = new StringBuilder();
                    inventoryReport.AppendLine("📦 Current Stock Position:");
                    inventoryReport.AppendLine("--------------------------------");

                    while (dr.Read())
                    {
                        string productName = dr["ProductName"].ToString();
                        int stock = Convert.ToInt32(dr["Stock"]);
                        inventoryReport.AppendLine($"- {productName}: {stock} units");
                    }
                    dr.Close();

                    inventoryReport.AppendLine("--------------------------------");
                    MessageBox.Show(inventoryReport.ToString(), "Inventory Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating inventory report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== EXPORT REPORT BUTTON (button2) ====================
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = "Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                string query = "SELECT ID, ProductName, Quantity, Total, PaymentMethod FROM SalesReport";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine("ID,Product Name,Quantity,Total,Payment Method");
                        while (dr.Read())
                        {
                            writer.WriteLine($"{dr["ID"]},{dr["ProductName"]},{dr["Quantity"]},{dr["Total"]},{dr["PaymentMethod"]}");
                        }
                    }
                    dr.Close();
                }

                MessageBox.Show($"💾 Report Exported Successfully!\nFile saved as: {fileName}\nLocation: Desktop", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== BACK BUTTON ====================
        private void BACK_Click(object sender, EventArgs e)
        {
            INVENTARY__5_ inventoryForm = new INVENTARY__5_();
            inventoryForm.Show();
            this.Close();
        }

        // ==================== NEXT BUTTON ====================
        private void NEXT_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}