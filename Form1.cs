using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace epood
{
    public partial class Form1 : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=HP-CZC2349HV1;Initial Catalog=epood;Integrated Security=True");

        SqlDataAdapter adapter_toode, adapter_kategooria;
        SqlCommand command;

        public Form1()
        {
            InitializeComponent();
            NaitaAndmed();
            Naitakategooriad();
            LisaKategooriat.Click += LisaKategooriat_Click;
            button3.Click += button3_Click;


            KohandaVarve();
            this.BackColor = Color.Black;

        }
        private void KohandaVarve()
        {
            // Kohanda nuppude, etikettide ja tekstikasti värve

            // LisaKategooriat nupu värv
            LisaKategooriat.BackColor = Color.White;
            LisaKategooriat.ForeColor = Color.Black;

           

            // button3 nupu värv
            button1.BackColor = Color.White;
            button1.ForeColor = Color.Black;
            button3.BackColor = Color.White;
            button3.ForeColor = Color.Black;
            button2.BackColor = Color.White;
            button2.ForeColor = Color.Black;
            button4.BackColor = Color.White;
            button4.ForeColor = Color.Black;

            // Etiketi värv
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
            label4.ForeColor = Color.White;
            label5.ForeColor = Color.White;

            // Tekstikasti värv
            textBox1.BackColor = Color.White;
            textBox1.ForeColor = Color.Black;

            textBox2.BackColor = Color.White;
            textBox2.ForeColor = Color.Black;
                
            textBox3.BackColor = Color.White;
            textBox3.ForeColor = Color.Black;
        }


        private void LisaKategooriat_Click(object sender, EventArgs e)
        {
            try
            {
                connect.Open();
                command = new SqlCommand("INSERT INTO KategooriaTable(Kategooria_Nimetus) VALUES (@kat)", connect);
                command.Parameters.AddWithValue("@kat", Kat_Box.Text);
                command.ExecuteNonQuery();
                connect.Close();
                Kat_Box.DataSource = null; 
                Kat_Box.Items.Clear();
                Naitakategooriad();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga kategooria lisamisel: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() != string.Empty && textBox2.Text.Trim() != string.Empty &&
                textBox1.Text.Trim() != string.Empty && Kat_Box.SelectedItem != null)
            {
                try
                {
                    connect.Open();
                    command = new SqlCommand("INSERT INTO ToodeTabel (ToodeNimetus, Hind, Pilt, KategooriadID) VALUES (@toode, @hind, @pilt, @kat)", connect);
                    command.Parameters.AddWithValue("@toode", textBox3.Text);
                    command.Parameters.AddWithValue("@hind", textBox1.Text);
                    command.Parameters.AddWithValue("@pilt", textBox3.Text + ".jpg");
                    command.Parameters.AddWithValue("@kat", Kat_Box.SelectedValue);

                    command.ExecuteNonQuery();
                    NaitaAndmed();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Andmebaasiga viga: " + ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }
            else
            {
                MessageBox.Show("Sisesta andmeid!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Kat_Box.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show("Kas soovite kustutada valitud kategooria?", "Kinnita kustutamine", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();

                        // Проверка наличия связанных записей в ToodeTabel для выбранной категории
                        command = new SqlCommand("SELECT COUNT(*) FROM ToodeTabel WHERE KategooriadID = @katId", connect);
                        command.Parameters.AddWithValue("@katId", Kat_Box.SelectedValue);
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            // Вывод предупреждения
                            MessageBox.Show("Viga kategooria kustutamisel. Kategoorias on seотud tooteid.");
                        }
                        else
                        {
                            // Удаление последней записи из ToodeTabel
                            command = new SqlCommand("DELETE FROM ToodeTabel WHERE Id = (SELECT TOP 1 Id FROM ToodeTabel ORDER BY Id DESC)", connect);
                            command.ExecuteNonQuery();
                            Naitakategooriad();
                            NaitaAndmed();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Viga kategooria kustutamisel: " + ex.Message);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Valige kategooria kustutamiseks");
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {

        }
        





        private void button2_Click(object sender, EventArgs e)
        {
            
            Naitakategooriad();

            
            NaitaAndmed();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                connect.Open();

                // Получаем ID последней записи в ToodeTabel
                command = new SqlCommand("SELECT TOP 1 Id FROM ToodeTabel ORDER BY Id DESC", connect);
                int lastRecordId = (int)command.ExecuteScalar();

                // Удаляем последнюю запись из ToodeTabel
                command = new SqlCommand("DELETE FROM ToodeTabel WHERE Id = @recordId", connect);
                command.Parameters.AddWithValue("@recordId", lastRecordId);
                command.ExecuteNonQuery();

                // Обновляем данные
                Naitakategooriad();
                NaitaAndmed();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga viimase kirje kustutamisel: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                try
                {
                    connect.Open();

                    // Получаем ID выбранной записи в KategooriaTable
                    int selectedCategoryId = (int)comboBox1.SelectedValue;

                    // Обновляем название категории
                    command = new SqlCommand("UPDATE KategooriaTable SET Kategooria_Nimetus = @newName WHERE Id = @categoryId", connect);
                    command.Parameters.AddWithValue("@newName", comboBox1.Text);
                    command.Parameters.AddWithValue("@categoryId", selectedCategoryId);
                    command.ExecuteNonQuery();

                    // Обновляем данные
                    Naitakategooriad();
                    NaitaAndmed();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Viga kategooria nime muutmisel: " + ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }
        }





        public void Naitakategooriad()
        {
            try
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();

                adapter_kategooria = new SqlDataAdapter("SELECT Id, Kategooria_Nimetus FROM KategooriaTable", connect);
                DataTable dt_Kat = new DataTable();
                adapter_kategooria.Fill(dt_Kat);
                Kat_Box.DataSource = dt_Kat;
                Kat_Box.DisplayMember = "Kategooria_Nimetus";
                Kat_Box.ValueMember = "Id";

                // Установите DataSource для comboBox1
                comboBox1.DataSource = dt_Kat;
                comboBox1.DisplayMember = "Kategooria_Nimetus";
                comboBox1.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga kategooriate laadimisel: " + ex.Message);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем значение ячейки с именем категории (замените "Index_Категории" на реальное имя столбца)
                object categoryValue = selectedRow.Cells["Index_Категории"].Value;

                // Устанавливаем значение в comboBox1
                comboBox1.SelectedItem = categoryValue;
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем ID записи
                int recordId = (int)selectedRow.Cells["Id"].Value;

                // Получаем новое значение из comboBox1
                string newCategoryValue = comboBox1.Text;

                // Обновляем значение в базе данных
                UpdateCategoryInDatabase(recordId, newCategoryValue);
            }
        }
        private void UpdateCategoryInDatabase(int recordId, string newCategoryValue)
        {
            try
            {
                connect.Open();

                // Обновляем значение в ToodeTabel
                SqlCommand updateCommand = new SqlCommand("UPDATE ToodeTabel SET KategooriadID = @newCategory WHERE Id = @recordId", connect);
                updateCommand.Parameters.AddWithValue("@newCategory", newCategoryValue);
                updateCommand.Parameters.AddWithValue("@recordId", recordId);
                updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления категории: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }
        }

        string kat;
        SaveFileDialog save;
        OpenFileDialog open;

        string extension = null;   

        private void button5_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            open.InitialDirectory = @"C:\Kasutajad\opilane\source\repos\TARgv22_app\epood\epood\images";
            open.Multiselect = true;
            open.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";

            if (open.ShowDialog() == DialogResult.OK)
            {
                save = new SaveFileDialog();
                save.InitialDirectory = Path.GetFullPath(@"..\..\images");
                extension = Path.GetExtension(open.FileName);
                save.FileName = textBox3.Text + Path.GetExtension(open.FileName);
                save.Filter="Image" + Path.GetExtension(open.FileName)+"|"+Path.GetExtension(open.FileName);
                // Вы можете использовать open.FileNames для получения выбранных файлов
                if (save.ShowDialog()== DialogResult.OK) 
                {
                    File.Copy(save.FileName, open.FileName);
                    pictureBox1.Image=Image.FromFile(save.FileName);
                }
            }
            else
            {
                MessageBox.Show("Puudub toode nimetus");
            }
        }


        

        public void NaitaAndmed()
        {
            try
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();

                
                string query = "SELECT ToodeTabel.Id, ToodeNimetus, Hind, Pilt, Kategooria_Nimetus " +
                               "FROM ToodeTabel " +
                               "JOIN KategooriaTable ON ToodeTabel.KategooriadID = KategooriaTable.Id";

                DataTable dt_Toode = new DataTable();
                adapter_toode = new SqlDataAdapter(query, connect);
                adapter_toode.Fill(dt_Toode);
                dataGridView1.DataSource = dt_Toode;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga andmete laadimisel: " + ex.Message);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }
    }
}