namespace Don_t_show_my_mouse_to_teacher
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
                ApplicationConfiguration.Initialize();
                Application.Run(new SettingForm());
        }
    }
}