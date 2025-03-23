using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace Don_t_show_my_mouse_to_teacher
{
    public partial class SettingForm : Form
    {
        #region ԭ�д������
        private bool is_hide;
        private bool LPshow = false;
        #endregion

        #region Win32 API
        [DllImport("user32.dll")]
        private static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const uint OCR_NORMAL = 32512;
        private const uint SPI_SETCURSORS = 0x0057;
        public const uint SPIF_SENDWININICHANGE = 2;
        #endregion

        #region �����Ʊ���
        private IntPtr _originalCursor = IntPtr.Zero;
        private IntPtr _transparentCursor = IntPtr.Zero;
        private GlobalMouseHook _mouseHook;
        #endregion
        public SettingForm()
        {

            CheckAdminPrivileges();
            SaveCursorToTemp();
            InitializeMouseHook();
            InitializeComponent();
        }
        #region ���ع��ʵ��
        private void CheckAdminPrivileges()
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("���Թ���ԱȨ�����б�����", "Ȩ�޴���",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
        public static void SaveCursorToTemp()
        {
            try
            {
                // 1. ��ȡ���򼯺���ʱ·��
                var assembly = Assembly.GetExecutingAssembly();
                string tempPath = Path.GetTempPath();

                // 2. ������Դ���ƣ������ļ�ʵ��·��������
                string resourceName = "Don_t_show_my_mouse_to_teacher.empty.cur";
                // 3. �������·��
                string outputPath = Path.Combine(tempPath, "empty.cur");

                // 4. ��ȡǶ����Դ������
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new FileNotFoundException($"��Դ '{resourceName}' δ�ҵ���");

                    using (FileStream fileStream = File.Create(outputPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"����{ex.Message}");
            }
        }
        private void InitializeMouseHook()
        {
            try
            {
                _mouseHook = new GlobalMouseHook();
                _mouseHook.MouseAction += (s, e) =>
                {
                    if (enablehide.Checked && e.Button == MouseButtons.Middle && e.EventType == MouseEventType.Down)
                    {
                        HideCursor();
                    }
                    if (longpress2display.Checked && e.Button == MouseButtons.Right && e.EventType == MouseEventType.Down)
                    {
                        is_hide = false;
                        SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
                    }
                    if (longpress2display.Checked && e.Button == MouseButtons.Right && e.EventType == MouseEventType.Up && !is_hide)
                    {
                        is_hide = true;
                        var cursorFile = Path.Combine(Path.GetTempPath(), "empty.cur");
                        _transparentCursor = LoadCursorFromFile(cursorFile);
                        SetSystemCursor(_transparentCursor, OCR_NORMAL);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��깳�ӳ�ʼ��ʧ��: {ex.Message}");
            }
        }
        private void HideCursor()
        {
            if (is_hide)  // ʹ��ԭ��hide��������״̬
            {
                is_hide = false;
                SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
            }
            else
            {
                is_hide = true;
                var cursorFile = Path.Combine(Path.GetTempPath(), "empty.cur");
                _transparentCursor = LoadCursorFromFile(cursorFile);
                SetSystemCursor(_transparentCursor, OCR_NORMAL);
            }
        }
        #endregion
        #region ����ͼ��
        private void Setting_Closing(object sender, FormClosingEventArgs e)
        {
            // ע���жϹر��¼�reason��Դ�ڴ��尴ť�������ò˵��˳�ʱ�޷��˳�!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //ȡ��"�رմ���"�¼�
                e.Cancel = true; // ȡ���رմ��� 

                //ʹ�ر�ʱ���������½���С��Ч��
                this.WindowState = FormWindowState.Minimized;
                this.mainNotifyIcon.Visible = true;
                //this.m_cartoonForm.CartoonClose();
                this.Hide();
                return;
            }
        }
        //�Ҽ���ʾ���̲˵�
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.mainNotifyIcon.ContextMenuStrip = this.contextmenu;
                contextmenu.Show();
            }
        }

        private void settings_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            var TempFilePath = Path.Combine(Path.GetTempPath(), "empty.cur");
            File.Delete(TempFilePath);
            System.Windows.Forms.Application.Exit();
        }
        #endregion

        private void SettingForm_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => { this.Hide(); }));
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPage about = new AboutPage();
            about.Show();
        }
    }
}
