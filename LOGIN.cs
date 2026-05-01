using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace POS_ADBMS
{
    public partial class LOGIN : Form
    {
        public LOGIN()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void LOGIN_button_Click(object sender, EventArgs e)
        {
            // Check karo ke field empty to nahi
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Please enter Username and Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Connection string
            string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FINAL_POS;Integrated Security=True;";

            // SQL query - authenticate user
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND PasswordHash = @password";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", textBox2.Text);

                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Login Successful! Welcome " + textBox1.Text, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // PRODUCT MANAGEMENT FORM OPEN KARO
                            PRODUCT_MANAGEMENT_SCREEN__2_ productForm = new PRODUCT_MANAGEMENT_SCREEN__2_();
                            productForm.Show();

                            // Login form hide karo
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox2.Clear();
                            textBox2.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Clear_button_Click(object sender, EventArgs e)
        {
            // Clear button - text boxes clear karo
            textBox1.Clear();
            textBox2.Clear();
            textBox1.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}