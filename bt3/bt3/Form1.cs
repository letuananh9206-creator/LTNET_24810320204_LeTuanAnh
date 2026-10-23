namespace bt3
{
    public partial class frmHospitalMain : Form
    {
        private readonly ToolStripStatusLabel doctorStatus = new();
        private readonly ToolStripProgressBar reloadProgress = new();
        private readonly ToolStripStatusLabel dataStatus = new();

        public frmHospitalMain()
        {
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            BuildMenu();
            BuildStatusBar();
        }

        private void BuildMenu()
        {
            var menu = new MenuStrip();

            var systemMenu = new ToolStripMenuItem("Hệ thống");
            systemMenu.DropDownItems.Add("Đăng xuất", null, (_, _) => LogOut());
            systemMenu.DropDownItems.Add("Nạp lại dữ liệu", null, (_, _) => ReloadData());
            systemMenu.DropDownItems.Add("Thoát", null, (_, _) => Close());

            var managementMenu = new ToolStripMenuItem("Quản lý");
            managementMenu.DropDownItems.Add("Hồ sơ Bệnh nhân", null, (_, _) => ShowChild<frmPatientProfiles>());
            managementMenu.DropDownItems.Add("Danh mục Bác sĩ", null, (_, _) => ShowChild<frmDoctorCatalog>());
            managementMenu.DropDownItems.Add("Danh mục Khoa/Phòng", null, (_, _) => ShowChild<frmDepartmentCatalog>());
            managementMenu.DropDownItems.Add("Lịch trực Bác sĩ", null, (_, _) => ShowChild<frmDoctorSchedule>());

            var windowMenu = new ToolStripMenuItem("Cửa sổ");
            windowMenu.DropDownItems.Add("Sắp xếp Xếp tầng", null, (_, _) => LayoutMdi(MdiLayout.Cascade));
            windowMenu.DropDownItems.Add("Ngang", null, (_, _) => LayoutMdi(MdiLayout.TileHorizontal));
            windowMenu.DropDownItems.Add("Dọc", null, (_, _) => LayoutMdi(MdiLayout.TileVertical));
            windowMenu.DropDownItems.Add("Đóng tất cả", null, (_, _) => CloseAllChildren());

            menu.Items.AddRange(new ToolStripItem[] { systemMenu, managementMenu, windowMenu });
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private void BuildStatusBar()
        {
            var status = new StatusStrip();
            doctorStatus.Text = "Bác sĩ trực: Nguyễn Minh Anh";
            dataStatus.Spring = true;
            dataStatus.TextAlign = ContentAlignment.MiddleRight;
            reloadProgress.Minimum = 0;
            reloadProgress.Maximum = 100;
            reloadProgress.Value = 0;
            reloadProgress.Size = new Size(120, 16);
            status.Items.AddRange(new ToolStripItem[] { doctorStatus, dataStatus, reloadProgress });
            Controls.Add(status);
        }

        private void ShowChild<TForm>() where TForm : Form, new()
        {
            foreach (Form child in MdiChildren)
            {
                if (child is TForm)
                {
                    child.Activate();
                    return;
                }
            }

            var form = new TForm { MdiParent = this };
            form.Show();
        }

        private void CloseAllChildren()
        {
            foreach (Form child in MdiChildren)
            {
                child.Close();
            }
        }

        private void ReloadData()
        {
            reloadProgress.Value = reloadProgress.Maximum;
            dataStatus.Text = "Dữ liệu đã được nạp lại";
        }

        private void LogOut()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
