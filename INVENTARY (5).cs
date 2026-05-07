using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_ADBMS
{
    public partial class INVENTARY__5_ : Form
    {
        // Connection string
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FINAL_POS;Integrated Security=True;";

        public INVENTARY__5_()
        {
            InitializeComponent();
        }

        // ==================== FORM LOAD ====================
        private void INVENTARY__5__Load(object sender, EventArgs e)
        {
            LoadInventoryToGrid();      // DataGridView mein inventory load karo
            LoadProductIDsToCombo();    // ComboBox mein Product IDs load karo
            UpdateAllLabels();          // Sab labels update karo
        }

        // ==================== LOAD INVENTORY TO DATAGRIDVIEW ====================
        private void LoadInventoryToGrid()
        {
            try
            {
                string query = "SELECT InventoryID, ProductID, ProductName, Stock, SoldToday FROM Inventory";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvInventory.DataSource = dt;

                    if (dgvInventory.Columns.Count > 0)
                    {
                        dgvInventory.Columns["InventoryID"].HeaderText = "Inventory ID";
                        dgvInventory.Columns["ProductID"].HeaderText = "Product ID";
                        dgvInventory.Columns["ProductName"].HeaderText = "Product Name";
                        dgvInventory.Columns["Stock"].HeaderText = "Stock";
                        dgvInventory.Columns["SoldToday"].HeaderText = "Sold Today";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== LOAD PRODUCT IDs TO COMBOBOX (Inventory table se) ====================
        private void LoadProductIDsToCombo()
        {
            try
            {
                // YAHAN CHANGE KIYA HAI - Pehle Products tha, ab Inventory hai
                string query = "SELECT ProductID FROM Inventory";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    comboBox1.Items.Clear();

                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr["ProductID"].ToString());
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Product IDs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== UPDATE ALL LABELS ====================
        private void UpdateAllLabels()
        {
            UpdateTotalProducts();
            UpdateSoldToday();
            UpdateLowStockAlerts();
            UpdateInventoryStatus();
        }

        // ==================== TOTAL PRODUCTS (label10) - SIRF NUMBER ====================
        private void UpdateTotalProducts()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Inventory";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    label10.Text = count.ToString();
                }
            }
            catch (Exception ex)
            {
                label10.Text = "0";
            }
        }

        // ==================== SOLD TODAY (label4) - SIRF NUMBER ====================
        private void UpdateSoldToday()
        {
            try
            {
                string query = "SELECT SUM(SoldToday) FROM Inventory";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    int soldToday = (result == DBNull.Value) ? 0 : Convert.ToInt32(result);
                    label4.Text = soldToday.ToString();
                }
            }
            catch (Exception ex)
            {
                label4.Text = "0";
            }
        }

        // ==================== LOW STOCK ALERTS (label3) - SIRF NUMBER ====================
        private void UpdateLowStockAlerts()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Inventory WHERE Stock <= 5";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    label3.Text = count.ToString();
                }
            }
            catch (Exception ex)
            {
                label3.Text = "0";
            }
        }

        // ==================== INVENTORY STATUS (label2) - SIRF NUMBER ====================
        private void UpdateInventoryStatus()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Inventory WHERE Stock > 0";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    label2.Text = count.ToString();
                }
            }
            catch (Exception ex)
            {
                label2.Text = "0";
            }
        }

        // ==================== HIGHLIGHT PRODUCT IN DATAGRIDVIEW ====================
        private void HighlightProductInGrid(string productID)
        {
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                if (row.Cells["ProductID"].Value != null && row.Cells["ProductID"].Value.ToString() == productID)
                {
                    dgvInventory.ClearSelection();
                    row.Selected = true;
                    dgvInventory.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        // ==================== COMBOBOX1 - ID select karne par highlight ====================
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string productID = comboBox1.SelectedItem.ToString();
                HighlightProductInGrid(productID);
            }
        }

        // ==================== UPDATE STOCK BUTTON (button15) ====================
        private void button15_Click(object sender, EventArgs e)
        {
            // Check karo ke product ID select ki hai
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a Product ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check karo ke new quantity enter ki hai
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter new stock quantity!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newStock;
            if (!int.TryParse(textBox1.Text, out newStock))
            {
                MessageBox.Show("Invalid quantity! Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newStock < 0)
            {
                MessageBox.Show("Stock cannot be negative!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string productID = comboBox1.SelectedItem.ToString();
            int oldStock = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Pehle old stock value le lo
                    string getOldStockQuery = "SELECT Stock FROM Inventory WHERE ProductID = @id";
                    SqlCommand getCmd = new SqlCommand(getOldStockQuery, conn);
                    getCmd.Parameters.AddWithValue("@id", productID);
                    object result = getCmd.ExecuteScalar();
                    if (result != null)
                    {
                        oldStock = Convert.ToInt32(result);
                    }

                    // Update Inventory table
                    string updateInventoryQuery = "UPDATE Inventory SET Stock = @stock WHERE ProductID = @id";
                    SqlCommand invCmd = new SqlCommand(updateInventoryQuery, conn);
                    invCmd.Parameters.AddWithValue("@stock", newStock);
                    invCmd.Parameters.AddWithValue("@id", productID);
                    invCmd.ExecuteNonQuery();

                    // Update Products table
                    string updateProductQuery = "UPDATE Products SET Stock = @stock WHERE ProductID = @id";
                    SqlCommand prodCmd = new SqlCommand(updateProductQuery, conn);
                    prodCmd.Parameters.AddWithValue("@stock", newStock);
                    prodCmd.Parameters.AddWithValue("@id", productID);
                    prodCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Stock updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh DataGridView
                LoadInventoryToGrid();

                // Update labels
                UpdateTotalProducts();
                UpdateLowStockAlerts();
                UpdateInventoryStatus();

                // Clear textBox
                textBox1.Clear();

                // Highlight updated product again
                HighlightProductInGrid(productID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== BACK BUTTON - Sales & Payment Screen ====================
        private void BACK_Click(object sender, EventArgs e)
        {
            SALES___PAYMENT_SCREEN__4_ salesForm = new SALES___PAYMENT_SCREEN__4_();
            salesForm.Show();
            this.Close();
        }

        // ==================== NEXT BUTTON - Reports Screen ====================
        private void NEXT_Click(object sender, EventArgs e)
        {
            REPORTS___6_ reportsForm = new REPORTS___6_();
            reportsForm.Show();
            this.Close();
        }

        // ==================== EXTRA EVENT HANDLERS ====================
        private void label1_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void dgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}