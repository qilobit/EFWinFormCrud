using System;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CRUD
{
    public partial class Form1 : Form
    {
        Customer customerModel = new Customer();

        public Form1()
        {
            InitializeComponent();
        }

        void clearFields() 
        {
            Console.WriteLine("=== CLEAR FIELDS ===");
            txtName.Text = txtLastName.Text = txtCity.Text = txtAddress.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            customerModel.CustomerId = 0;
            lbEntryDate.Text = "";
            errorProvider1.Clear();
            resetImage();
            
            if(cmbCountry.Items.Count > 0)
                cmbCountry.SelectedItem = cmbCountry.Items[0];
        }

        private void resetImage() 
        {
            if (pictureBox1.Image != null)
            {
                Console.WriteLine("DISPOSE IMAGE");
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            clearFields();
            dgvCustomers.AutoGenerateColumns = false;
            populateDataGridView();
            loadCountries();
        }

        private void loadCountries()
        {
            using (DBEntities db = new DBEntities()) 
            {
                cmbCountry.DataSource = db.Countries.ToList();
                cmbCountry.DisplayMember = "Name";
                cmbCountry.ValueMember = "CountryId";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (invalidFields()) return;

            appIsBusy();

            customerModel.CountryId = Int32.Parse(cmbCountry.SelectedValue.ToString());
            customerModel.FirstName = txtName.Text.Trim();
            customerModel.LastName = txtLastName.Text.Trim();
            customerModel.City = txtCity.Text.Trim();
            customerModel.Address = txtAddress.Text.Trim();

            if (pictureBox1.Image != null)
            {
                MemoryStream stream = new MemoryStream();
                pictureBox1.Image.Save(stream, pictureBox1.Image.RawFormat);
                Byte[] arr = stream.GetBuffer();
                customerModel.Image = arr;
            }

            using (DBEntities dbEntities = new DBEntities()) 
            {
                if (customerModel.CustomerId == 0)
                {
                    customerModel.EntryDate = DateTime.Now;
                    dbEntities.Customers.Add(customerModel);
                }
                else 
                {
                    dbEntities.Entry(customerModel).State = EntityState.Modified;
                }
                
                dbEntities.SaveChanges();
            }
            populateDataGridView();
            clearFields();
            MessageBox.Show("Successfull operation!");
            setAppAsReady();
        }

        private bool invalidFields() 
        {
            bool invalid = false;
            if (txtName.Text == "")
            {
                errorProvider1.SetError(txtName, "Name required");
                invalid = true;
            }

            if (txtLastName.Text == "")
            {
                errorProvider1.SetError(txtLastName, "Last Name required");
                invalid = true;
            }

            if (txtCity.Text == "")
            {
                errorProvider1.SetError(txtCity, "City required");
                invalid = true;
            }

            return invalid;
        }

        private void populateDataGridView(string metadata = "") 
        {
            
            using (DBEntities db = new DBEntities()) 
            {
                if (metadata != "")
                {
                    dgvCustomers.DataSource = db.Customers.Where(c => c.FirstName.Contains(metadata)).ToList<Customer>();
                }
                else 
                {
                    dgvCustomers.DataSource = db.Customers.ToList<Customer>();
                }
                
            }
        }

        private void dgvCustomers_DoubleClick(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow.Index != -1) 
            {
                appIsBusy();
                resetImage();
                customerModel.CustomerId = int.Parse(dgvCustomers.CurrentRow.Cells[0].Value.ToString());

                //  TODO: Se puede ahorrar esta consulta
         
                using (DBEntities db = new DBEntities()) 
                {
                    customerModel = db.Customers.Find(customerModel.CustomerId);

                    txtName.Text = customerModel.FirstName;
                    txtLastName.Text = customerModel.LastName;
                    txtCity.Text = customerModel.City;
                    txtAddress.Text = customerModel.Address;
                    lbEntryDate.Text = customerModel.EntryDate.ToString();

                    if (!customerModel.CountryId.Equals(null)) 
                    {
                        cmbCountry.SelectedValue = customerModel.CountryId;
                    }

                    if (customerModel.Image != null)
                    {
                        byte[] arrB = customerModel.Image;
                        MemoryStream stream = new MemoryStream(arrB);
                        pictureBox1.Image = Image.FromStream(stream);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    }

                }

                btnSave.Text = "Update";
                btnDelete.Enabled = true;
                setAppAsReady();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Confirm delete record?", "Confirmation", MessageBoxButtons.YesNo);
            if (messageBoxResult == DialogResult.Yes) 
            {
                if (customerModel.CustomerId != 0)
                {
                    using (DBEntities dbEntities = new DBEntities())
                    {
                        dbEntities.Entry(customerModel).State = EntityState.Deleted;
                        dbEntities.SaveChanges();
                    }
                    clearFields();
                    populateDataGridView();
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void appIsBusy() 
        {
            btnSave.Text = "Wait..";
            btnSave.Enabled = false;
            btnCancel.Enabled = false;            
            if(btnCancel.Enabled) btnCancel.Enabled = false;
        }
        private void setAppAsReady()
        {
            btnSave.Text = "Save";
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
        }

        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            String metadata = txtSearch.Text.Trim();
            populateDataGridView(metadata);         
        }
    }
}
