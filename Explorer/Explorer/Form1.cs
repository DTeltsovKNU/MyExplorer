using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Explorer
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> list_files = new Dictionary<string, string>();


        public Form1()
        {
            InitializeComponent();
            treeView1.BeforeExpand += treeView1_BeforeExpand;
            // заполняем дерево дисками
            FillDriveNodes();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        //
        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            listView1.Clear();
            list_files.Clear();
            try
            {
                FillListView(e.Node.FullPath);
                textBox2.Text = e.Node.FullPath;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        //Добавляем узлы в TreeView
        private void FillDriveNodes()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode { Text = drive.Name };
                    FillTreeNode(driveNode, drive.Name);
                    treeView1.Nodes.Add(driveNode);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        // получаем дочерние узлы для определенного узла
        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
                    driveNode.Nodes.Add(dirNode);
                }
            }
            catch (Exception ex) { }
        }

        //Заполняем ListView файлами, что в папке
        private void FillListView(string path)
        {
            string extension1 = "*.html";
            string extension2 = "*.txt";
            string extension3 = "*.xml";
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles(extension1).Union(di.GetFiles(extension2)).Union(di.GetFiles(extension3)))
                {
                    listView1.View = View.List;
                    listView1.Items.Add(file.Name);
                    list_files.Add(file.Name, file.FullName);
                }
            }
            catch (Exception ex) { }
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                string path = list_files[listView1.FocusedItem.Text];
                richTextBox1.Clear();
                richTextBox1.Text = ReadFile(path);
                textBox2.Text = path;
            }
        }


        //Поиск 
        private void button1_Click(object sender, EventArgs e)
        {
            string ftext = textBox1.Text;
            foreach (string t in list_files.Values)
            {
                try
                {
                    string sr = ReadFile(t); 
                        foreach (Match m in Regex.Matches(sr, ftext, RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace))
                        {
                            string s = ReadFile(t); 
                            richTextBox1.Text = s;
                            textBox2.Text = t;
                        }
                }
                catch (Exception ex) { }
            }
        }


        //Сохранение изменений в файле
        private void button2_Click(object sender, EventArgs e)
        {
            string path = list_files[listView1.FocusedItem.Text];
            richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
        }


        //Нахождение слов, что встречаются в тексте один раз
        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            string str = richTextBox1.Text;
            var result = str.Split(new[] { ' ', '.', ',', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(x => x)
                .Where(x => x.Count() == 1)
                .Select(x => x.Key);
            foreach (var item in result)
                textBox3.Text += item + " ";
        }


        //Копирование текста в новый файл без потовторяющихся слов
        private void button4_Click(object sender, EventArgs e)
        {
            string str = richTextBox1.Text;
            string path = treeView1.SelectedNode.FullPath;
            string new_path = path + (@"\New_" + listView1.FocusedItem.Text);
            using(StreamWriter sw = new StreamWriter(new_path))
            {
                var result = str.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .GroupBy(x => x)
                    .Where(x => x.Count() == 1 || x.Count() == 2)
                    .Select(x => x.Key);
                foreach (var item in result)
                    sw.WriteLine(item);
            }
        }


        //Вспомогательная функция для чтения файлов
        private string ReadFile(string path)
        {
            if (Path.GetExtension(path) == ".txt" || Path.GetExtension(path) == ".html")
            {
                File f = new File(new TxtOrHTMLFactory());
                string result = f.Run(path);
                return result;
            }
            else if (Path.GetExtension(path) == ".xml")
            {

                File f = new File(new XMLFactory());
                string result = f.Run(path);
                return result;
            }
            else
                return "Fail";
        }
    }      
}
