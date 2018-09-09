using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibEveryFileExplorer.IO;

namespace KartPhysicalParamEditor
{
    public partial class Form1 : Form
    {
        List<byte[]> rows = new List<byte[]>();
        string[] defaultList =
            {
                "Standard MR",
                "Shooting Star",
                "B-Dasher",
                "Standard DK",
                "Wild Life",
                "Rambi Rider",
                "Standard TD",
                "Mushmellow",
                "Four Wheel Cradle",
                "Standard BW",
                "Hurricane",
                "Tyrant",
                "Standard PC",
                "Light Tripper",
                "Royale",
                "Standard WR",
                "Brute",
                "Dragonfly",
                "Standard YS",
                "Egg 1",
                "Cucumber",
                "Standard LG",
                "Poltergust 4000",
                "Streamliner",
                "Standard DB",
                "Dry Bomber",
                "Banisher",
                "Standard DS",
                "Light Dancer",
                "Power Flower",
                "Standard WL",
                "Gold Mantis",
                "Zipper",
                "Standard RB",
                "ROB BLS",
                "ROB LGS",
                "Standard HH",
            };
        List<string> KartOrderList = new List<string>();
        Tuple<decimal, decimal> returnMax(int columnNumber)
        {
            decimal max, min;
            switch (columnNumber)
            {
                case 0:
                    max = 0.8M;
                    min = 0.2M;
                    break;
                case 5:
                case 6:
                case 10:
                case 11:
                case 12:
                case 13:
                    max = 15.999M;
                    min = 0;
                    break;
                default:
                    max = 65535;
                    min = 0;
                    break;
            }
            return Tuple.Create(max, min);
        }

        void populateDefaultList()
        {
            KartOrderList.Clear();
            foreach (string temp in defaultList)
            {
                KartOrderList.Add(temp);
            }

        }

