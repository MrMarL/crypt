using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication9
{
    public partial class Form1 : Form
    {
        const string SharedSecret = "rath/Haste. Grace or Discipline for survivability. Grace if with a group of ES users, Discipline if solo. HatrJ*()!G*Hjkasgasikg891gjh";
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            openFileDialog2.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog2.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog3.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            Slovar.fn = openFileDialog1.FileName;
            Slovar.File_To_List();
            load();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Slovar.Poisk(textBox1.Text) != -1)
                MessageBox.Show("Найдено в строке: " + Slovar.Poisk(textBox1.Text).ToString(), "Поиск", MessageBoxButtons.OK); 
            else
                MessageBox.Show("Не найдено.", "Не найдено", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Slovar.Poisk(textBox1.Text) != -1)
                MessageBox.Show("Удалено в строке:" + Slovar.Ydoli(textBox1.Text).ToString(), "Del", MessageBoxButtons.OK);
            else
                MessageBox.Show("Не найдено.", "Не найдено", MessageBoxButtons.OK, MessageBoxIcon.Error);
            load();
        }
        public void load()
        {
            Slovar.list.Sort();
            textBox2.Text = "";
            for (int i = 0; i < Slovar.list.Count; i++)
                if (Slovar.list[i] != "")
                    if (i < Slovar.list.Count-1)
                        textBox2.Text += Slovar.list[i] + Environment.NewLine;
                    else textBox2.Text += Slovar.list[i];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(filename, textBox2.Text);
            MessageBox.Show("Файл сохранен");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Slovar.Poisk(textBox1.Text) == -1)
            {
                Slovar.Dobavka(textBox1.Text);
                MessageBox.Show("Добавлено!", ":D", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Уже существует!", "Повтор", MessageBoxButtons.OK, MessageBoxIcon.Error);
            load();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (Slovar.PoiskPoSlogam(textBox1.Text) != "")
                textBox3.Text = Slovar.PoiskPoSlogam(textBox1.Text);
            else
                MessageBox.Show("Не найдено.", "Не найдено", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (saveFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog2.FileName;
            System.IO.File.WriteAllText(filename, textBox3.Text);
            MessageBox.Show("Файл сохранен");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var encryptedStringAES = Crypto.EncryptStringAES(textBox2.Text, SharedSecret);
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(filename, encryptedStringAES);
            MessageBox.Show("Файл зашифрован");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            string netxt = System.IO.File.ReadAllText(openFileDialog2.FileName);
            textBox2.Text = Crypto.DecryptStringAES(netxt, SharedSecret);
            Slovar.list = new List<string>(textBox2.Lines);
            load();
        }
    }

    class Slovar
    {
        static public string fn = "Словарь.txt";
        static public List<string> list = new List<string>();

        static public bool File_To_List()     // загрузить файл в список
        {
            list.Clear();
            try
            {
                System.IO.StreamReader f = new System.IO.StreamReader(fn);
                while (!f.EndOfStream)
                {
                    list.Add(f.ReadLine());
                }
                f.Close();
                return true;
            }
            catch
            {
                MessageBox.Show("Ошибка доступа к файлу!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        static public int Ydoli(string slovo)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == slovo && slovo != "")
                {
                    list[i] = "";
                    return i;
                }
            }
            return -1;
        }

        static public void Dobavka(string slovo)
        {
            list.Add(slovo);
        }

        static public int Poisk(string slovo)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == slovo && slovo != "")
                {
                    return i;
                }
            }
            return -1;
        }
        static public string PoiskPoSlogam(string slovo)
        {
            string otv = "";
            for (int i = 0; i < list.Count; i++)
            {
                for(int s = 0,q=0; s < slovo.Length; s++){
                    if ((list[i].Length >=slovo.Length) && ((list[i])[s] == slovo[s]) && (slovo != ""))
                    {
                        ++q;
                        if (q == slovo.Length)
                            otv += list[i] + Environment.NewLine;
                    }
                }
            }
            return otv;
        }
    }
}