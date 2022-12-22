using Dance_Dreamers_Administartor.model;

namespace Dance_Dreamers_Administartor
{
    public partial class MainForm : Form
    {
        private readonly DanceDreamersDao dao;
        private Event chosenEvent;
        private AddEventForm addEventForm;

        public MainForm(DanceDreamersDao dao, AddEventForm addEventForm)
        {
            this.dao = dao;
            this.addEventForm = addEventForm;
            dao.FileError += new DanceDreamersDao.FileErrorEventHandler(HandleFileError);
            InitializeComponent();
            FetchEvents();
        }

        private void FetchEvents()
        {
            dataGridView1.Rows.Clear();
            List<Event> events = dao.FetchEventsFromDB();
            int i;
            foreach (Event e in events)
            {
                i = dataGridView1.Rows.Add();
                dataGridView1[0, i].Value = e.Name;
                dataGridView1[0, i].Tag = e;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FetchEvents();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            groupBox1.Visible = true;
            chosenEvent = (Event)dataGridView1[e.ColumnIndex, e.RowIndex].Tag;
            groupBox1.Text = chosenEvent.Name;
            startDateBox.Text = chosenEvent.StartDate.ToShortDateString();
            endDateBox.Text = chosenEvent.EndDate != null ? chosenEvent.EndDate.Value.ToShortDateString() : "Не вказано";
            townBox.Text = chosenEvent.Town;
            placeBox.Text = chosenEvent.Place;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Ви впевнені, що бажаєте видалити цей захід?\nУ випадку його видалення будуть видалені усі записи на цей захід", "Ви впевнені?", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                dao.DeleteEvent(chosenEvent.Id.Value);
            }
            FetchEvents();
            groupBox1.Visible = false;
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (dao.GetEnrollmentsFromDBAndWriteToFiles(saveFileDialog1.FileName, chosenEvent.Id.Value))
                {
                    MessageBox.Show("Файл збережено за вказаним шляхом", "Успіх", MessageBoxButtons.OKCancel);
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if(addEventForm.ShowDialog()==DialogResult.OK)
            {
                dao.InsertEvent(addEventForm.DanceEvent);
            }
            FetchEvents();
            groupBox1.Visible = false;
        }

        private void HandleFileError(string msg)
        {
            MessageBox.Show(msg);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}