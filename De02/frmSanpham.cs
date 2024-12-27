using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using De02.Database;

namespace ProductManagement
{
    public partial class frmSanpham : Form
    {
        private MyDbContext db;
        private List<Sanpham> allProducts;

        public frmSanpham()
        {
            InitializeComponent();
            db = new MyDbContext();
            btTimkiem.Click += new EventHandler(btTimkiem_Click);
        }

        private void frmSanpham_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                allProducts = db.Sanphams.ToList();
                var categories = db.LoaiSPs.ToList();
                cboLoaiSP.DataSource = categories;
                cboLoaiSP.DisplayMember = "TenLoai";
                cboLoaiSP.ValueMember = "MaLoai";
                UpdateListView(allProducts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void UpdateListView(List<Sanpham> products)
        {
            lvSanpham.Items.Clear();
            foreach (var p in products)
            {
                ListViewItem item = new ListViewItem(p.MaSP);
                item.SubItems.Add(p.TenSP);
                item.SubItems.Add(p.Ngaynhap.ToString("dd/MM/yyyy"));
                item.SubItems.Add(db.LoaiSPs.Find(p.MaLoai)?.TenLoai);
                lvSanpham.Items.Add(item);
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSP.Text) || string.IsNullOrEmpty(txtTenSP.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                var newProduct = new Sanpham
                {
                    MaSP = txtMaSP.Text,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtNgaynhap.Value,
                    MaLoai = cboLoaiSP.SelectedValue.ToString()
                };

                db.Sanphams.Add(newProduct);
                db.SaveChanges();
                LoadData();
                ClearFields();
                btLuu.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm sản phẩm: " + ex.Message);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                var product = db.Sanphams.Find(txtMaSP.Text);
                if (product != null)
                {
                    product.TenSP = txtTenSP.Text;
                    product.Ngaynhap = dtNgaynhap.Value;
                    product.MaLoai = cboLoaiSP.SelectedValue.ToString();
                    db.SaveChanges();
                    LoadData();
                    btLuu.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Sản phẩm không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật sản phẩm: " + ex.Message);
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (lvSanpham.SelectedItems.Count == 0) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string maSP = lvSanpham.SelectedItems[0].Text;
                    var product = db.Sanphams.Find(maSP);
                    if (product != null)
                    {
                        db.Sanphams.Remove(product);
                        db.SaveChanges();
                        LoadData();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Sản phẩm không tồn tại.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa sản phẩm: " + ex.Message);
                }
            }
        }

        private void btTimkiem_Click(object sender, EventArgs e)
        {
            string searchText = txtTim.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                UpdateListView(allProducts);
                return;
            }

            var filteredProducts = allProducts
                .Where(p => p.MaSP.Contains(searchText))
                .ToList();

            UpdateListView(filteredProducts);
        }

        private void ClearFields()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = 0;
            txtTim.Clear();
            btLuu.Enabled = false;
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void lvSanpham_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvSanpham.SelectedItems.Count > 0)
            {
                var selectedItem = lvSanpham.SelectedItems[0];
                txtMaSP.Text = selectedItem.Text;
                txtTenSP.Text = selectedItem.SubItems[1].Text;

                string dateString = selectedItem.SubItems[2].Text;
                
            
                DateTime parsedDate = DateTime.ParseExact(dateString, "dd/MM/yyyy", null);
                dtNgaynhap.Value = parsedDate;

                cboLoaiSP.SelectedValue = db.Sanphams.Find(selectedItem.Text).MaLoai;
            }
        }

     
    }
}