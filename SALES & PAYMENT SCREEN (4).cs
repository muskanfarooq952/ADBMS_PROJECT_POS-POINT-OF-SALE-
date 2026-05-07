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
    public partial class SALES___PAYMENT_SCREEN__4_ : Form
    {
        // Connection string
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FINAL_POS;Integrated Security=True;";

        // DataTable to hold cart items
        DataTable cartTable;
        decimal totalAmount = 0;

        public SALES___PAYMENT_SCREEN__4_()
        {
            InitializeComponent();
            InitializeCartTable();
        }

        // ==================== INITIALIZE CART TABLE ====================
        private void InitializeCartTable()
        {
            cartTable = new DataTable();
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("Price", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("Total", typeof(decimal));
            dgvSales.DataSource = cartTable;
        }

        // ==================== FORM LOAD ====================
        private void SALES___PAYMENT_SCREEN__4__Load(object sender, EventArgs e)
        {
            // NumericUpDown ki maximum value set karo
            numericUpDown1.Maximum = 100;
            numericUpDown1.Minimum = 1;
            numericUpDown1.Value = 1;

            // Total amount label reset
            label5.Text = "0 rs";
            totalAmount = 0;
        }

        // ==================== LOAD PRODUCT NAMES TO COMBOBOX (Optional - Auto suggest ke liye) ====================
        private void LoadProductNames()
        {
            try
            {
                string query = "SELECT ProductName FROM Products";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();
                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    while (dr.Read())
                    {
                        collection.Add(dr["ProductName"].ToString());
                    }
                    textBox1.AutoCompleteCustomSource = collection;
                    textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        // ==================== ADD TO CART (button15) ====================
        private void button15_Click(object sender, EventArgs e)
        {
            // Check empty fields
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter Product Name!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Please enter Price!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price;
            if (!decimal.TryParse(textBox4.Text, out price))
            {
                MessageBox.Show("Invalid Price! Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int quantity = (int)numericUpDown1.Value;
            string productName = textBox1.Text.Trim();
            decimal total = price * quantity;

            // Check if product already exists in cart
            bool productExists = false;
            foreach (DataRow row in cartTable.Rows)
            {
                if (row["ProductName"].ToString().Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    // Update existing product
                    int existingQty = (int)row["Quantity"];
                    int newQty = existingQty + quantity;
                    row["Quantity"] = newQty;
                    row["Total"] = price * newQty;
                    productExists = true;
                    break;
                }
            }

            if (!productExists)
            {
                // Add new product to cart
                cartTable.Rows.Add(productName, price, quantity, total);
            }

            // Update total amount
            UpdateTotalAmount();

            // Clear input fields
            ClearAddFields();

            MessageBox.Show("Item added to cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== CLEAR ADD FIELDS (CLEAR1) ====================
        private void CLEAR1_Click(object sender, EventArgs e)
        {
            ClearAddFields();
        }

        private void ClearAddFields()
        {
            textBox1.Clear();
            textBox4.Clear();
            numericUpDown1.Value = 1;
            textBox1.Focus();
        }

        // ==================== UPDATE TOTAL AMOUNT ====================
        private void UpdateTotalAmount()
        {
            totalAmount = 0;
            foreach (DataRow row in cartTable.Rows)
            {
                totalAmount += Convert.ToDecimal(row["Total"]);
            }
            label5.Text = totalAmount.ToString("F2") + " rs";
        }

        // ==================== REMOVE SELECTED ITEM (button2) ====================
        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvSales.CurrentRow == null)
            {
                MessageBox.Show("Please select an item to remove!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DataRow selectedRow = ((DataRowView)dgvSales.CurrentRow.DataBoundItem).Row;
                cartTable.Rows.Remove(selectedRow);
                UpdateTotalAmount();
                MessageBox.Show("Item removed from cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ==================== EDIT SELECTED ITEM (button1) ====================
        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvSales.CurrentRow == null)
            {
                MessageBox.Show("Please select an item to edit!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow selectedRow = ((DataRowView)dgvSales.CurrentRow.DataBoundItem).Row;

            // Load selected item data to input fields
            textBox1.Text = selectedRow["ProductName"].ToString();
            textBox4.Text = selectedRow["Price"].ToString();
            numericUpDown1.Value = Convert.ToInt32(selectedRow["Quantity"]);

            // Remove the selected item from cart
            cartTable.Rows.Remove(selectedRow);
            UpdateTotalAmount();

            MessageBox.Show("Edit the item and click Add to Cart to save changes!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== DATAGRIDVIEW SELECTION CHANGE ====================
        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Just for selection - no action needed
        }

        // ==================== PROCESS PAYMENT (button3) ====================
        private void button3_Click(object sender, EventArgs e)
        {
            // Check if cart is empty
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty! Please add items before processing payment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if payment method is selected
            if (!radioButton1.Checked && !radioButton2.Checked && !radioButton3.Checked)
            {
                MessageBox.Show("Please select a payment method (Cash, Card, or Online)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Payment successful
            MessageBox.Show("Payment Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== GENERATE RECEIPT (button4) ====================
        private void button4_Click(object sender, EventArgs e)
        {
            // Check if cart is empty
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty! Nothing to generate receipt for.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if payment method is selected
            if (!radioButton1.Checked && !radioButton2.Checked && !radioButton3.Checked)
            {
                MessageBox.Show("Please select a payment method first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get payment method
            string paymentMethod = "";
            if (radioButton1.Checked) paymentMethod = "Cash";
            else if (radioButton2.Checked) paymentMethod = "Card";
            else if (radioButton3.Checked) paymentMethod = "Online";

            // Build receipt
            StringBuilder receipt = new StringBuilder();
            receipt.AppendLine("========== RECEIPT ==========");
            receipt.AppendLine("Items:");
            receipt.AppendLine("--------------------------------");

            foreach (DataRow row in cartTable.Rows)
            {
                string productName = row["ProductName"].ToString();
                int quantity = Convert.ToInt32(row["Quantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                decimal total = Convert.ToDecimal(row["Total"]);
                receipt.AppendLine($"{productName}   x{quantity}   {price:F2}   = {total:F2}");
            }

            receipt.AppendLine("--------------------------------");
            receipt.AppendLine($"Total Amount: {totalAmount:F2} rs");
            receipt.AppendLine($"Payment Method: {paymentMethod}");
            receipt.AppendLine("");
            receipt.AppendLine("Thank You!");
            receipt.AppendLine("==============================");

            MessageBox.Show(receipt.ToString(), "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== SAVE SALE (button5) - SIRF MESSAGE SHOW KAREGA ====================
        private void button5_Click(object sender, EventArgs e)
        {
            // Check karo cart empty to nahi
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("No items to save! Please add items to cart first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Sirf success message show karo
            MessageBox.Show("Sale saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== BACK BUTTON - Customer Management Screen ====================
        private void BACK_Click(object sender, EventArgs e)
        {
            Customer_Mnagement__3_ customerForm = new Customer_Mnagement__3_();
            customerForm.Show();
            this.Close();
        }

        // ==================== NEXT BUTTON - Inventory Screen ====================
        private void NEXT_Click(object sender, EventArgs e)
        {
            INVENTARY__5_ inventoryForm = new INVENTARY__5_();
            inventoryForm.Show();
            this.Close();
        }

        // ==================== RADIO BUTTONS ====================
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void panel5_Paint(object sender, PaintEventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
    }
}