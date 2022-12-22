namespace Dance_Dreamers_Administartor
{
    public partial class LoginForm : Form
    {
        private MainForm mainForm;
        private DanceDreamersDao dao;

        public LoginForm(MainForm mainForm, DanceDreamersDao danceDreamers)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.dao = danceDreamers;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dao.Authenticate(textBox1.Text))
            {
                mainForm.Show();
                this.Hide();
            }
            else
            {
                errorProvider1.SetError(textBox1, "Невірний пароль");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();
        }
    }
}
