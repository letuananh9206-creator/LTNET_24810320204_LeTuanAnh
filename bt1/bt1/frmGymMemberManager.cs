using System.ComponentModel;
using System.Globalization;
using System.Net.Mail;

namespace bt1
{
    public class frmGymMemberManager : Form
    {
        private static readonly string[] Packages = ["Theo tháng", "6 tháng", "1 năm"];

        private readonly BindingList<MemberModel> _members = [];
        private readonly BindingSource _membersSource = new();
        private readonly BindingSource _editorSource = new();
        private readonly ErrorProvider _errorProvider;
        private readonly DataGridView _dgvMembers = new();
        private readonly TextBox _txtMemberId = new();
        private readonly TextBox _txtFullName = new();
        private readonly TextBox _txtPhone = new();
        private readonly TextBox _txtEmail = new();
        private readonly ComboBox _cboPackage = new();
        private readonly DateTimePicker _dtpStartDate = new();
        private readonly DateTimePicker _dtpExpirationDate = new();
        private readonly TextBox _txtFee = new();
        private MemberModel? _selectedMember;
        private int _nextMemberNumber = 1;
        private bool _changingSelection;

        public frmGymMemberManager()
        {
            Text = "Quản lý dịch vụ phòng gym & hội viên";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(980, 700);
            Size = new Size(1120, 790);
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.FromArgb(245, 247, 250);

            _errorProvider = new ErrorProvider
            {
                ContainerControl = this,
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };
            Disposed += (_, _) => _errorProvider.Dispose();

            BuildInterface();
            ConfigureGrid();
            ConfigureBindings();
            ConfigureEvents();

            _membersSource.DataSource = _members;
            _dgvMembers.DataSource = _membersSource;
            SetEditorModel(CreateEmptyMember());
        }

