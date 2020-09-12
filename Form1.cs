using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace RandomStudent
{
    //名单来自 live.k12top.com 通过筛选获得
    public partial class Form1 : Form
    {
        string[] z12List = null;
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
            string source = "学生名称英文逗号间隔";
            z12List = source.Split(',');        
            InitializeComponent();
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            this.Focus();
            this.DoubleBuffered = true;
#if DEBUG
            //ApplyResult(z12List);
            //textBox1.Text = Resource1.Z12List;
            //this.ApplyResult(z12List);
#endif
        }
        private void ApplyResult(string[] res)
        {
            if (res.Length == 0)
                return;
            StringBuilder sb = new StringBuilder();
            foreach (var item in res)
            {
                //windows的\r\n真的烦，直接\n不就好了么，浪费1个char的内存
                sb.Append($"{item}\r\n");
            }
            textBox1.Text = sb.ToString().Substring(0, sb.ToString().Length - 1);

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
            if (radioButton2.Checked == true)
                this.numericUpDown1.Value = 5;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                this.numericUpDown1.Value = 3;
        }

        bool end = true;
        /// <summary>
        /// //随机数发生器 说不公平的杠精自己看
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //开始
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
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }
        }
        private void PickList()
        {

            //textBox1.Clear();
            int cnt = Convert.ToInt32(numericUpDown1.Value);
            //拷贝静态的字符串数组至动态数字，方便删除操作
            List<string> lst = new List<string>();
            foreach (var item in z12List)
            {
                lst.Add(item);
            }
            string[] generated = new string[cnt];
            for (int i = 0; i < cnt; i++)
            {
                int n = PickNumber(0, lst.Count - 1);
                string name = lst[n];
                generated[i] = name;
                lst.Remove(name);
            }
            ApplyResult(generated);
        }
        /// <summary>
        /// //随机数发生器 说不公平的杠精自己看
        /// </summary>
        private int PickNumber(int min,int max)
        {
            byte[] randomBytes = new byte[4 /*Int32长度*/ ];
            RNGCryptoServiceProvider rngCrypto =new RNGCryptoServiceProvider();
            
            // From MSDN:
            //
            // 摘要:
            //     用经过加密的强随机值序列填充字节数组。
            //
            // 参数:
            //   data:
            //     用经过加密的强随机值序列填充的数组。
            //
            // 异常:
            //   T:System.Security.Cryptography.CryptographicException:
            //     无法获取加密服务提供程序 (CSP)。
            //
            //   T:System.ArgumentNullException:
            //     data 为 null。
            rngCrypto.GetBytes(randomBytes);
            int res = BitConverter.ToInt32(randomBytes, 0);
            return (res > -1 ? res : - res) % (max - min + 1) + min;
        }
    }
}
