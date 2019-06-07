using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;

namespace Conductor
{
    public partial class Form1 : Form
    {
        string path;//Содержит полный путь к выделенному файлу
        string strFullPath;//Содержит текущий выделенный путь
        string[] dirs;//Хранит массив директорий
        public Form1()
        {
            InitializeComponent();//Инициализация компонента - требуемый метод для поддержки конструктора
            treeView1.BeforeSelect += treeView1_BeforeSelect;//Добавляет обработчик treeView1_BeforeSelect, который срабатывает непосредственно перед выделением узла
            treeView1.BeforeSelect += treeView1_BeforeExpand;//Добавляет обработчик treeView1_BeforeExpand, который срабатывает непосредственно перед раскрытия узла
            FillDriveNodes();//Вызываем функцию для загрузки данных в listbox&treeView
        }
       
        void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)// событие перед раскрытием узла
        {
            e.Node.Nodes.Clear(); //Очищаем объекты в текущем узле
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))//Если директория действительно существует 
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);//Получаем все дочерние директории
                    if (dirs.Length != 0)//Если дочерние директории существуют
                    {
                        for (int i = 0; i < dirs.Length; i++)//Перебираем каждую директорию
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);//Создаём новое дерево для дочерней директории
                            FillTreeNode(dirNode, dirs[i]);//Передаём дерево дочерней директории, а также путь к ней
                            e.Node.Nodes.Add(dirNode);//Добавляем ранее созданный узел дерева в конец коллекции узлов дерева
                        }
                    }
                }
            }
            catch { }
        }
      
        void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e) // событие перед выделением узла
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))//Если директория существует
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);//Получаем дочерние директории
                    if (dirs.Length != 0)//Если такие директории есть
                    {
                        for (int i = 0; i < dirs.Length; i++)//Перебираем каждую директорию
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);//Создаём новое дерево для дочерней директории
                            FillTreeNode(dirNode, dirs[i]);//Передаём дерево дочерней директории, а также путь к ней
                            e.Node.Nodes.Add(dirNode);//Добавляем ранее созданный узел дерева в конец коллекции узлов дерева
                        }
                    }
                }
            }
            catch { }
        }
        
        private void FillDriveNodes()// получаем все диски на компьютере
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())//Перебирает доступные диски на пк
                {
                    TreeNode driveNode = new TreeNode(Text = drive.Name);//Инициализируем новый экземпляр System.Windows.Forms.TreeNode класса с заданным текстом метки, а именно названием диска
                    FillTreeNode(driveNode, drive.Name);//Вызываем функцию для компоновки Диска с Директориями в единое древо
                    treeView1.Nodes.Add(driveNode);//Добавляем диски в древо
                }
            }
            catch { }
        }
       
        private void FillTreeNode(TreeNode driveNode, string path)// получаем дочерние узлы для определённого узла
        {
            try
            {
                dirs = Directory.GetDirectories(path);//Получаем все директории из дочернего пути
                foreach (string dir in dirs)//Перебираем директории
                {
                    TreeNode dirNode = new TreeNode();//Создаём новое дерево
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);//Обрезаем путь до последних слешей и получаем Имя Директории
                    driveNode.Nodes.Add(dirNode);//Добавляем в дерево с дисками новое дочернее дерево с именем полученным выше
                }
            }
            catch { }
        }

        private void DisplayFiles(string dirName)//Отображаем файлы на основе директории
        {
            try
            {
                listBox1.Items.Clear();//Очищаем listbox
                DirectoryInfo dir = new DirectoryInfo(dirName);
                if (!dir.Exists)//Если директории нет
                {
                    throw new DirectoryNotFoundException("Не существует каталог" + dirName);
                }
                foreach (FileInfo fi in dir.GetFiles())//Получем файлы из директории
                {
                    listBox1.Items.Add(fi.Name);//Добавляем файлы
                }
            }
            catch { }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) //После выбора элемента
        {
            try
            {
                TreeNode node = e.Node;
                strFullPath = node.FullPath;
                DisplayFiles(strFullPath);
                groupBox2.Visible = false;
                richTextBox1.Text = "";
                pictureBox1.Image = null;
            }
            catch { }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                groupBox2.Visible = true;//Отображаем список файлов
                label5.Text = "Директория: " + treeView1.SelectedNode.FullPath;
                path = Path.Combine(treeView1.SelectedNode.FullPath, listBox1.SelectedItem.ToString());//Объединяет две строки в путь, а именно создаёт полный путь к выделенному файлу
                FileInfo fi = new FileInfo(path);
                label6.Text = "Имя: " + listBox1.SelectedItem.ToString();
                label7.Text = "Размер: " + fi.Length.ToString();
                label8.Text = "Дата создания: " + fi.CreationTime.ToString();
                label9.Text = "Дата последнего доступа: " + fi.LastAccessTime.ToString();
                label10.Text = "Дата последнего изменения: " + fi.LastWriteTime.ToString();
                richTextBox1.Text = "";
                pictureBox1.Image = null;
                string a = fi.Extension;
                if ((a == ".jpg") || (a == ".gif") || (a == ".png"))
                    pictureBox1.Image = Image.FromFile(path);//Загружаем картинку
                else
                    if (a == ".txt" || a == ".ini")//Доступные форматы
                    richTextBox1.Text = File.ReadAllText(path);//Загружаем текст
                else richTextBox1.Text = "Данный формат файла не поддерживается для чтения";                
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringCollection copys = new StringCollection();//Создаём коллекцию с файлами
            copys.Add(path);
            Clipboard.SetFileDropList(copys);//Копируем в буфер файлы из коллекции
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                String crtzip = strFullPath + "\\..\\" + new DirectoryInfo(strFullPath).Name;//Объявляем директорию, выходим из папки
                if (!File.Exists(crtzip + ".zip"))
                {
                    System.IO.Compression.ZipFile.CreateFromDirectory(strFullPath, crtzip + ".zip");//Создаеём zip архив
                }
                else System.IO.Compression.ZipFile.CreateFromDirectory(strFullPath, crtzip + "(1).zip");//Если архив есть в этой папке добавляем (1)
                DisplayFiles(strFullPath);//Обновляем список файлов
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(strFullPath + "\\" + Microsoft.VisualBasic.Interaction.InputBox("Введите название папки:"));
                treeView1.SelectedNode.Collapse();//Обновялем узел
                treeView1.SelectedNode.Expand();//Обновялем узел
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.Delete(strFullPath, true);
                treeView1.CollapseAll();//Обновялем узел
                treeView1.ExpandAll();//Обновялем узел
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(path, strFullPath);//Извлекам файлы из архива
                treeView1.SelectedNode.Collapse();//Обновляем узел
                treeView1.SelectedNode.Expand();//Обновялем узел
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(path);//Удаляем файл
                DisplayFiles(strFullPath);//Обновляем список файлов
            }
            catch { }
        }
    }
}
