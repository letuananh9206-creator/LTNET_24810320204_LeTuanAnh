namespace bt3
{
    public sealed class frmPatientProfiles : Form
    {
        private readonly DataGridView patientsGrid = new();
        private readonly TextBox patientIdInput = new();
        private readonly TextBox patientNameInput = new();
        private readonly TextBox birthDateInput = new();
        private readonly TextBox departmentInput = new();

        public frmPatientProfiles()
        {
            Text = "Hồ sơ Bệnh nhân";
            Width = 900;
            Height = 550;
            StartPosition = FormStartPosition.CenterParent;

            var inputPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(8),
                WrapContents = false
            };
            patientIdInput.PlaceholderText = "Mã bệnh nhân";
            patientNameInput.PlaceholderText = "Họ và tên";
            birthDateInput.PlaceholderText = "Ngày sinh";
            departmentInput.PlaceholderText = "Khoa/Phòng";
            patientIdInput.Width = 130;
            patientNameInput.Width = 220;
            birthDateInput.Width = 130;
            departmentInput.Width = 150;
            var addButton = new Button { Text = "Thêm bệnh nhân", AutoSize = true };
            addButton.Click += AddPatient;
            inputPanel.Controls.AddRange(new Control[]
            {
                patientIdInput, patientNameInput, birthDateInput, departmentInput, addButton
            });

            patientsGrid.Dock = DockStyle.Fill;
            patientsGrid.AllowUserToAddRows = false;
            patientsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            patientsGrid.Columns.Add("PatientId", "Mã bệnh nhân");
            patientsGrid.Columns.Add("PatientName", "Họ và tên");
            patientsGrid.Columns.Add("BirthDate", "Ngày sinh");
            patientsGrid.Columns.Add("Department", "Khoa/Phòng");

            Controls.Add(patientsGrid);
            Controls.Add(inputPanel);
        }

        private void AddPatient(object? sender, EventArgs e)
        {
            var patientId = patientIdInput.Text.Trim();
            var patientName = patientNameInput.Text.Trim();
            if (patientId.Length == 0 || patientName.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập mã bệnh nhân và họ tên.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            patientsGrid.Rows.Add(
                patientId,
                patientName,
                birthDateInput.Text.Trim(),
                departmentInput.Text.Trim());
            patientIdInput.Clear();
            patientNameInput.Clear();
            birthDateInput.Clear();
            departmentInput.Clear();
        }
    }

    public sealed class frmDoctorSchedule : Form
    {
        public frmDoctorSchedule()
        {
            Text = "Lịch trực Bác sĩ";
            Width = 850;
            Height = 480;
            StartPosition = FormStartPosition.CenterParent;

            var schedule = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            schedule.Columns.Add("Day", "Ngày trực");
            schedule.Columns.Add("Shift", "Ca trực");
            schedule.Columns.Add("Doctor", "Bác sĩ");
            schedule.Columns.Add("Department", "Khoa/Phòng");
            schedule.Rows.Add("Thứ Hai", "Sáng", "Nguyễn Minh Anh", "Nội tổng hợp");
            schedule.Rows.Add("Thứ Ba", "Chiều", "Trần Hoàng Nam", "Ngoại khoa");
            schedule.Rows.Add("Thứ Tư", "Đêm", "Lê Thu Hà", "Cấp cứu");
            Controls.Add(schedule);
        }
    }

    public sealed class frmDoctorCatalog : frmCatalog
    {
        public frmDoctorCatalog() : base("Danh mục Bác sĩ", new[]
        {
            "BS001 - Nguyễn Minh Anh - Nội tổng hợp",
            "BS002 - Trần Hoàng Nam - Ngoại khoa",
            "BS003 - Lê Thu Hà - Cấp cứu"
        })
        {
        }
    }

    public sealed class frmDepartmentCatalog : frmCatalog
    {
        public frmDepartmentCatalog() : base("Danh mục Khoa/Phòng", new[]
        {
            "Khoa Nội tổng hợp",
            "Khoa Ngoại khoa",
            "Khoa Cấp cứu",
            "Phòng Khám tổng quát"
        })
        {
        }
    }

    public class frmCatalog : Form
    {
        protected frmCatalog(string title, IEnumerable<string> entries)
        {
            Text = title;
            Width = 650;
            Height = 400;
            StartPosition = FormStartPosition.CenterParent;
            var list = new ListBox { Dock = DockStyle.Fill };
            list.Items.AddRange(entries.Cast<object>().ToArray());
            Controls.Add(list);
        }
    }
}