        void refreshNames()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = "";
            }
            int id = 0;
            foreach (string temp in KartOrderList)
            {
                if (id < dataGridView1.RowCount)
                {
                    dataGridView1.Rows[id].HeaderCell.Value = temp;
                    id++;
                }
                else break;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title ="Open KartPhysicalParam.bin";

            openFileDialog1.DefaultExt = "bin";
            openFileDialog1.Filter = "KartPhysicalParam Files (*.bin)|*.bin";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                byte[] file = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                if (file.Length % 152 != 0)
                {
                    MessageBox.Show("This is not a valid KartPhysicalParam.bin file", "Wrong file!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (file.Length % 152 == 0 && file.Length != 7600) MessageBox.Show("This might not be a correct KartPhysicalParam file. The file will be loaded, but there is no guarantee it's going to be correctly processed.", "Wrong file?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                rows.Clear();
                button2.Enabled = true;
                button3.Enabled = true;

                for (int i = 0; i < file.Length / 152; i++) {
                    byte[] temp = new byte[152];
                    Buffer.BlockCopy(file, i*152, temp, 0, 152);
                    rows.Add(temp);
                }
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                populateDefaultList();

                dataGridView1.Columns.Add("weight", "Weight");
                dataGridView1.Columns.Add("miniturbo", "Mini-Turbo");
                dataGridView1.Columns.Add("topspeed", "Top Speed");
                dataGridView1.Columns.Add("acceleration", "Acceleration");
                dataGridView1.Columns.Add("acceleration2", "Acceleration factor");
                dataGridView1.Columns.Add("acceleration3", "Acceleration factor 2");
                dataGridView1.Columns.Add("deceleration", "Deceleration");
                dataGridView1.Columns.Add("handling", "Handling");
                dataGridView1.Columns.Add("drift", "Drift scale");
                dataGridView1.Columns.Add("turning", "Turning");
                dataGridView1.Columns.Add("offroad3", "Offroad value 3");
                dataGridView1.Columns.Add("offroad2", "Offroad value 2");
                dataGridView1.Columns.Add("offroad1", "Offroad value 1");
                dataGridView1.Columns.Add("offroad4", "Offroad value 4");
                comboBox1.Items.Clear();
                foreach (DataGridViewColumn temp in dataGridView1.Columns)
                {
                    comboBox1.Items.Add(temp.HeaderText);
                }

                foreach (byte[] row in rows)
                    dataGridView1.Rows.Add(
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x0C) / 4096, 3, MidpointRounding.AwayFromZero),
                        IOUtil.ReadU16LE(row, 0x0E),
                        IOUtil.ReadU16LE(row, 0x10),
                        IOUtil.ReadU16LE(row, 0x14),
                        IOUtil.ReadU16LE(row, 0x18),
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x1C) / 4096, 3, MidpointRounding.AwayFromZero),
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x2C) / 4096, 3, MidpointRounding.AwayFromZero),
                        IOUtil.ReadU16LE(row, 0x30),
                        IOUtil.ReadU16LE(row, 0x32),
                        IOUtil.ReadU16LE(row, 0x34),
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x70) / 4096, 3, MidpointRounding.AwayFromZero),
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x74) / 4096, 3, MidpointRounding.AwayFromZero),
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x78) / 4096, 3, MidpointRounding.AwayFromZero),
                        decimal.Round((decimal)IOUtil.ReadU16LE(row, 0x80) / 4096, 3, MidpointRounding.AwayFromZero)
                        );

                refreshNames();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open names of the karts";

            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                KartOrderList.Clear();
                foreach (var line in lines)
                {
                    KartOrderList.Add(line);
                }
                refreshNames();
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var minmax = returnMax(e.ColumnIndex);
            decimal max = minmax.Item1;
            decimal min = minmax.Item2;

            if (Convert.ToDecimal(dataGridView1[e.ColumnIndex, e.RowIndex].Value) > max)
            {
                MessageBox.Show("The value you've typed in is too big!", "Too big!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = max;
            }
            else if (Convert.ToDecimal(dataGridView1[e.ColumnIndex, e.RowIndex].Value) < min)
            {
                MessageBox.Show("The value you've typed in is too small!", "Too small!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = min;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int id = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                IOUtil.WriteU16LE(rows[id], 0x0C, Convert.ToUInt16(Convert.ToDecimal(row.Cells[0].Value) * 4096M));
                IOUtil.WriteU16LE(rows[id], 0x0E, Convert.ToUInt16(row.Cells[1].Value));
                IOUtil.WriteU16LE(rows[id], 0x10, Convert.ToUInt16(row.Cells[2].Value));
                IOUtil.WriteU16LE(rows[id], 0x14, Convert.ToUInt16(row.Cells[3].Value));
                IOUtil.WriteU16LE(rows[id], 0x18, Convert.ToUInt16(row.Cells[4].Value));
                IOUtil.WriteU16LE(rows[id], 0x1C, Convert.ToUInt16(Convert.ToDecimal(row.Cells[5].Value) * 4096M));
                IOUtil.WriteU16LE(rows[id], 0x2C, Convert.ToUInt16(Convert.ToDecimal(row.Cells[6].Value) * 4096M));
                IOUtil.WriteU16LE(rows[id], 0x30, Convert.ToUInt16(row.Cells[7].Value));
                IOUtil.WriteU16LE(rows[id], 0x32, Convert.ToUInt16(row.Cells[8].Value));
                IOUtil.WriteU16LE(rows[id], 0x34, Convert.ToUInt16(row.Cells[9].Value));
                IOUtil.WriteU16LE(rows[id], 0x70, Convert.ToUInt16(Convert.ToDecimal(row.Cells[10].Value) * 4096M));
                IOUtil.WriteU16LE(rows[id], 0x74, Convert.ToUInt16(Convert.ToDecimal(row.Cells[11].Value) * 4096M));
                IOUtil.WriteU16LE(rows[id], 0x78, Convert.ToUInt16(Convert.ToDecimal(row.Cells[12].Value) * 4096M));
                IOUtil.WriteU16LE(rows[id], 0x80, Convert.ToUInt16(Convert.ToDecimal(row.Cells[13].Value) * 4096M));
                id++;
            }

            byte[] export = rows
                .SelectMany(a => a)
                .ToArray();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Open KartPhysicalParam.bin";
            saveFileDialog1.DefaultExt = "bin";
            saveFileDialog1.Filter = "KartPhysicalParam Files (*.bin)|*.bin";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "") System.IO.File.WriteAllBytes(saveFileDialog1.FileName, export);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBar1.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.CellValueChanged += dataGridView1_CellEndEdit;
            foreach (DataGridViewRow temp in dataGridView1.Rows)
            {
                temp.Cells[comboBox1.SelectedIndex].Value = decimal.Round(Convert.ToDecimal(temp.Cells[comboBox1.SelectedIndex].Value) * ((decimal)trackBar1.Value / 100), 3, MidpointRounding.AwayFromZero);
            }
            dataGridView1.CellValueChanged -= dataGridView1_CellEndEdit;
            trackBar1.Value = 100;
            label2.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow temp in dataGridView1.Rows)
            {
                temp.Cells[comboBox1.SelectedIndex].Value = Convert.ToUInt16(temp.Cells[comboBox1.SelectedIndex].Value);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Export a template file";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog1.FileName, true))
                {
                    foreach (string temp in defaultList)
                        writer.WriteLine(temp);
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = (((decimal) trackBar1.Value / 100)).ToString() + "x";
        }
    }
}
