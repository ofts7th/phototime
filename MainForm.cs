using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace photoTimer
{
    public partial class MainForm : Form
    {
        PropertyItem timePropertyItem = null;
        PropertyItem[] photoPropertyItems = null;
        int timeItemId = 0x9003;
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private int idx;
        List<string> files;
        private Image img;
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
                return;

            files = new List<string>();
            readDir(this.textBox1.Text, files);

            idx = 0;
            readNextOne();
        }

        private List<string> imgExts = new List<string> { ".jpg" };
        private void readDir(string dir, List<string> files)
        {
            foreach (string sd in Directory.GetDirectories(dir))
            {
                readDir(sd, files);
            }
            foreach (string f in Directory.GetFiles(dir))
            {
                if (imgExts.Contains(Path.GetExtension(f).ToLower()))
                {
                    files.Add(f);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filePath = files[idx - 1];
            timePropertyItem.Value = Encoding.ASCII.GetBytes("20" + this.txtTime.Text.Substring(0, 2) + "-" + this.txtTime.Text.Substring(2, 2) + "-" + this.txtTime.Text.Substring(4, 2) + " 00:00:00" + "\0");
            img.SetPropertyItem(timePropertyItem);
            foreach (PropertyItem i in photoPropertyItems)
            {
                if (i.Id != timeItemId && i.Id != 0x501B)//0x501B: ThumbNailData
                {
                    img.SetPropertyItem(i);
                }
            }
            string dir = Path.Combine(Path.GetDirectoryName(filePath), "temp");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            img.Save(Path.Combine(dir, Path.GetFileName(filePath)));

            readNextOne();
        }

        private void readNextOne()
        {
            if (idx >= files.Count)
            {
                MessageBox.Show("已经是最后一张了");
                return;
            }

            idx++;

            string filePath = files[idx - 1];
            this.label1.Text = "共" + files.Count + "张照片，当前第" + idx + "张," + Path.GetFileName(filePath);
            img = Image.FromFile(filePath);
            this.pictureBox1.Image = img;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            System.Drawing.Image image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "demo.jpg"));
            photoPropertyItems = image.PropertyItems;
            timePropertyItem = image.GetPropertyItem(timeItemId);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            readNextOne();
        }
    }
}

