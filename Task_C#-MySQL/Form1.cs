using MySql.Data.MySqlClient;

namespace Task_C_MySQL
{
    public partial class Form1 : Form
    {
        static System.Data.DataTable? table;
        static string? bdName;
        static string? tableName;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ToolStripMenuItem saveBtn = new ToolStripMenuItem("Save");

            saveBtn.Click += new EventHandler(saveBtn_Click);
            menuStrip1.Items.Add(saveBtn);
            ToolStripMenuItem exitBtn = new ToolStripMenuItem("Exit");
            exitBtn.Click += new EventHandler(exitBtn_Click);
            menuStrip1.Items.Add(exitBtn);
        }
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                MySqlConnection connEx = GetConnection();
                string filePath = @"C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\autoTableData.sql";
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        MySqlCommand cmd = new MySqlCommand($"select * from {tableName} into outfile \"{filePath}\" fields terminated by ',' lines terminated by '\\n';", connEx);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Table data save into path: {filePath}", "Saved successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand($"select * from {tableName} into outfile \"{filePath}\" fields terminated by ',' lines terminated by '\\n';", connEx);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Table data save into path: {filePath}", "Saved successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Save file error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                connEx.Close();

            });
        }
        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public static MySqlConnection GetConnection()
        {
            string connData = $"Server=localhost;Database={bdName};port=3306;UserId=root;password=Password";
            MySqlConnection connEx = new MySqlConnection(connData);
            try
            {
                connEx.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return connEx;
        }
        async Task RegularUpdate()
        {
            MySqlConnection connEx = GetConnection();
            MySqlCommand cmd;
            while (true)
            {
                try
                {
                    cmd = new MySqlCommand($"select count(*) from {tableName};", connEx);
                    int recordCount = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 0; i < recordCount; i++)
                    {
                        await Task.Run(() =>
                        {
                            cmd = new MySqlCommand($"select id from {tableName} limit @i, 1;", connEx);
                            cmd.Parameters.Add("@i", MySqlDbType.Int32).Value = i;
                            int id = Convert.ToInt32(cmd.ExecuteScalar());
                            Random rand = new Random();
                            cmd = new MySqlCommand($"update {tableName} set speed=@rand where id=@id;", connEx);
                            cmd.Parameters.Add("@rand", MySqlDbType.Int32).Value = Math.Abs(rand.Next() * DateTime.Now.Millisecond) % 110;
                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                            cmd.ExecuteNonQuery();
                        });
                    }
                    MySqlDataAdapter ms_data = new MySqlDataAdapter($"select * from {tableName};", connEx);
                    table = new System.Data.DataTable();
                    ms_data.Fill(table);
                    dataGridView1.DataSource = table;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Regular update error", MessageBoxButtons.OK, MessageBoxIcon.Error); connEx.Close(); }
            }
        }
        private async void insertBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                MySqlConnection connEx = GetConnection();
                try
                {
                    Random rand = new Random();
                    MySqlCommand cmd = new MySqlCommand($"insert into {tableName} (num, mark, color, mileage, speed) values (@Num,@Mark,@Color,@Mileage,@Speed);", connEx);
                    cmd.Parameters.Add("@Num", MySqlDbType.Text).Value = insertNumTB.Text;
                    cmd.Parameters.Add("@Mark", MySqlDbType.Text).Value = insertMarkTB.Text;
                    cmd.Parameters.Add("@Color", MySqlDbType.Text).Value = insertColorTB.Text;
                    cmd.Parameters.Add("@Mileage", MySqlDbType.Text).Value = insertMileageTB.Text;
                    cmd.Parameters.Add("@Speed", MySqlDbType.Int32).Value = rand.Next(110);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Insert error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                connEx.Close();
            });
        }
        private async void searchBtn_Click(object sender, EventArgs e)
        {
            string?[] c = await ReadById();
            if (c[0] != null && c[1] != null && c[2] != null && c[3] != null)
            {
                updateNumTB.Text = c[0];
                updateMarkTB.Text = c[1];
                updateColorTB.Text = c[2];
                updateMileageTB.Text = c[3].Replace(",", ".");
            }
            else { MessageBox.Show("Auto with this ID isn't exist.", "Search error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        async Task<string?[]> ReadById()
        {
            await Task.Delay(0);
            string?[] c = new string?[] { "", "", "", "" };
            MySqlDataReader reader;
            MySqlConnection connEx = GetConnection();
            MySqlCommand cmd = new MySqlCommand($"select num, mark, color, mileage from {tableName} where id=@Id;", connEx);
            cmd.Parameters.Add("@Id", MySqlDbType.Text).Value = updateIdTB.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                c[0] = reader[0].ToString();
                c[1] = reader[1].ToString();
                c[2] = reader[2].ToString();
                c[3] = reader[3].ToString();
            }
            reader.Close();
            connEx.Close();
            return c;
        }
        private async void updateBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                MySqlConnection connEx = GetConnection();
                try
                {
                    MySqlCommand cmd = new MySqlCommand($"update {tableName} set num=@Num, mark=@Mark, color=@Color, mileage=@Mileage where id=@Id;", connEx);
                    cmd.Parameters.Add("@Id", MySqlDbType.Text).Value = updateIdTB.Text;
                    cmd.Parameters.Add("@Num", MySqlDbType.Text).Value = updateNumTB.Text;
                    cmd.Parameters.Add("@Mark", MySqlDbType.Text).Value = updateMarkTB.Text;
                    cmd.Parameters.Add("@Color", MySqlDbType.Text).Value = updateColorTB.Text;
                    cmd.Parameters.Add("@Mileage", MySqlDbType.Text).Value = updateMileageTB.Text;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                connEx.Close();
            });
        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                MySqlConnection connEx = GetConnection();
                try
                {
                    MySqlCommand cmd = new MySqlCommand($"delete from {tableName} where id=@Id;", connEx);
                    cmd.Parameters.Add("@Id", MySqlDbType.Text).Value = deleteIdTB.Text;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Delete error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                connEx.Close();
            });
        }
        private async void connectionBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (bdNameTB.Text != null && tableNameTB.Text != null)
                {
                    bdName = bdNameTB.Text;
                    tableName = tableNameTB.Text;
                    await RegularUpdate();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}