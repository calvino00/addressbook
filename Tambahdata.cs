using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // file
using System.Text.RegularExpressions; 
namespace addressbook
{
    public partial class Tambahdata : Form
    {
        bool _result = false;
        bool _addMode = false; // klo true = additem , false = edit item
        people _addrBook = null;
      
        public bool Run(Tambahdata form)
        {
            form.ShowDialog();
            return _result;
        }
        public Tambahdata(bool addMode, people addrBook = null)
        {
            InitializeComponent();
            _addMode = addMode;
            if (addrBook != null)
            {
                _addrBook = addrBook;
                this.txtNama.Text = addrBook.Nama;
                this.txtAlamat.Text = addrBook.Alamat;
                this.txtKota.Text = addrBook.Kota;
                this.txtNoHP.Text = addrBook.NoHP;
                this.dtpTglLahir.Value = addrBook.TanggalLahir.Date;
                this.txtEmail.Text = addrBook.Email;
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // validase 
            Addressbook form = new Addressbook();
            Tambahdata close = new Tambahdata(true);
            if (this.txtNama.Text.Trim() == "")
            {
                MessageBox.Show("nama wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtNama.Focus();
            }
            else if (this.txtAlamat.Text.Trim() == "")
            {
                MessageBox.Show("Alamat wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtAlamat.Focus();
            }
            else if (this.txtKota.Text.Trim() == "")
            {
                MessageBox.Show("Kota wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtKota.Focus();
            }
            else if (this.txtNoHP.Text.Trim() == "")
            {
                MessageBox.Show("No HP wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtNoHP.Focus();
            }
            else if (this.txtEmail.Text.Trim() == "")
            {
                MessageBox.Show("Email wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtEmail.Focus();
            }
            else
            {
                try
                {
                    // simpan data ke file 
                    if (_addMode)
                    {
                        using (var fs = new FileStream("addressbook.csv ", FileMode.Append, FileAccess.Write)) // file Stream untuk menangani data
                        {
                            using (StreamWriter writer = new StreamWriter(fs))
                            {

                                writer.WriteLine($"{txtNama.Text.Trim()};{txtAlamat.Text.Trim()};{txtKota.Text.Trim()};{txtNoHP.Text.Trim()};{dtpTglLahir.Value.ToShortDateString()};{txtEmail.Text.Trim()}"); // nama;alamat, ....
                                
                            }
                        }
                    }
                    else // edit data
                    {
                        string[] fileContent = File.ReadAllLines("addressbook.csv");
                        using (FileStream fs = new FileStream("temporary.csv", FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(fs))
                            {
                                foreach (string line in fileContent)
                                {
                                    string[] arrline = line.Split(';');
                                    if (arrline[0] == _addrBook.Nama && arrline[1] == _addrBook.Alamat && arrline[2] == _addrBook.Kota && arrline[3] == _addrBook.NoHP && Convert.ToDateTime(arrline[4]).Date == _addrBook.TanggalLahir.Date && arrline[5] == _addrBook.Email)
                                    {
                                        writer.WriteLine($"{txtNama.Text.Trim()};{txtAlamat.Text.Trim()};{txtKota.Text.Trim()};{txtNoHP.Text.Trim()};{dtpTglLahir.Value.ToShortDateString()};{txtEmail.Text.Trim()}");
                                    }
                                    else
                                    {
                                        writer.WriteLine(line);
                                    }
                                }
                            }
                        }
                        File.Delete("addressbook.csv");
                        File.Move("temporary.csv", "addressbook.csv");
                    }
                    _result = true;
                    this.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void txtNama_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{tab}");
        }

        private bool EmailIsValid(string emailAddr)
        {
            string emailPattern1 = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(emailPattern1);
            Match match = regex.Match(emailAddr);
            return match.Success;
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (this.txtEmail.Text.Trim() != "")
            {
                if (!EmailIsValid(this.txtEmail.Text))
                {
                    MessageBox.Show("Sorry, data email tidak valid ...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtEmail.Clear();
                    this.txtEmail.Focus();
                }
            }
        }

        private void txtNoHp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == '.' || e.KeyChar == ' ' || e.KeyChar == '-' || e.KeyChar == '(' || e.KeyChar == ')')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Tambahdata_Load(object sender, EventArgs e)
        {

        }
    }
}
