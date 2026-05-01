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
    public partial class Customer_Mnagement__3_ : Form
    {
        // Connection string
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FINAL_POS;Integrated Security=True;";

        public Customer_Mnagement__3_()
        {
            InitializeComponent();
        }

        // ==================== FORM LOAD ====================
        private void Customer_Mnagement__3__Load(object sender, EventArgs e)
        {
            LoadCustomerIDsToCombo();  // ComboBox mein Customer IDs load karo
        }

        // ==================== LOAD CUSTOMER IDs TO COMBOBOX1 ====================
        private void LoadCustomerIDsToCombo()
        {
            try
            {
                string query = "SELECT CustomerID FROM Customers";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    comboBox1.Items.Clear();

                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr["CustomerID"].ToString());
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Customer IDs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== LOAD ALL CUSTOMERS TO DATAGRIDVIEW (View All Customers - button1) ====================
        private void LoadAllCustomersToGrid()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName, PhoneNo, Address, Email FROM Customers";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCustomers.DataSource = dt;

                    if (dgvCustomers.Columns.Count > 0)
                    {
                        dgvCustomers.Columns["CustomerID"].HeaderText = "Customer ID";
                        dgvCustomers.Columns["CustomerName"].HeaderText = "Customer Name";
                        dgvCustomers.Columns["PhoneNo"].HeaderText = "Phone No";
                        dgvCustomers.Columns["Address"].HeaderText = "Address";
                        dgvCustomers.Columns["Email"].HeaderText = "Email";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== VIEW ALL CUSTOMERS BUTTON (button1) ====================
        private void button1_Click(object sender, EventArgs e)
        {
            LoadAllCustomersToGrid();
        }

        // ==================== ADD CUSTOMER (AddCustomer button) ====================
        // TextBox names: textBox4 = Name, textBox3 = PhoneNo, textBox2 = Address, textBox1 = Email
        private void AddCustomer_Click(object sender, EventArgs e)
        {
            // Check empty fields
            if (string.IsNullOrEmpty(textBox4.Text) ||   // Name
                string.IsNullOrEmpty(textBox3.Text) ||   // PhoneNo
                string.IsNullOrEmpty(textBox2.Text) ||   // Address
                string.IsNullOrEmpty(textBox1.Text))     // Email
            {
                MessageBox.Show("Please fill all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate phone number (must be exactly 11 digits)
            string phoneNo = textBox3.Text.Trim();
            if (phoneNo.Length != 11 || !phoneNo.All(char.IsDigit))
            {
                MessageBox.Show("Phone Number must be exactly 11 digits!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate email (must contain @gmail.com)
            string email = textBox1.Text.Trim();
            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email must be a valid Gmail address (example@gmail.com)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Check if customer already exists
                string checkQuery = "SELECT COUNT(*) FROM Customers WHERE CustomerName = @name AND PhoneNo = @phone";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", textBox4.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@phone", phoneNo);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Customer already exists with same Name and Phone Number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insert new customer
                    string insertQuery = "INSERT INTO Customers (CustomerName, PhoneNo, Address, Email) VALUES (@name, @phone, @address, @email)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@name", textBox4.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@phone", phoneNo);
                        insertCmd.Parameters.AddWithValue("@address", textBox2.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@email", email);

                        int rows = insertCmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Customer Added Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh DataGridView
                            LoadAllCustomersToGrid();

                            // Refresh ComboBox
                            LoadCustomerIDsToCombo();

                            // Clear add fields
                            ClearAddCustomerFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== CLEAR ADD CUSTOMER FIELDS (CLEAR1) ====================
        private void CLEAR1_Click(object sender, EventArgs e)
        {
            ClearAddCustomerFields();
        }

        private void ClearAddCustomerFields()
        {
            textBox4.Clear();  // Name
            textBox3.Clear();  // PhoneNo
            textBox2.Clear();  // Address
            textBox1.Clear();  // Email
            textBox4.Focus();
        }

        // ==================== COMBOBOX1 - ID select karne par details show hongi ====================
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string customerID = comboBox1.SelectedItem.ToString();

                try
                {
                    string query = "SELECT * FROM Customers WHERE CustomerID = @id";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", customerID);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            textBox5.Text = dr["CustomerName"].ToString();  // Name
                            textBox6.Text = dr["PhoneNo"].ToString();      // PhoneNo
                            textBox7.Text = dr["Address"].ToString();      // Address
                            textBox8.Text = dr["Email"].ToString();        // Email
                        }
                        else
                        {
                            ClearUpdateCustomerFields();
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

        // ==================== EDIT CUSTOMER (EDIT button) ====================
        private void EDIT_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a Customer ID to update!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textBox5.Text) ||
                string.IsNullOrEmpty(textBox6.Text) ||
                string.IsNullOrEmpty(textBox7.Text) ||
                string.IsNullOrEmpty(textBox8.Text))
            {
                MessageBox.Show("Please fill all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerID = comboBox1.SelectedItem.ToString();

            // Validate phone number (11 digits)
            string phoneNo = textBox6.Text.Trim();
            if (phoneNo.Length != 11 || !phoneNo.All(char.IsDigit))
            {
                MessageBox.Show("Phone Number must be exactly 11 digits!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate email (must contain @gmail.com)
            string email = textBox8.Text.Trim();
            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email must be a valid Gmail address (example@gmail.com)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = "UPDATE Customers SET CustomerName = @name, PhoneNo = @phone, Address = @address, Email = @email WHERE CustomerID = @id";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", customerID);
                    cmd.Parameters.AddWithValue("@name", textBox5.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", phoneNo);
                    cmd.Parameters.AddWithValue("@address", textBox7.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", email);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Customer Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh DataGridView
                        LoadAllCustomersToGrid();

                        // Refresh ComboBox
                        LoadCustomerIDsToCombo();

                        // Clear fields
                        ClearUpdateCustomerFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== DELETE CUSTOMER (DELETE button) ====================
        private void DELETE_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a Customer ID to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerID = comboBox1.SelectedItem.ToString();

            DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Customers WHERE CustomerID = @id";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", customerID);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Customer Deleted Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh DataGridView
                            LoadAllCustomersToGrid();

                            // Refresh ComboBox
                            LoadCustomerIDsToCombo();

                            // Clear fields
                            ClearUpdateCustomerFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==================== CLEAR UPDATE CUSTOMER FIELDS (CLEAR2) ====================
        private void CLEAR2_Click(object sender, EventArgs e)
        {
            ClearUpdateCustomerFields();
        }

        private void ClearUpdateCustomerFields()
        {
            comboBox1.SelectedIndex = -1;
            textBox5.Clear();  // Name
            textBox6.Clear();  // PhoneNo
            textBox7.Clear();  // Address
            textBox8.Clear();  // Email
        }

        // ==================== BACK BUTTON (BACK) - Product Management Screen ====================
        private void BACK_Click(object sender, EventArgs e)
        {
            PRODUCT_MANAGEMENT_SCREEN__2_ productForm = new PRODUCT_MANAGEMENT_SCREEN__2_();
            productForm.Show();
            this.Close();
        }

        // ==================== NEXT BUTTON (NEXT) - Sales & Payment Screen ====================
        private void NEXT_Click(object sender, EventArgs e)
        {
            SALES___PAYMENT_SCREEN__4_ salesForm = new SALES___PAYMENT_SCREEN__4_();
            salesForm.Show();
            this.Close();
        }

        // ==================== EXTRA EVENT HANDLERS ====================
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label27_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label28_Click(object sender, EventArgs e) { }
    }
}