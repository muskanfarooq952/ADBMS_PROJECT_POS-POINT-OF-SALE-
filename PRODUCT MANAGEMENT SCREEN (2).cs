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
    public partial class PRODUCT_MANAGEMENT_SCREEN__2_ : Form
    {
        // Connection string
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FINAL_POS;Integrated Security=True;";

        public PRODUCT_MANAGEMENT_SCREEN__2_()
        {
            InitializeComponent();
        }

        // ==================== FORM LOAD ====================
        private void PRODUCT_MANAGEMENT_SCREEN__2__Load(object sender, EventArgs e)
        {
            LoadProductsToGrid();      // DataGridView mein products load karo
            LoadProductIDsToCombo1();  // ComboBox1 mein Product IDs load karo
            LoadProductIDsToCombo2();  // ComboBox2 mein Product IDs load karo
        }

        // ==================== LOAD PRODUCTS TO DATAGRIDVIEW ====================
        private void LoadProductsToGrid()
        {
            try
            {
                string query = "SELECT ProductID, ProductName, Price, ProductType, Stock FROM Products";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvProducts.DataSource = dt;

                    if (dgvProducts.Columns.Count > 0)
                    {
                        dgvProducts.Columns["ProductID"].HeaderText = "Product ID";
                        dgvProducts.Columns["ProductName"].HeaderText = "Product Name";
                        dgvProducts.Columns["Price"].HeaderText = "Price";
                        dgvProducts.Columns["ProductType"].HeaderText = "Product Type";
                        dgvProducts.Columns["Stock"].HeaderText = "Stock";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== LOAD PRODUCT IDs TO COMBOBOX1 (Update/Delete ke liye) ====================
        private void LoadProductIDsToCombo1()
        {
            try
            {
                string query = "SELECT ProductID FROM Products";

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

        // ==================== LOAD PRODUCT IDs TO COMBOBOX2 (Search ke liye) ====================
        private void LoadProductIDsToCombo2()
        {
            try
            {
                string query = "SELECT ProductID FROM Products";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    comboBox2.Items.Clear();

                    while (dr.Read())
                    {
                        comboBox2.Items.Add(dr["ProductID"].ToString());
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Product IDs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== ADD PRODUCT (button15) ====================
        private void button15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TXTNAME.Text) ||
                string.IsNullOrEmpty(textBox3.Text) ||
                string.IsNullOrEmpty(textBox4.Text) ||
                string.IsNullOrEmpty(TXTSTOCK.Text))
            {
                MessageBox.Show("Please fill all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price;
            int stock;

            if (!decimal.TryParse(textBox3.Text, out price))
            {
                MessageBox.Show("Invalid Price! Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(TXTSTOCK.Text, out stock))
            {
                MessageBox.Show("Invalid Stock! Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string checkQuery = "SELECT COUNT(*) FROM Products WHERE ProductName = @name AND ProductType = @type AND Price = @price";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", TXTNAME.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@type", textBox4.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@price", price);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Product already exists with same Name, Type and Price!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string insertQuery = "INSERT INTO Products (ProductName, Price, ProductType, Stock) VALUES (@name, @price, @type, @stock)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@name", TXTNAME.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@price", price);
                        insertCmd.Parameters.AddWithValue("@type", textBox4.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@stock", stock);

                        int rows = insertCmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Product Added Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadProductsToGrid();
                            LoadProductIDsToCombo1();
                            LoadProductIDsToCombo2();
                            ClearAddProductFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== CLEAR ADD PRODUCT FIELDS (CLEAR1) ====================
        private void CLEAR1_Click(object sender, EventArgs e)
        {
            ClearAddProductFields();
        }

        private void ClearAddProductFields()
        {
            TXTNAME.Clear();
            textBox3.Clear();
            textBox4.Clear();
            TXTSTOCK.Clear();
            TXTNAME.Focus();
        }

        // ==================== HIGHLIGHT PRODUCT IN DATAGRIDVIEW BY ID ====================
        private void HighlightProductInGrid(string productID)
        {
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.Cells["ProductID"].Value != null && row.Cells["ProductID"].Value.ToString() == productID)
                {
                    dgvProducts.ClearSelection();
                    row.Selected = true;
                    dgvProducts.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        // ==================== SEARCH BUTTON - Sirf comboBox2 ke liye (Highlight + Message) ====================
        private void SEARCH_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a Product ID from the Search box!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productID = comboBox2.SelectedItem.ToString();

            if (string.IsNullOrEmpty(productID))
            {
                MessageBox.Show("Invalid Product ID selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // DataGridView mein highlight karo
            HighlightProductInGrid(productID);
            MessageBox.Show("Product Found and Highlighted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== COMBOBOX1 - ID select karne par neeche textboxes mein details show hongi (Update/Delete ke liye) ====================
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string productID = comboBox1.SelectedItem.ToString();

                try
                {
                    string query = "SELECT * FROM Products WHERE ProductID = @id";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", productID);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            textBox9.Text = dr["ProductName"].ToString();
                            TXTPRICE2.Text = dr["Price"].ToString();
                            TXTTYPE2.Text = dr["ProductType"].ToString();
                            TXTSTOCK2.Text = dr["Stock"].ToString();
                        }
                        else
                        {
                            ClearUpdateProductFields();
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==================== COMBOBOX2 - Sirf ID select hoga, koi textbox mein data nahi aayega, koi highlight nahi hoga ====================
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // comboBox2 select karne par kuch nahi hoga
            // Sirf ID select hogi, koi action nahi
            // Highlight SEARCH button press karne par hoga
        }

        // ==================== UPDATE PRODUCT (UPDATE button) ====================
        private void UPDATE_Click(object sender, EventArgs e)
        {
            string productID = null;

            if (comboBox1.SelectedItem != null)
            {
                productID = comboBox1.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a Product ID from the dropdown to update!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textBox9.Text) ||
                string.IsNullOrEmpty(TXTPRICE2.Text) ||
                string.IsNullOrEmpty(TXTTYPE2.Text) ||
                string.IsNullOrEmpty(TXTSTOCK2.Text))
            {
                MessageBox.Show("Please fill all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price;
            int stock;

            if (!decimal.TryParse(TXTPRICE2.Text, out price))
            {
                MessageBox.Show("Invalid Price!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(TXTSTOCK2.Text, out stock))
            {
                MessageBox.Show("Invalid Stock!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = "UPDATE Products SET ProductName = @name, Price = @price, ProductType = @type, Stock = @stock WHERE ProductID = @id";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", productID);
                    cmd.Parameters.AddWithValue("@name", textBox9.Text.Trim());
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@type", TXTTYPE2.Text.Trim());
                    cmd.Parameters.AddWithValue("@stock", stock);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Product Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadProductsToGrid();
                        LoadProductIDsToCombo1();
                        LoadProductIDsToCombo2();
                        ClearUpdateProductFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== DELETE PRODUCT (DELETE button) ====================
        private void DELETE_Click(object sender, EventArgs e)
        {
            string productID = null;

            if (comboBox1.SelectedItem != null)
            {
                productID = comboBox1.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Please select a Product ID from the dropdown to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Products WHERE ProductID = @id";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", productID);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Product Deleted Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadProductsToGrid();
                            LoadProductIDsToCombo1();
                            LoadProductIDsToCombo2();
                            ClearUpdateProductFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==================== CLEAR UPDATE PRODUCT FIELDS (CLEAR2) ====================
        private void CLEAR2_Click(object sender, EventArgs e)
        {
            ClearUpdateProductFields();
        }

        private void ClearUpdateProductFields()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            textBox9.Clear();
            TXTPRICE2.Clear();
            TXTTYPE2.Clear();
            TXTSTOCK2.Clear();
        }

        // ==================== BACK BUTTON (button1) ====================
        private void button1_Click(object sender, EventArgs e)
        {
            LOGIN loginForm = new LOGIN();
            loginForm.Show();
            this.Close();
        }

        // ==================== NEXT BUTTON (button2) ====================
        private void button2_Click(object sender, EventArgs e)
        {
            Customer_Mnagement__3_ customerForm = new Customer_Mnagement__3_();
            customerForm.Show();
            this.Close();
        }

        // ==================== EXTRA EVENT HANDLERS ====================
        private void label1_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void label28_Click(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void button16_Click(object sender, EventArgs e) { }
        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void button17_Click(object sender, EventArgs e) { }
        private void label36_Click(object sender, EventArgs e) { }
        private void label37_Click(object sender, EventArgs e) { }
        private void textBox9_TextChanged(object sender, EventArgs e) { }
        private void TXTNAME_TextChanged(object sender, EventArgs e) { }
        private void TXTSTOCK_TextChanged(object sender, EventArgs e) { }
        private void TXTPRICE2_TextChanged(object sender, EventArgs e) { }
        private void TXTTYPE2_TextChanged(object sender, EventArgs e) { }
        private void TXTSTOCK2_TextChanged(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint_1(object sender, PaintEventArgs e) { }
    }
}