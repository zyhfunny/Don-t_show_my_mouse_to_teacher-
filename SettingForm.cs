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
        #region ״̬����
        private bool is_hide;
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
        private const uint OCR_IBEAM = 32513;
        private const uint OCR_WAIT = 32514;
        private const uint OCR_CROSS = 32515;
        private const uint OCR_UP = 32516;
        private const uint OCR_SIZENWSE = 32642;
        private const uint OCR_SIZENESW = 32643;
        private const uint OCR_SIZEWE = 32644;
        private const uint OCR_SIZENS = 32645;
        private const uint OCR_SIZEALL = 32646;
        private const uint OCR_NO = 32648;
        private const uint OCR_HAND = 32649;
        private const uint OCR_APPSTARTING = 32650;
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
                    if (longpress2display.Checked && e.Button == MouseButtons.Right && e.EventType == MouseEventType.Down&&is_hide)
                    {
                        SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
                        is_hide = false;
                    }
                    if (longpress2display.Checked && e.Button == MouseButtons.Right && e.EventType == MouseEventType.Up&&is_hide==false)
                    {
                        var cursorFile = Path.Combine(Path.GetTempPath(), "empty.cur");
                        _transparentCursor = LoadCursorFromFile(cursorFile);
                        HideCursor_useAPI();
                        is_hide = true;
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
                HideCursor_useAPI();
            }
        }
        private void HideCursor_useAPI()
        {
            SetSystemCursor(_transparentCursor, OCR_NORMAL);
            SetSystemCursor(_transparentCursor, OCR_IBEAM);
            SetSystemCursor(_transparentCursor, OCR_WAIT);
            SetSystemCursor(_transparentCursor, OCR_CROSS);
            SetSystemCursor(_transparentCursor, OCR_UP);
            SetSystemCursor(_transparentCursor, OCR_SIZENWSE);
            SetSystemCursor(_transparentCursor, OCR_SIZENESW);
            SetSystemCursor(_transparentCursor, OCR_SIZEWE);
            SetSystemCursor(_transparentCursor, OCR_SIZENS);
            SetSystemCursor(_transparentCursor, OCR_SIZEALL);
            SetSystemCursor(_transparentCursor, OCR_NO);
            SetSystemCursor(_transparentCursor, OCR_HAND);
            SetSystemCursor(_transparentCursor, OCR_APPSTARTING);
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
