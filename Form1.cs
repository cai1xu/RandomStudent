using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Deployment.Internal;

namespace RandomStudent
{
    public partial class Form1 : Form
    {
        string[] z12List = null;
        int[] orders = null;
        public Form1()
        {
            //名单来自 https://live.k12top.com:8025/Moral/Query/QueryStudentList (参数已省略)通过json解析得到

            /*
            
                int cnt = 0;
                var json = JObject.Parse(File.ReadAllText(@"c:\temp\Studentlist"));
                StringBuilder sb = new StringBuilder();
                foreach (var item in json["Data"])
                {
                    if (item["EduYearName"].ToString() == "高2020级" && item["TeacherName"].ToString() == "**乐") 姓名已省略
                    {
                        cnt++;
                        sb.Append(item["Name"].ToString() + ",");
                    }
                }
                _ = sb.ToString();

            */


            z12List = source.Split(',');
            orders = new int[z12List.Length];
            for (var i = 0; i < orders.Length; i++)
                orders[i] = i;
            InitializeComponent();
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            this.Focus();
#if DEBUG
            //ApplyResult(z12List);
            //textBox1.Text = Resource1.Z12List;
            //this.ApplyResult(z12List);
#endif
        }
        private void ApplyResult(string[] res)
        {
            textBox1.Lines = res;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = numericUpDown1.Value < 1 ? 1 : numericUpDown1.Value > z12List.Length ? z12List.Length : numericUpDown1.Value;
            if (numericUpDown1.Value == 3)
            {
                this.radioButton1.Checked = true;
            }
            else if (numericUpDown1.Value == 5)
            {
                this.radioButton2.Checked = true;
            }
            else
            {
                this.radioButton3.Checked = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                this.numericUpDown1.Value = 5;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                this.numericUpDown1.Value = 3;
        }

        bool end = true;
        private void button1_Click(object sender, EventArgs e)
        {
            if (end)
            {
                end = false;
                button1.Text = "停止 生成名单(不重复)";
                begin();
            }
            else
            {
                button1.Text = "开始 生成名单(不重复)";
                end = true;
            }
        }
        private void begin()
        {
            while (!end)
            {
                PickList();
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }
        }
        static readonly RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        private void PickList()
        {
            //textBox1.Clear();
            int cnt = Convert.ToInt32(numericUpDown1.Value);
            string[] generated = new string[cnt];
            byte[] randomBytes = new byte[4];
            for (int i = 0; i < cnt; i++)
            {
                rngCrypto.GetBytes(randomBytes);
                uint res = BitConverter.ToUInt32(randomBytes, 0);
                int n = (int)(res % z12List.Length);
                string name = z12List[orders[n]];
                generated[i] = name;
                Swap(ref orders[n], ref orders[orders.Length - i - 1]);
            }
            //textBox1.Text = string.Join(",", z12List);
            ApplyResult(generated);
        }

        private void Swap(ref int a, ref int b)
        {
            int c = a;
            a = b;
            b = c;
        }

        private void Form1_FormClosing
            (object sender, FormClosingEventArgs e)
        {
            end = true;
        }
    }
}
