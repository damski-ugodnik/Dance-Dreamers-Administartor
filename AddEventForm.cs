using Dance_Dreamers_Administartor.model;
using Dance_Dreamers_Administartor.util;

namespace Dance_Dreamers_Administartor
{
    public partial class AddEventForm : Form
    {
        private Event danceEvent;

        public AddEventForm()
        {
            InitializeComponent();
        }

        public Event DanceEvent
        {
            get
            {
                return danceEvent;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!Constants.INPUT_REGEX.IsMatch(nameBox.Text))
            {
                errorProvider1.SetError(nameBox, "Неправильний формат");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (!Uri.IsWellFormedUriString(urlBox.Text, UriKind.RelativeOrAbsolute) && checkBox1.Checked)
            {
                errorProvider1.SetError(urlBox, "Неправильний формат посилання");
                return;
            }
            if (checkBox1.Checked && dateTimePicker2.Checked)
            {
                danceEvent = new Event(null, nameBox.Text, dateTimePicker1.Value, dateTimePicker2.Value, townBox.Text, placeBox.Text, urlBox.Text);
                return;
            }
            else if (checkBox1.Checked)
            {
                danceEvent = new Event(null, nameBox.Text, dateTimePicker1.Value, null, townBox.Text, placeBox.Text, urlBox.Text);
                return;
            } else
            {
                danceEvent = new Event(null, nameBox.Text, dateTimePicker1.Value, null, townBox.Text, placeBox.Text, null);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            urlBox.Enabled = checkBox1.Checked;
        }
    }
}
