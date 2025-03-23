namespace Don_t_show_my_mouse_to_teacher
{
    partial class SettingForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            enablehide = new CheckBox();
            longpress2display = new CheckBox();
            mainNotifyIcon = new NotifyIcon(components);
            contextmenu = new ContextMenuStrip(components);
            settings = new ToolStripMenuItem();
            exit = new ToolStripMenuItem();
            AboutToolStripMenuItem = new ToolStripMenuItem();
            contextmenu.SuspendLayout();
            SuspendLayout();
            // 
            // enablehide
            // 
            enablehide.AutoSize = true;
            enablehide.Checked = true;
            enablehide.CheckState = CheckState.Checked;
            enablehide.Location = new Point(12, 12);
            enablehide.Name = "enablehide";
            enablehide.Size = new Size(91, 24);
            enablehide.TabIndex = 0;
            enablehide.Text = "启用隐藏";
            enablehide.UseVisualStyleBackColor = true;
            // 
            // longpress2display
            // 
            longpress2display.AutoSize = true;
            longpress2display.Location = new Point(127, 12);
            longpress2display.Name = "longpress2display";
            longpress2display.Size = new Size(196, 24);
            longpress2display.TabIndex = 1;
            longpress2display.Text = "长按右键时显示鼠标指针";
            longpress2display.UseVisualStyleBackColor = true;
            // 
            // mainNotifyIcon
            // 
            mainNotifyIcon.Icon = (Icon)resources.GetObject("mainNotifyIcon.Icon");
            mainNotifyIcon.Text = "don't show my mouse with teacher";
            mainNotifyIcon.Visible = true;
            mainNotifyIcon.MouseClick += notifyIcon_MouseClick;
            // 
            // contextmenu
            // 
            contextmenu.ImageScalingSize = new Size(20, 20);
            contextmenu.Items.AddRange(new ToolStripItem[] { settings, exit, AboutToolStripMenuItem });
            contextmenu.Name = "contextmenu";
            contextmenu.Size = new Size(139, 76);
            contextmenu.Text = "don't show my mouse with teacher";
            // 
            // settings
            // 
            settings.Name = "settings";
            settings.Size = new Size(138, 24);
            settings.Text = "设置";
            settings.Click += settings_Click;
            // 
            // exit
            // 
            exit.Name = "exit";
            exit.Size = new Size(138, 24);
            exit.Text = "退出程序";
            exit.Click += exit_Click;
            // 
            // AboutToolStripMenuItem
            // 
            AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            AboutToolStripMenuItem.Size = new Size(138, 24);
            AboutToolStripMenuItem.Text = "关于";
            AboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(327, 42);
            Controls.Add(longpress2display);
            Controls.Add(enablehide);
            Name = "SettingForm";
            Text = "设置";
            FormClosing += Setting_Closing;
            Load += SettingForm_Load;
            contextmenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox enablehide;
        private CheckBox longpress2display;
        private NotifyIcon mainNotifyIcon;
        private ContextMenuStrip contextmenu;
        private ToolStripMenuItem settings;
        private ToolStripMenuItem exit;
        private ToolStripMenuItem AboutToolStripMenuItem;
    }
}
