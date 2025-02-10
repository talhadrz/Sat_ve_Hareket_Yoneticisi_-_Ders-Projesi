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
using System.Security.Permissions;
namespace Proje6
{
    public partial class Hareketler : Form
    {
        public Hareketler()
        {
            InitializeComponent();
        }
        SqlConnection bag = new SqlConnection(@"Data Source=LAPTOP-4ROLKKOD;Initial Catalog=Proje6_2;Integrated Security=True;");
        SqlDataAdapter Data = new SqlDataAdapter();
        SqlCommand Komut = new SqlCommand();
        private void Form1_Load(object sender, EventArgs e)
        {
            Komut.Connection = bag;
            TabloDoldur();
            cmbDoldur();
            Komut.Parameters.Add(new SqlParameter("@Urun", SqlDbType.Int) { Value = 0 });
            Komut.Parameters.Add(new SqlParameter("@Musterı", SqlDbType.Int) { Value = 0 });
            Komut.Parameters.Add(new SqlParameter("@Personel", SqlDbType.Int) { Value = 0 });
            Komut.Parameters.Add(new SqlParameter("@Fıyet", SqlDbType.Int) { Value = 0 });
            Komut.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = 0 }); 
            txtId.Text = null;
            txtFıat.Text = null;
        }

        private void TabloDoldur()
        {
            CMBgetır(cmbAd, "Execute Proje6", "Tablo");
        }
        private void cmbDoldur()
        {
            CMBgetır(cmbAd, "select * from URUNLER", "AD", "ID");
            CMBgetır(cmbMusterı, "select * from MUSTERILER", "ADSOYAD", "ID");
            CMBgetır(cmbPersonel, "select * from PERSONELLER", "AD", "ID");
        }

        private void CMBgetır(ComboBox cmb, string kod, string Display = "AD", string Value = "ID")
        {
            Data = new SqlDataAdapter(kod, bag);
            DataTable Tablo = new DataTable();
            Data.Fill(Tablo);
            if (Display == "Tablo")
                dataGridView1.DataSource = Tablo;
            else
            {
                cmb.DisplayMember = Display;
                cmb.ValueMember = Value;
                cmb.DataSource = Tablo;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int satır = dataGridView1.SelectedCells[0].RowIndex;

                txtId.Text = dataGridView1.Rows[satır].Cells[0].Value.ToString();
                cmbAd.Text = dataGridView1.Rows[satır].Cells[1].Value.ToString();
                cmbMusterı.Text = dataGridView1.Rows[satır].Cells[2].Value.ToString();
                cmbPersonel.Text = dataGridView1.Rows[satır].Cells[3].Value.ToString();
                txtFıat.Text = dataGridView1.Rows[satır].Cells[4].Value.ToString();
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (KayıtKontrol())
            {
                txtId.Text = "0";
                bag.Open();
                Komut.CommandText = "insert into HAREKETLER (URUN,MUSTERI,personel,FIYAT) values (@Urun,@Musterı,@Personel,@Fıyet)";
                DegısıklıkKaydet();
            }
        }
        bool KayıtKontrol()
        {
           
            bag.Open();
            Komut.CommandText = "select AD,ALISFIYAT,SATISFIAT from URUNLER";
            Komut.ExecuteNonQuery();
            SqlDataReader reader = Komut.ExecuteReader();
            while (reader.Read())
            {
                string AD = reader.GetString(0);
                int ALISF = int.Parse(reader[1].ToString());
                int SATISF = int.Parse(reader[2].ToString());
                int FIAT = int.Parse(txtFıat.Text);
                if (AD == cmbAd.Text)
                {
                    try
                    {
                        if (FIAT > ALISF & FIAT < SATISF)
                        {
                            return true;
                        }
                        else
                        {
                            if (FIAT > ALISF)
                            {
                                MessageBox.Show($"Fiyatınız Alış Fiyatından Düşük! Lütfen şu aralıkta seçin: {ALISF} - {SATISF}");
                            }
                            else
                            {
                                MessageBox.Show($"Fiyatınız Satış Fiyatından Yüksek! Lütfen şu aralıkta seçin: {ALISF} - {SATISF}");
                            } 
                            return false;
                        }
                    }
                    finally
                    {
                        bag.Close();
                    }
                }
            }
            return true;
        }
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (KayıtKontrol())
            {
                bag.Open();
                Komut.CommandText = "UPDATE HAREKETLER SET URUN=@Urun,MUSTERI=@Musterı,PERSONEL=@Personel,FIYAT=@Fıyet WHERE HAREKETID=@ID";
                DegısıklıkKaydet();
            }
        }
        private void btnSıl_Click(object sender, EventArgs e)
        {
            bag.Open();
            Komut.CommandText = "delete HAREKETLER where HAREKETLER.HAREKETID = @ID";
            DegısıklıkKaydet();
        }
        void DegısıklıkKaydet()
        {
            try
            {
                Komut.Parameters["@urun"].Value = cmbAd.SelectedValue.ToString();
                Komut.Parameters["@Musterı"].Value = cmbMusterı.SelectedValue.ToString();
                Komut.Parameters["@Personel"].Value = cmbPersonel.SelectedValue.ToString();
                Komut.Parameters["@Fıyet"].Value = txtFıat.Text;
                Komut.Parameters["@ID"].Value = txtId.Text;
                Komut.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message, "SQL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen fiyat ve ID alanlarına geçerli bir sayı girin!", "Format Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Eksik bilgi: " + ex.Message, "Null Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bilinmeyen bir hata oluştu: " + ex.Message, "Genel Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (bag.State == ConnectionState.Open)
                {
                    bag.Close();
                    TabloDoldur();
                }
            }
        }
    }
}