        private void BuildInterface()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18),
                BackColor = BackColor
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 290));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var heading = new Label
            {
                Text = "QUẢN LÝ HỘI VIÊN GYM",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 50, 79),
                TextAlign = ContentAlignment.MiddleLeft
            };
            root.Controls.Add(heading, 0, 0);

            var editorGroup = new GroupBox
            {
                Text = "Thông tin hội viên",
                Dock = DockStyle.Fill,
                Padding = new Padding(14),
                ForeColor = Color.FromArgb(43, 55, 72)
            };
            root.Controls.Add(editorGroup, 0, 1);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 4,
                Padding = new Padding(5, 8, 5, 4)
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 122));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (var i = 0; i < 4; i++)
            {
                content.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            }
            editorGroup.Controls.Add(content);

            _txtMemberId.Name = "txtMemberId";
            _txtMemberId.ReadOnly = true;
            _txtMemberId.BackColor = Color.FromArgb(235, 239, 244);
            _txtFullName.Name = "txtFullName";
            _txtPhone.Name = "txtPhone";
            _txtEmail.Name = "txtEmail";
            AddField(content, "Mã hội viên", _txtMemberId, 0, 0);
            AddField(content, "Họ và tên", _txtFullName, 2, 0);
            AddField(content, "Số điện thoại", _txtPhone, 0, 1);
            AddField(content, "Email", _txtEmail, 2, 1);

            _cboPackage.Name = "cboPackage";
            _cboPackage.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboPackage.Items.AddRange(Packages);
            AddField(content, "Gói tập", _cboPackage, 0, 2);

            _dtpStartDate.Name = "dtpStartDate";
            _dtpStartDate.Format = DateTimePickerFormat.Custom;
            _dtpStartDate.CustomFormat = "dd/MM/yyyy";
            AddField(content, "Ngày đăng ký", _dtpStartDate, 2, 2);
            _dtpExpirationDate.Name = "dtpExpirationDate";
            _dtpExpirationDate.Format = DateTimePickerFormat.Custom;
            _dtpExpirationDate.CustomFormat = "dd/MM/yyyy";
            AddField(content, "Ngày hết hạn", _dtpExpirationDate, 0, 3);
            _txtFee.Name = "txtFee";
            AddField(content, "Phí dịch vụ", _txtFee, 2, 3);

            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 7, 0, 0)
            };
            editorGroup.Controls.Add(actionPanel);

            actionPanel.Controls.Add(CreateButton("Thêm mới", Color.FromArgb(34, 139, 94), AddMember));
            actionPanel.Controls.Add(CreateButton("Sửa thông tin", Color.FromArgb(45, 108, 180), EditMember));
            actionPanel.Controls.Add(CreateButton("Xóa hội viên", Color.FromArgb(190, 64, 64), DeleteMember));
            actionPanel.Controls.Add(CreateButton("Làm mới", Color.FromArgb(100, 112, 128), ResetEditor));

            _dgvMembers.Name = "dgvMembers";
            var gridGroup = new GroupBox
            {
                Text = "Danh sách hội viên",
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ForeColor = Color.FromArgb(43, 55, 72)
            };
            root.Controls.Add(gridGroup, 0, 2);

            _dgvMembers.Dock = DockStyle.Fill;
            _dgvMembers.BackgroundColor = Color.White;
            _dgvMembers.BorderStyle = BorderStyle.FixedSingle;
            _dgvMembers.RowHeadersVisible = false;
            _dgvMembers.AllowUserToAddRows = false;
            _dgvMembers.AllowUserToDeleteRows = false;
            _dgvMembers.AllowUserToResizeRows = false;
            _dgvMembers.MultiSelect = false;
            _dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvMembers.ReadOnly = true;
            _dgvMembers.AutoGenerateColumns = false;
            _dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvMembers.RowTemplate.Height = 32;
            _dgvMembers.EnableHeadersVisualStyles = false;
            _dgvMembers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(34, 52, 75);
            _dgvMembers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _dgvMembers.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
            _dgvMembers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 226, 247);
            _dgvMembers.DefaultCellStyle.SelectionForeColor = Color.FromArgb(25, 43, 63);
            gridGroup.Controls.Add(_dgvMembers);
        }

        private static void AddField(TableLayoutPanel panel, string labelText, Control input, int column, int row)
        {
            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(55, 66, 81)
            };
            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(3, 7, 14, 7);
            panel.Controls.Add(label, column, row);
            panel.Controls.Add(input, column + 1, row);
        }

        private static Button CreateButton(string text, Color color, EventHandler onClick)
        {
            var button = new Button
            {
                Text = text,
                Width = 142,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += onClick;
            return button;
        }

        private void ConfigureGrid()
        {
            _dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMemberId",
                HeaderText = "Mã HV",
                DataPropertyName = nameof(MemberModel.MemberId),
                FillWeight = 75
            });
            _dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFullName",
                HeaderText = "Tên hội viên",
                DataPropertyName = nameof(MemberModel.FullName),
                FillWeight = 145
            });
            _dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhone",
                HeaderText = "SĐT",
                DataPropertyName = nameof(MemberModel.Phone),
                FillWeight = 95
            });
            _dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPackage",
                HeaderText = "Gói tập",
                DataPropertyName = nameof(MemberModel.Package),
                FillWeight = 90
            });
            _dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStartDate",
                HeaderText = "Ngày ĐK",
                DataPropertyName = nameof(MemberModel.StartDate),
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" },
                FillWeight = 90
            });
            _dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFee",
                HeaderText = "Phí dịch vụ",
                DataPropertyName = nameof(MemberModel.Fee),
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight },
                FillWeight = 100
            });
        }

        private void ConfigureBindings()
        {
            _txtMemberId.DataBindings.Add(nameof(TextBox.Text), _editorSource, nameof(MemberModel.MemberId), true, DataSourceUpdateMode.OnPropertyChanged);
            _txtFullName.DataBindings.Add(nameof(TextBox.Text), _editorSource, nameof(MemberModel.FullName), true, DataSourceUpdateMode.OnPropertyChanged);
            _txtPhone.DataBindings.Add(nameof(TextBox.Text), _editorSource, nameof(MemberModel.Phone), true, DataSourceUpdateMode.OnPropertyChanged);
            _txtEmail.DataBindings.Add(nameof(TextBox.Text), _editorSource, nameof(MemberModel.Email), true, DataSourceUpdateMode.OnPropertyChanged);
            _cboPackage.DataBindings.Add(nameof(ComboBox.SelectedItem), _editorSource, nameof(MemberModel.Package), true, DataSourceUpdateMode.OnPropertyChanged);
            _dtpStartDate.DataBindings.Add(nameof(DateTimePicker.Value), _editorSource, nameof(MemberModel.StartDate), true, DataSourceUpdateMode.OnPropertyChanged);
            _dtpExpirationDate.DataBindings.Add(nameof(DateTimePicker.Value), _editorSource, nameof(MemberModel.ExpirationDate), true, DataSourceUpdateMode.OnPropertyChanged);
            _txtFee.DataBindings.Add(nameof(TextBox.Text), _editorSource, nameof(MemberModel.FeeInput), true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void ConfigureEvents()
        {
            _dgvMembers.SelectionChanged += (_, _) => LoadSelectedMember();
        }

        private void LoadSelectedMember()
        {
            if (_changingSelection || _dgvMembers.CurrentRow?.DataBoundItem is not MemberModel member)
            {
                return;
            }

            _selectedMember = member;
            SetEditorModel(member.Copy());
        }

        private void SetEditorModel(MemberModel member)
        {
            _editorSource.DataSource = member;
            _editorSource.ResetBindings(false);
            ClearValidationErrors();
        }

        private MemberModel CreateEmptyMember()
        {
            var startDate = DateTime.Today;
            var package = Packages[0];
            return new MemberModel
            {
                MemberId = GenerateNextMemberId(),
                Package = package,
                StartDate = startDate,
                ExpirationDate = GetExpirationDate(startDate, package),
                FeeInput = string.Empty
            };
        }

        private string GenerateNextMemberId() => $"HV{_nextMemberNumber:D3}";

        private static DateTime GetExpirationDate(DateTime startDate, string package) => package switch
        {
            "6 tháng" => startDate.AddMonths(6),
            "1 năm" => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };

        private bool ValidateEditor(out long fee)
        {
            fee = 0;
            var isValid = true;
            var normalizedName = _txtFullName.Text.Trim().Normalize();
            var nameIsValid = normalizedName.Length > 0 && normalizedName.All(character => char.IsLetter(character) || character == ' ');
            SetError(_txtFullName, nameIsValid ? string.Empty : "Họ tên không được trống hoặc chứa ký tự đặc biệt.", ref isValid);

            var phone = _txtPhone.Text.Trim();
            var phoneIsValid = phone.Length == 10 && phone.All(character => character is >= '0' and <= '9');
            SetError(_txtPhone, phoneIsValid ? string.Empty : "Số điện thoại phải gồm đúng 10 chữ số.", ref isValid);

            var email = _txtEmail.Text.Trim();
            var emailIsValid = email.Length == 0 || IsValidEmail(email);
            SetError(_txtEmail, emailIsValid ? string.Empty : "Email không đúng định dạng.", ref isValid);

            var feeText = _txtFee.Text.Trim();
            var feeIsValid = long.TryParse(feeText, NumberStyles.None, CultureInfo.InvariantCulture, out fee) && fee > 0;
            SetError(_txtFee, feeIsValid ? string.Empty : "Phí dịch vụ phải là số nguyên dương.", ref isValid);

            var expirationIsValid = _dtpExpirationDate.Value.Date > _dtpStartDate.Value.Date;
            SetError(_dtpExpirationDate, expirationIsValid ? string.Empty : "Ngày hết hạn phải sau ngày đăng ký.", ref isValid);

            var packageIsValid = _cboPackage.SelectedItem is string package && Packages.Contains(package);
            SetError(_cboPackage, packageIsValid ? string.Empty : "Vui lòng chọn gói tập hợp lệ.", ref isValid);
            return isValid;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void SetError(Control control, string message, ref bool isValid)
        {
            _errorProvider.SetError(control, message);
            if (message.Length > 0)
            {
                isValid = false;
            }
        }

        private void ClearValidationErrors()
        {
            foreach (Control control in new Control[] { _txtFullName, _txtPhone, _txtEmail, _txtFee, _dtpExpirationDate, _cboPackage })
            {
                _errorProvider.SetError(control, string.Empty);
            }
        }

        private void AddMember(object? sender, EventArgs e)
        {
            if (!ValidateEditor(out var fee))
            {
                return;
            }

            var newMember = new MemberModel
            {
                MemberId = GenerateNextMemberId(),
                FullName = _txtFullName.Text.Trim().Normalize(),
                Phone = _txtPhone.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Package = _cboPackage.SelectedItem!.ToString()!,
                StartDate = _dtpStartDate.Value.Date,
                ExpirationDate = _dtpExpirationDate.Value.Date,
                Fee = fee,
                FeeInput = fee.ToString(CultureInfo.InvariantCulture)
            };

            _members.Add(newMember);
            _nextMemberNumber++;
            SelectMember(newMember);
        }

        private void EditMember(object? sender, EventArgs e)
        {
            if (_selectedMember is null)
            {
                MessageBox.Show(this, "Vui lòng chọn hội viên cần sửa.", "Chưa chọn hội viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateEditor(out var fee))
            {
                return;
            }

            _selectedMember.FullName = _txtFullName.Text.Trim().Normalize();
            _selectedMember.Phone = _txtPhone.Text.Trim();
            _selectedMember.Email = _txtEmail.Text.Trim();
            _selectedMember.Package = _cboPackage.SelectedItem!.ToString()!;
            _selectedMember.StartDate = _dtpStartDate.Value.Date;
            _selectedMember.ExpirationDate = _dtpExpirationDate.Value.Date;
            _selectedMember.Fee = fee;
            SetEditorModel(_selectedMember.Copy());
        }

        private void DeleteMember(object? sender, EventArgs e)
        {
            if (_selectedMember is null)
            {
                MessageBox.Show(this, "Vui lòng chọn hội viên cần xóa.", "Chưa chọn hội viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(this,
                $"Bạn có chắc chắn muốn xóa hội viên {_selectedMember.MemberId} - {_selectedMember.FullName}?",
                "Xác nhận xóa hội viên",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }

            var removedMember = _selectedMember;
            _changingSelection = true;
            _members.Remove(removedMember);
            _selectedMember = null;
            _dgvMembers.ClearSelection();
            _dgvMembers.CurrentCell = null;
            _changingSelection = false;
            SetEditorModel(CreateEmptyMember());
        }

        private void ResetEditor(object? sender, EventArgs e)
        {
            _changingSelection = true;
            _selectedMember = null;
            _dgvMembers.ClearSelection();
            _dgvMembers.CurrentCell = null;
            _changingSelection = false;
            SetEditorModel(CreateEmptyMember());
        }

        private void SelectMember(MemberModel member)
        {
            var index = _members.IndexOf(member);
            if (index < 0)
            {
                return;
            }

            _changingSelection = true;
            _membersSource.Position = index;
            _dgvMembers.ClearSelection();
            _dgvMembers.Rows[index].Selected = true;
            _dgvMembers.CurrentCell = _dgvMembers.Rows[index].Cells[0];
            _selectedMember = member;
            _changingSelection = false;
            SetEditorModel(member.Copy());
        }
    }
}
