using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Bazy_danych
{
    public partial class FormularzAktualizacji : Form
    {
        public FormularzAktualizacji()
        {
            InitializeComponent();
           
        }

        static string connectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\Ola\Desktop\Baza_P.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True"; //@"Integrated Security=SSPI;Initial Catalog=Baza_P;Data Source=(local);";
        SqlConnection polaczenie = new SqlConnection(connectionString);
        string pobierzImie = string.Empty;
        string pobierzNazwisko = string.Empty;
        string pobierzKierunek = string.Empty;
        string pobierzRok = string.Empty;
        string pobierzGrupe = string.Empty;
        int index = 0;

        private void comboBoxKierunek_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxGrupa.Items.Clear();


            string komenda2 = "SELECT grupa.nazwa FROM grupa, kierunek WHERE kierunek.nazwa like @NAZWA_KIERUNKU AND grupa.id_kierunku = kierunek.id_kierunku";


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
                comboBoxGrupa.SelectedIndex = 0;
            }
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
        

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            

            wypelnijKierunki();

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                pobierzImie = row.Cells[0].Value.ToString();
                pobierzNazwisko = row.Cells[1].Value.ToString();
                index = (int)row.Cells[2].Value;
                pobierzKierunek = row.Cells[3].Value.ToString();
                pobierzRok = row.Cells[4].Value.ToString();
                pobierzGrupe = row.Cells[5].Value.ToString();
            }
            textBoxImie.Text = pobierzImie;
            textBoxNazwisko.Text = pobierzNazwisko;
            comboBoxRok.SelectedItem = pobierzRok;
            comboBoxKierunek.SelectedItem = pobierzKierunek;
            comboBoxGrupa.SelectedItem = pobierzGrupe;

            
        }

       

        private void wypelnijKierunki()
        {
            comboBoxKierunek.Items.Clear();
            string komenda = "SELECT nazwa, rok FROM kierunek";

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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int idNowejGrupy = 0;
            int idNowegoKierunku = 0;

            string komenda = "update student set imie = @IMIE, nazwisko = @NAZWISKO where indeks = @INDEKS;";
            

            SqlCommand cmd = new SqlCommand(komenda, polaczenie);
            

            cmd.Parameters.Add("@IMIE", textBoxImie.Text);
            cmd.Parameters.Add("@NAZWISKO", textBoxNazwisko.Text);
            cmd.Parameters.Add("@IDNOWEJGRUPY", idNowejGrupy);
            cmd.Parameters.Add("@INDEKS", index);


            polaczenie.Open();

            try
            {
           
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


        }

       

        
        
    }
}
