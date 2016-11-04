using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Bazy_danych
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            odswiezListeStudentow();
            listBoxOcena.SelectedIndex = 0;
        }

        //
        static string connectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\Ola\Desktop\Baza_P.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True; MultipleActiveResultSets=True";
        SqlConnection polaczenie = new SqlConnection(connectionString);

        DataClasses1DataContext dc = new DataClasses1DataContext();

        string nazwaTypu = string.Empty;
        string nazwaPrzedmiotu = string.Empty;
        string nazwaKierunku = string.Empty;
        string nazwaGrupy = string.Empty;

        string lista = string.Empty;
        

        //

        private void button1_Click(object sender, EventArgs e)
        {
            FormularzDodawaniaStudenta okno = new FormularzDodawaniaStudenta();
            okno.Show();
        }

        

        public void odswiezListeStudentow()
        {
            
            try
            {
                string komenda = "SELECT imie, nazwisko, indeks, k.nazwa as kierunek, k.rok, g.nazwa as grupa FROM student s, kierunek k, grupa g, indeks i WHERE s.indeks = i.id_indeksu and i.id_kierunku = k.id_kierunku and g.id_grupy = s.id_grupy";
                polaczenie.Open();
                    SqlCommand cmd = new SqlCommand(komenda, polaczenie);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("student");
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt.DefaultView;
                    polaczenie.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            odswiezListeStudentow();
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {

            

            var q =
                from a in dc.GetTable<przedmiot>()
                group a by a.nazwa into aGroup
                select aGroup;

            dataGridViewPrzedmiot.DataSource = q;

        }

        private void dataGridViewPrzedmiot_SelectionChanged(object sender, EventArgs e)
        {
            
            

            foreach (DataGridViewRow row in dataGridViewPrzedmiot.SelectedRows)
                nazwaPrzedmiotu = row.Cells[0].Value.ToString();
            
            var listaTypow =
                (from p in dc.GetTable<przedmiot>()
                from t in dc.GetTable<typ>()
                where p.id_typu == t.id_typu && p.nazwa == nazwaPrzedmiotu
                select new { t.nazwa_typu }).ToList();

            dataGridViewTyp.DataSource = listaTypow;

            
            

            var listaKierunkow =
                (from k in dc.GetTable<kierunek>()
                 group k by k.nazwa into g
                 select new { g.Key }).ToList();

            dataGridViewKierunek.DataSource = listaKierunkow;


        }

        private void dataGridViewTyp_SelectionChanged(object sender, EventArgs e)
        {
            


            foreach (DataGridViewRow row in dataGridViewPrzedmiot.SelectedRows)
                nazwaPrzedmiotu = row.Cells[0].Value.ToString();
            foreach (DataGridViewRow row in dataGridViewTyp.SelectedRows)
                nazwaTypu = row.Cells[0].Value.ToString();

            var listaProwadzacych =
                (from przedm in dc.GetTable<przedmiot>()
                 from prowadz in dc.GetTable<prowadzacy>()
                 from t in dc.GetTable<typ>()
                 where przedm.nazwa == nazwaPrzedmiotu && przedm.id_prowadzacego == prowadz.id_prowadzacego && przedm.id_typu == t.id_typu && t.nazwa_typu == nazwaTypu
                 select new { prowadz.tytul, prowadz.imie, prowadz.nazwisko }).ToList();

            dataGridViewProwadzacy.DataSource = listaProwadzacych;
        }

        private void dataGridViewKierunek_SelectionChanged(object sender, EventArgs e)
        {
            

            foreach (DataGridViewRow row in dataGridViewKierunek.SelectedRows)
                nazwaKierunku = row.Cells[0].Value.ToString();

            var listaRoku =
            (from k in dc.GetTable<kierunek>()
             where k.nazwa == nazwaKierunku
             group k by k.rok into g
             select new { g.Key }).ToList();

            dataGridViewRok.DataSource = listaRoku;

            

        }

        private void dataGridViewRok_SelectionChanged(object sender, EventArgs e)
        {
            var listaGrup =
                (from k in dc.GetTable<kierunek>()
                 from g in dc.GetTable<grupa>()
                 where k.nazwa == nazwaKierunku && k.id_kierunku == g.id_kierunku
                 group g by g.nazwa into grupa
                 select new { grupa.Key}).ToList();

            dataGridViewGrupa.DataSource = listaGrup;
        }

        private void dataGridViewGrupa_SelectionChanged(object sender, EventArgs e)
        {
             foreach (DataGridViewRow row in dataGridViewGrupa.SelectedRows)
                nazwaGrupy = row.Cells[0].Value.ToString();

             var listaStudentow =
                 (from s in dc.GetTable<student>()
                  from i in dc.GetTable<indeks>()
                  from g in dc.GetTable<grupa>()
                  where s.id_grupy == g.id_grupy && g.nazwa == nazwaGrupy && i.id_indeksu == s.indeks
                  select new { s.imie, s.nazwisko, s.indeks }).ToList();

             dataGridViewStudent.DataSource = listaStudentow;
        }

        private void button7_Click(object sender, EventArgs e)
        {
           
            string nazwaTypu = string.Empty;
            foreach (DataGridViewRow row in dataGridViewTyp.SelectedRows)
                nazwaTypu = row.Cells[0].Value.ToString();
            

            int idZal = 0;
              int index = 0;
            int idPrzedm = 0;
            int idGrupy = 0;
            int IdTypu = 0;

            SqlCommand cmd = new SqlCommand ("select COUNT(*) from zaliczenie",polaczenie);
            SqlCommand cmd1 = new SqlCommand("select id_przedmiotu from przedmiot where nazwa like @NAZWAPRZEDMIOTU and id_typu = @IDTYPU ",polaczenie);
            SqlCommand cmd2 = new SqlCommand("select id_grupy from grupa where nazwa like @NAZWAGRUPY",polaczenie);
            SqlCommand cmd0 = new SqlCommand ("select t.id_typu from typ t, przedmiot p where t.id_typu = p.id_typu and t.nazwa_typu like @NAZWATYPU and p.nazwa like @NAZWAPRZEDMIOTU",polaczenie);

            cmd0.Parameters.Add("@NAZWAPRZEDMIOTU", nazwaPrzedmiotu);
            cmd0.Parameters.Add("@NAZWATYPU", nazwaTypu);

            
            cmd2.Parameters.Add("@NAZWAGRUPY", nazwaGrupy);
            polaczenie.Open();
            IdTypu = (int)cmd0.ExecuteScalar();

            cmd1.Parameters.Add("@NAZWAPRZEDMIOTU", nazwaPrzedmiotu);
            cmd1.Parameters.Add("@IDTYPU", IdTypu);
            idPrzedm = (int)cmd1.ExecuteScalar();
            idGrupy = (int)cmd2.ExecuteScalar();
            idZal = (int)cmd.ExecuteScalar();
            polaczenie.Close();
            idZal++;

          

            foreach (DataGridViewRow row in dataGridViewStudent.SelectedRows)
                index = (int)row.Cells[2].Value;

            string komenda = "insert into zaliczenie values (@IDZAL, @INDEKS, @DATA, @NOTA, @IDPRZEDM, @IDG);";
            SqlCommand cmd3 = new SqlCommand(komenda, polaczenie);
            cmd3.Parameters.Add("@IDZAL", idZal);
            cmd3.Parameters.Add("@INDEKS", index);
            cmd3.Parameters.Add("@DATA", dateTimePicker1.Value);
            cmd3.Parameters.Add("@NOTA", listBoxOcena.SelectedItem);
            cmd3.Parameters.Add("@IDPRZEDM", idPrzedm);
            cmd3.Parameters.Add("@IDG", idGrupy);

            try
            {
                polaczenie.Open();
                cmd3.ExecuteNonQuery();

                pictureBox1.Visible = true;
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(1000);
                pictureBox1.Visible = false;
                polaczenie.Close(); 
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
                
        }


        private void tabPage3_Enter(object sender, EventArgs e)
        {
            comboBoxLiczbaNajlepszych.SelectedIndex = 0;
            
            string komenda = "SELECT nazwa FROM kierunek group by nazwa";
            comboBoxSrednieKierunki.Items.Clear();
            

                SqlCommand cmd = new SqlCommand(komenda, polaczenie);




                try
                {

                    polaczenie.Open();
                    SqlDataReader reader2 = cmd.ExecuteReader();

                    while (reader2.Read())
                    {

                        comboBoxSrednieKierunki.Items.Add(reader2["nazwa"].ToString());

                    }


                    comboBoxSrednieKierunki.SelectedIndex = 0;

                    


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    polaczenie.Close();
                }
                
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormularzAktualizacji okno = new FormularzAktualizacji();
            okno.Show();
        }

        private void comboBoxSrednieKierunki_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nazwaKierunku = comboBoxSrednieKierunki.SelectedItem.ToString();
            string komenda1 = "SELECT rok FROM kierunek where nazwa like @NAZWAKIERUNKU group by rok";
            SqlCommand cmd1 = new SqlCommand(komenda1, polaczenie);
            cmd1.Parameters.Add("@NAZWAKIERUNKU", nazwaKierunku);

            try
            {
                if (polaczenie.State != ConnectionState.Open)
                polaczenie.Open();
                SqlDataReader reader = cmd1.ExecuteReader();

                while (reader.Read())
                {

                    comboBoxRok.Items.Add(reader["rok"].ToString());

                }
                polaczenie.Close();
                comboBoxRok.SelectedIndex = 0;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string komenda = "select imie, nazwisko, indeks, g.nazwa as grupa, ROUND(AVG(ocena),4) as średnia from student s, zaliczenie z, grupa g, kierunek k where s.indeks = z.id_indeksu and s.id_grupy = g.id_grupy and g.id_kierunku = k.id_kierunku and k.nazwa like @NAZWAKIERUNKU and rok = @ROK group by imie, nazwisko,  indeks, g.nazwa";
            SqlCommand cmd = new SqlCommand(komenda, polaczenie);

            cmd.Parameters.Add("@NAZWAKIERUNKU", comboBoxSrednieKierunki.SelectedItem.ToString());
            cmd.Parameters.Add("@ROK", comboBoxRok.SelectedItem.ToString());
            try
            {
                polaczenie.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("student");
                adapter.Fill(dt);
                dataGridViewStatystyki.DataSource = dt.DefaultView;
                polaczenie.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string komenda = "select top (@LICZBA) imie, nazwisko, indeks, k.nazwa, rok, ROUND(AVG(ocena),4) as średnia from student s, zaliczenie z, grupa g, kierunek k where s.indeks = z.id_indeksu and s.id_grupy = g.id_grupy and g.id_kierunku = k.id_kierunku group by imie, nazwisko,  indeks, k.nazwa, rok order by średnia desc";
            SqlCommand cmd = new SqlCommand(komenda, polaczenie);
            cmd.Parameters.Add("@LICZBA", Convert.ToInt32(comboBoxLiczbaNajlepszych.SelectedItem.ToString()));

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("student");
                adapter.Fill(dt);
                dataGridViewStatystyki.DataSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            string komenda = "select k.nazwa,rok, ROUND(AVG (ocena),4) as średnia from kierunek k, zaliczenie z, indeks i where k.id_kierunku = i.id_kierunku and z.id_indeksu = i.id_indeksu group by k.nazwa, rok order by średnia desc";
            SqlCommand cmd = new SqlCommand(komenda, polaczenie);

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("student");
                adapter.Fill(dt);
                dataGridViewStatystyki.DataSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string komenda = "select p.nazwa, ROUND(AVG (ocena),4) as średnia from przedmiot p, zaliczenie z where p.id_przedmiotu = z.id_przedmiotu and p.id_typu = 0 group by p.nazwa order by średnia";
            SqlCommand cmd = new SqlCommand(komenda, polaczenie);

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("student");
                adapter.Fill(dt);
                dataGridViewStatystyki.DataSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var wyszukajProwadzacych = dc.prowadzacyPoNazwisku(textBoxNazwisko.Text);
            dataGridViewProwadz.DataSource = wyszukajProwadzacych;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string komenda = "select nazwa, rok, COUNT(i.id_indeksu) as liczbaStudentow from kierunek k, indeks i where k.id_kierunku = i.id_kierunku group by nazwa, rok";
            SqlCommand cmd = new SqlCommand(komenda, polaczenie);

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("student");
                adapter.Fill(dt);
                dataGridViewStatystyki.DataSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
           
            dc.prowadzacyLista(ref lista);

            for (int i = 0; i < lista.Length; i++)
            {
                if (lista[i] == ',')
                {
                    richTextBox1.Text = richTextBox1.Text + lista[i] + "\n";
                    i++;
                }
                else
                    richTextBox1.Text += lista[i];
            }
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string nazwa = saveFileDialog1.FileName;
            File.WriteAllText(nazwa,lista );
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //odczyt stanu procedury i zapis go do zmiennej zwrotnej
            string podanyTytul = textBox1.Text;
            int? liczbaProwadzacych = 0;
            dc.ileProwadzacych(podanyTytul, ref liczbaProwadzacych);

             label11.Text = liczbaProwadzacych.ToString();
        }

        private void button14_Click(object sender, EventArgs e)
        {
           
           XDocument doc = new XDocument();
                doc = XDocument.Load("kierunki.xml");
            
            
             var   wczytywanie = from k in doc.Descendants("kierunek")
                                  select
                                  new 
                                  {
                                      id_kierunku = Convert.ToInt32(k.Element("id").Value),
                                      nazwa = (string)k.Element("name").Value,
                                      rok = Convert.ToInt32(k.Element("ROK").Value),
                                      specjalizacja = (string)k.Element("SPEC")

                                  };


             dataGridViewKierunki.DataSource = wczytywanie.ToList();
            
        }

        



     


        


        


       
    }
}
