using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Bazy_danych
{
    public partial class FormularzDodawaniaStudenta : Form
    {
        public FormularzDodawaniaStudenta()
        {
            InitializeComponent();

            comboBoxRok.SelectedIndex = 0;
            



            string komenda = "SELECT nazwa FROM kierunek group by nazwa";
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(komenda, conn);
               


                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           
                            comboBoxKierunek.Items.Add(reader["nazwa"].ToString());
                        }
                    }

                    comboBoxKierunek.SelectedIndex = 0;
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            zaproponujIndeks();
        }
        //
        static string connectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\Ola\Desktop\Baza_P.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
        SqlConnection polaczenie = new SqlConnection(connectionString);
        string nazwiskoDoUsuniecia = string.Empty;
        string indeksDoUsuniecia = string.Empty;

        private void comboBoxKierunek_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxGrupa.Items.Clear();
            

            string komenda2 = "SELECT grupa.nazwa FROM grupa, kierunek WHERE kierunek.nazwa like @NAZWA_KIERUNKU AND grupa.id_kierunku = kierunek.id_kierunku group by grupa.nazwa";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd2 = new SqlCommand(komenda2, conn);
                try
                {
                    conn.Open();
                    cmd2.Parameters.Add("@NAZWA_KIERUNKU", comboBoxKierunek.SelectedItem.ToString());

                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            comboBoxGrupa.Items.Add(reader["nazwa"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            comboBoxGrupa.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string komenda = "INSERT INTO student VALUES (@IMIE,@NAZWISKO,@INDEKS,@IDGRUPY);";
            string komenda1 = "SELECT id_kierunku FROM kierunek WHERE nazwa like @NAZWAKIERUNKU AND rok = @ROKKIERUNKU";
            
            string komenda3 = "INSERT INTO indeks VALUES (@IDINDEKSU,@IDKIERUNKU);";
            string komenda4 = "SELECT id_grupy FROM grupa WHERE nazwa like @NAZWAGRUPY";
            
            
            int idKierunku = 0;
            int idIndeksu = Convert.ToInt32(textBoxIndeks.Text);
            int idGrupy = 0;
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(komenda, conn);
                SqlCommand cmd1 = new SqlCommand(komenda1, conn);
                
                SqlCommand cmd3 = new SqlCommand(komenda3, conn);
                SqlCommand cmd4 = new SqlCommand(komenda4, conn);
                
                

               try
                {
                    conn.Open();
                    

                    cmd4.Parameters.AddWithValue("@NAZWAGRUPY", comboBoxGrupa.SelectedItem.ToString());
                    

                    idGrupy = (int)cmd4.ExecuteScalar();

                    cmd1.Parameters.AddWithValue("@NAZWAKIERUNKU", comboBoxKierunek.SelectedItem.ToString());
                    cmd1.Parameters.AddWithValue("@ROKKIERUNKU", comboBoxRok.SelectedItem.ToString());

                    idKierunku = (int)cmd1.ExecuteScalar();

                    cmd3.Parameters.AddWithValue("@IDINDEKSU", idIndeksu);
                    cmd3.Parameters.AddWithValue("@IDKIERUNKU", idKierunku);

                    cmd3.ExecuteNonQuery();

                    
                    cmd.Parameters.AddWithValue("@IMIE", textBoxImie.Text);
                    cmd.Parameters.AddWithValue("@NAZWISKO", textBoxNazwisko.Text);
                    cmd.Parameters.AddWithValue("@INDEKS", idIndeksu);
                    cmd.Parameters.AddWithValue("@IDGRUPY", idGrupy);

                    cmd.ExecuteNonQuery();

                    pictureBox1.Visible = true;
                    pictureBox1.Refresh();
                    System.Threading.Thread.Sleep(1000);
                    pictureBox1.Visible = false;

                    
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

               
               textBoxImie.Text = "";
               textBoxNazwisko.Text = "";
               textBoxIndeks.Text = "";


            }

            zaproponujIndeks();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBoxPoNazwisku.Text == "")
                usunPoIndeksie();
            else
                usunPoNazwisku();
            

        }

        private void usunPoNazwisku()
        {
            string nazwiskoDoUsuniecia = textBoxPoNazwisku.Text;


            try
            {
                string komenda = "SELECT imie, nazwisko, indeks, k.nazwa as kierunek, k.rok, g.nazwa as grupa FROM student s, kierunek k, grupa g, indeks i WHERE s.indeks = i.id_indeksu and i.id_kierunku = k.id_kierunku and g.id_grupy = s.id_grupy and s.nazwisko like @NAZWISKO";
                using (SqlConnection polaczenie = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(komenda, polaczenie);
                    cmd.Parameters.AddWithValue("@NAZWISKO", nazwiskoDoUsuniecia);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("student");
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt.DefaultView;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void usunPoIndeksie()
        {
            int indeksDoUsuniecia = Convert.ToInt32(textBoxPoIndeksie.Text);



            try
            {
                string komenda = "SELECT imie, nazwisko, indeks, k.nazwa as kierunek, k.rok, g.nazwa as grupa FROM student s, kierunek k, grupa g, indeks i WHERE s.indeks = i.id_indeksu and i.id_kierunku = k.id_kierunku and g.id_grupy = s.id_grupy and s.indeks = @INDEKS";
                using (SqlConnection polaczenie = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(komenda, polaczenie);
                    cmd.Parameters.AddWithValue("@INDEKS", indeksDoUsuniecia);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("student");
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt.DefaultView;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            try
            {
                string komenda = "delete from student where nazwisko like @NAZWISKO and indeks = @INDEKS";
                string komenda1 = "delete from indeks where id_indeksu = @INDEKS";
               
                
                polaczenie.Open();

                    SqlCommand cmd = new SqlCommand(komenda, polaczenie);
                    SqlCommand cmd1 = new SqlCommand(komenda1, polaczenie);
                 
                    cmd.Parameters.AddWithValue("@NAZWISKO", nazwiskoDoUsuniecia);
                    cmd.Parameters.AddWithValue("@INDEKS", Convert.ToInt32(indeksDoUsuniecia));
                    cmd.ExecuteNonQuery();
                    cmd1.Parameters.AddWithValue("@INDEKS", Convert.ToInt32(indeksDoUsuniecia));
                    
                    cmd1.ExecuteNonQuery();

                    pictureBox2.Visible = true;
                    pictureBox2.Refresh();
                    System.Threading.Thread.Sleep(1000);
                    pictureBox2.Visible = false;

                    textBoxPoNazwisku.Text = "";
                    textBoxPoIndeksie.Text = "";
                    dataGridView1.DataSource = "";
                    polaczenie.Close();
                  

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                nazwiskoDoUsuniecia = row.Cells[1].Value.ToString();
                indeksDoUsuniecia = row.Cells[2].Value.ToString();
            }




        }

        private void zaproponujIndeks()
        {
            string komenda = "select MAX(id_indeksu) from indeks";
            string komenda1 = "select COUNT(*) from indeks";
            int proponowanyIndeks = 0;
            int licznik = 0;
            try
            {
                polaczenie.Open();
                SqlCommand cmd = new SqlCommand(komenda, polaczenie);
                SqlCommand cmd1 = new SqlCommand(komenda1, polaczenie);
                licznik = (int)cmd1.ExecuteScalar();

                if (licznik == 0)
                    textBoxIndeks.Text = "1";
                else
                {
                    proponowanyIndeks = (int)cmd.ExecuteScalar();
                    proponowanyIndeks++;
                    textBoxIndeks.Text = proponowanyIndeks.ToString();
                }
                polaczenie.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        //
    }
}
