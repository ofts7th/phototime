using photoTimer.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace photoTimer
{
    public partial class AddTime : Form
    {
        public AddTime()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            processFolder(this.txtFolder.Text, this.txtFolder.Text);
        }

        private void processFolder(string rootFolder, string folder)
        {
            foreach (string sd in Directory.GetDirectories(folder))
            {
                processFolder(rootFolder, sd);
            }
            foreach (string f in Directory.GetFiles(folder))
            {
                processFile(rootFolder, f);
            }
        }

        private void processFile(string rootFolder, string file)
        {
            Image image = Image.FromFile(file);
            string newFile = rootFolder + "\\new" + file.Substring(rootFolder.Length);
            string newDir = Path.GetDirectoryName(newFile);
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }
            try
            {
                var timePropertyItem = image.GetPropertyItem(0x9003);
                if (timePropertyItem != null)
                {
                    string text = Encoding.ASCII.GetString(timePropertyItem.Value);
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (text.IndexOf(' ') > 0)
                        {
                            text = text.Substring(0, text.IndexOf(' '));
                        }
                        text = text.Replace(':', '-');

                        WaterMarkHelper.AddImageSignText(file, newFile, text, 9, 80, 60, Color.OrangeRed);
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
