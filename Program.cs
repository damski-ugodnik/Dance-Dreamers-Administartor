using Dance_Dreamers_Administartor.util;

namespace Dance_Dreamers_Administartor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            DanceDreamersDao dao = new DanceDreamersDao(Constants.DB_CONNECTION_STRING);
            MainForm mainForm = new MainForm(dao, new AddEventForm());
            LoginForm loginForm = new LoginForm(mainForm, dao);
            Application.Run(loginForm);
        }
    }
}