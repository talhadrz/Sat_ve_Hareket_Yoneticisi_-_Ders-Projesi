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
        }

        private void TabloDoldur()
        {

            Data = new SqlDataAdapter("Execute Proje6", bag);
            DataTable dt = new DataTable();
            Data.Fill(dt);
            dataGridView1.DataSource = dt;
            txtId.Text = null;
            txtFıat.Text = null;
        }
        private void cmbDoldur()
        {
            Data = new SqlDataAdapter("select * from URUNLER", bag);
            DataTable Tablo = new DataTable();
            Data.Fill(Tablo);
            cmbAd.DisplayMember = "AD";
            cmbAd.ValueMember = "ID";
            cmbAd.DataSource = Tablo;

            Data = new SqlDataAdapter("select * from MUSTERILER", bag);
            DataTable Tablo2 = new DataTable();
            Data.Fill(Tablo2);
            cmbMusterı.DisplayMember = "ADSOYAD";
            cmbMusterı.ValueMember = "ID";
            cmbMusterı.DataSource = Tablo2;

            Data = new SqlDataAdapter("select * from PERSONELLER", bag);
            DataTable Tablo3 = new DataTable();
            Data.Fill(Tablo3);
            cmbPersonel.DisplayMember = "AD";
            cmbPersonel.ValueMember = "ID";
            cmbPersonel.DataSource = Tablo3;
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
            bag.Open();
            Komut.CommandText = "insert into HAREKETLER (URUN,MUSTERI,personel,FIYAT) values (@Urun,@Musterı,@Personel,@Fıyet)";
            DegısıklıkKaydet();
        }
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            bag.Open();
            Komut.CommandText = "UPDATE HAREKETLER SET URUN=@Urun,MUSTERI=@Musterı,PERSONEL=@Personel,FIYAT=@Fıyet WHERE HAREKETID=@ID";
            DegısıklıkKaydet();
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
