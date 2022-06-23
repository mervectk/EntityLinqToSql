using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityLinqToSql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //urunler tablosundaki verileri listelettik!!.Entity framework kendi entity ve facade oluşturdu
            //otomatik oluşturduğu get ile select komutu oluşturur ve listeleme işlemini yapar.
            KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
            dataGridView1.DataSource = ctx.Urunlers;

            //////////////
            cmbKategori.DataSource = ctx.Kategorilers;
            cmbKategori.DisplayMember = "KategoriAdi";//görünmesini istedğimiz property seçtik
            cmbKategori.ValueMember = "KategoriID"; //selectedvalue da gösterilmesini istediğimiz değerdir
            cmbTedarikci.DataSource = ctx.Tedarikcilers;

            cmbTedarikci.DisplayMember = "SirketAdi";
            cmbTedarikci.ValueMember = "TedarikciID";
         

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
            Urunler urn = new Urunler();
            urn.UrunAdi = txtUrunAdi.Text;
            urn.Fiyat = numFiyat.Value;
            urn.Stok = (short)numStok.Value;
            urn.KategoriID = (int)cmbKategori.SelectedValue; //CAST İŞLEMİ
            urn.TedarikciID = (int)cmbTedarikci.SelectedValue;

            ctx.Urunlers.InsertOnSubmit(urn);
            ctx.SubmitChanges();//komutu olmadan ekleme,silme vb işlemler database de gerçekleşmez,işlem bekler
            MessageBox.Show("Ürün Eklendi");
            dataGridView1.DataSource = ctx.Urunlers;//güncel listeyi yazdırdık

        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            //datagriedview selection mode--full row select yap!!

            //önce seçili satırda urunun ıd alıp sonra silme işlemi yapılır!
            int urunid = (int)dataGridView1.CurrentRow.Cells["UrunId"].Value;
            KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
            Urunler urn=ctx.Urunlers.SingleOrDefault(urun =>urun.UrunID==urunid);//lambda expressions şart verip ürün seçilir
           //=>urun öyle ki....
            
            //singleordefault metotu contextin oluşturduğu sorguda ilk kolonu alır...
            //lambda expressions id almak için kullanılır.Sorgudaki ilk elemanı getirir.(row 0.elemanı alır)
            //Birden fazla eleman gelirse  eleman yoksa hata verir.Eleman yoksa default sayesinde null döner hata oluşmaz!!

            ctx.Urunlers.DeleteOnSubmit(urn);
            ctx.SubmitChanges();
            MessageBox.Show("Ürün Silindi");
            dataGridView1.DataSource = ctx.Urunlers;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            txtUrunAdi.Text = row.Cells["UrunAdi"].Value.ToString();//object tanımlama olduğundan tür dönüşümü(cast) itiyor!!
            txtUrunAdi.Tag = row.Cells["UrunId"].Value;
            //urunid değerini txtboxun tag tutmasını sağlıyoruz
            //yanlışlıkla datagridview başka ürün seçilir güncelleme yapılırsa önceki bilgiler yerine yeni ürün bilgileri güncellenir
            //işte bunu önlemek için tag ıd tutulur!!
            cmbKategori.SelectedValue = row.Cells["KategoriID"].Value;
            cmbTedarikci.SelectedValue = row.Cells["TedarikciID"].Value;
            numFiyat.Value = (decimal)row.Cells["Fiyat"].Value;
            numStok.Value = (short)row.Cells["Stok"].Value;


        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            int urunid = (int)txtUrunAdi.Tag;
            KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
            Urunler urn = ctx.Urunlers.SingleOrDefault(urun => urun.UrunID == urunid);
            urn.UrunAdi= txtUrunAdi.Text;
            urn.Fiyat = numFiyat.Value;
            urn.Stok =(short) numStok.Value;
            urn.TedarikciID = (int)cmbTedarikci.SelectedValue;
            urn.KategoriID = (int)cmbKategori.SelectedValue;
            ctx.SubmitChanges();
            MessageBox.Show("Ürün Güncellendi");
            dataGridView1.DataSource = ctx.Urunlers;


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
            dataGridView1.DataSource = ctx.Urunlers.Where(x => x.UrunAdi.Contains(textBox1.Text));
        }

        private void rdbUrunAdi_CheckedChanged(object sender, EventArgs e)
        {
            KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
            if (rdbUrunAdi.Checked)
                dataGridView1.DataSource = ctx.Urunlers.OrderBy(x => x.UrunAdi);
            //x-urun nesnesi
            else if (radioButton1.Checked)
                dataGridView1.DataSource = ctx.Urunlers.OrderByDescending(x => x.Fiyat);//azalan sırada listeleme yaptık
            else if (rdbStok.Checked)
                dataGridView1.DataSource = ctx.Urunlers.OrderByDescending(x => x.Stok);
        }
    }
}
