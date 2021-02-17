using Library.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Window : Form
    {
        public SortableBindingList<Album> Albums = new SortableBindingList<Album>();
        String jsonPath = "Resources/data.json";

        public Window()
            {
                Albums = GetAlbums();
                InitializeComponent();
            }

        private SortableBindingList<Album> GetAlbums()
        {
            var list = new SortableBindingList<Album>();

            using (StreamReader file = File.OpenText(jsonPath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jObject = (JObject)JToken.ReadFrom(reader);

                foreach(var obj in jObject)
                {
                    list.Add(new Album()
                    {
                        albumId = obj.Key,
                        artist = obj.Value.Value<string>("Artist"),
                        title = obj.Value.Value<string>("Title"),
                        cover = obj.Value.Value<string>("Cover"),
                        listenedTo = obj.Value.Value<bool>("Listened to")
                    });
                }
            }

            return list;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Albums;
            dataGridView1.Columns["albumId"].Visible = false;
            dataGridView1.Columns["artist"].HeaderText = "Artiste";
            dataGridView1.Columns["artist"].FillWeight = 4;
            dataGridView1.Columns["title"].HeaderText = "Titre";
            dataGridView1.Columns["title"].FillWeight = 4;
            dataGridView1.Columns["cover"].Visible = false;
            dataGridView1.Columns["listenedTo"].HeaderText = "Écouté";
            dataGridView1.Columns["listenedTo"].SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns["listenedTo"].FillWeight = 1;
        }

        private void fichierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell = this.dataGridView1[1, this.dataGridView1.CurrentCell.RowIndex];
            this.dataGridView1.CurrentCell.Selected = false;

            string json = File.ReadAllText(jsonPath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            
            foreach(Album album in Albums)
            {
                jsonObj[album.albumId]["Listened to"] = album.listenedTo;
                if (album.listenedTo)
                {
                    Console.WriteLine(album.title);
                }
            }
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(jsonPath, output);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int currentRow = dataGridView1.CurrentCell.RowIndex;
            //int currentColumn = dataGridView1.CurrentCell.ColumnIndex;

            Albums = GetAlbums();
            dataGridView1.DataSource = Albums;
            dataGridView1.Update();
            dataGridView1.Refresh();

            //dataGridView1.CurrentCell = dataGridView1[currentColumn, currentRow];
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process proc;

            try
            {
                string batDir = string.Format(Directory.GetCurrentDirectory().ToString() + @"\Resources");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batDir;
                proc.StartInfo.FileName = "script.bat";
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }
    }
}
