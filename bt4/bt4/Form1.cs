using System.Globalization;

namespace bt4
{
    public partial class frmRevenueAnalytics : Form
    {
        private readonly DateTimePicker dtpFromDate = new();
        private readonly DateTimePicker dtpToDate = new();
        private readonly ComboBox cboBranch = new();
        private readonly Button btnAnalyze = new();
        private readonly Button btnCancel = new();
        private readonly ProgressBar pgbLoading = new();
        private readonly Label lblProcessingStatus = new();
        private readonly DataGridView dgvRevenueData = new();
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isClosing;

        public frmRevenueAnalytics()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Bảng điều khiển thống kê doanh thu";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(980, 620);
            Size = new Size(1200, 760);
            Font = new Font("Segoe UI", 9F);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.FromArgb(246, 248, 251)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            var heading = new Label
            {
                Text = "THỐNG KÊ DOANH THU",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(heading, 0, 0);

            var criteria = new GroupBox
            {
                Text = "Tiêu chí thống kê",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };
            layout.Controls.Add(criteria, 0, 1);

            var criteriaLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 2
            };
            float[] columnWidths = [7, 16, 7, 16, 9, 17, 16, 12];
            for (var i = 0; i < columnWidths.Length; i++)
                criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, columnWidths[i]));
            criteriaLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            criteriaLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            criteria.Controls.Add(criteriaLayout);

            AddCriteriaLabel(criteriaLayout, "Từ ngày:", 0);
            AddCriteriaLabel(criteriaLayout, "Đến ngày:", 2);
            AddCriteriaLabel(criteriaLayout, "Chi nhánh:", 4);

            ConfigureDatePicker(dtpFromDate, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            ConfigureDatePicker(dtpToDate, DateTime.Today);
            criteriaLayout.Controls.Add(dtpFromDate, 1, 1);
            criteriaLayout.Controls.Add(dtpToDate, 3, 1);

            cboBranch.Dock = DockStyle.Fill;
            cboBranch.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBranch.Items.AddRange(["Tất cả chi nhánh", "Chi nhánh Hà Nội", "Chi nhánh Đà Nẵng", "Chi nhánh TP. Hồ Chí Minh"]);
            cboBranch.SelectedIndex = 0;
            criteriaLayout.Controls.Add(cboBranch, 5, 1);

            btnAnalyze.Text = "Thống Kê Doanh Thu";
            btnAnalyze.Dock = DockStyle.Fill;
            btnAnalyze.Margin = new Padding(6, 3, 6, 3);
            btnAnalyze.BackColor = Color.FromArgb(31, 120, 180);
            btnAnalyze.ForeColor = Color.White;
            btnAnalyze.FlatStyle = FlatStyle.Flat;
            btnAnalyze.Click += AnalyzeButton_Click;
            criteriaLayout.Controls.Add(btnAnalyze, 6, 1);

            btnCancel.Text = "Hủy Thao Tác";
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Margin = new Padding(6, 3, 0, 3);
            btnCancel.BackColor = Color.FromArgb(190, 55, 55);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Enabled = false;
            btnCancel.Click += CancelButton_Click;
            criteriaLayout.Controls.Add(btnCancel, 7, 1);

            var status = new GroupBox
            {
                Text = "Trạng thái xử lý",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };
            layout.Controls.Add(status, 0, 2);

            var statusLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            statusLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            statusLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            status.Controls.Add(statusLayout);

            pgbLoading.Dock = DockStyle.Fill;
            pgbLoading.Minimum = 0;
            pgbLoading.Maximum = 100;
            pgbLoading.Margin = new Padding(0, 0, 0, 5);
            statusLayout.Controls.Add(pgbLoading, 0, 0);

            lblProcessingStatus.Text = "Sẵn sàng thống kê doanh thu.";
            lblProcessingStatus.Dock = DockStyle.Fill;
            lblProcessingStatus.TextAlign = ContentAlignment.MiddleLeft;
            statusLayout.Controls.Add(lblProcessingStatus, 0, 1);

            var results = new GroupBox
            {
                Text = "Chi tiết giao dịch",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            layout.Controls.Add(results, 0, 3);

            dgvRevenueData.Dock = DockStyle.Fill;
            dgvRevenueData.ReadOnly = true;
            dgvRevenueData.AllowUserToAddRows = false;
            dgvRevenueData.AllowUserToDeleteRows = false;
            dgvRevenueData.AllowUserToResizeRows = false;
            dgvRevenueData.AutoGenerateColumns = false;
            dgvRevenueData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRevenueData.RowHeadersVisible = false;
            dgvRevenueData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRevenueData.BackgroundColor = Color.White;
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.TransactionCode),
                HeaderText = "Mã giao dịch",
                FillWeight = 95
            });
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.TransactionDate),
                HeaderText = "Ngày giao dịch",
                FillWeight = 105,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.Branch),
                HeaderText = "Chi nhánh",
                FillWeight = 145
            });
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.Product),
                HeaderText = "Sản phẩm",
                FillWeight = 130
            });
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.Quantity),
                HeaderText = "Số lượng",
                FillWeight = 70
            });
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.UnitPrice),
                HeaderText = "Đơn giá",
                FillWeight = 105,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", FormatProvider = CultureInfo.GetCultureInfo("vi-VN") }
            });
            dgvRevenueData.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(RevenueTransaction.Total),
                HeaderText = "Thành tiền",
                FillWeight = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", FormatProvider = CultureInfo.GetCultureInfo("vi-VN") }
            });
            results.Controls.Add(dgvRevenueData);

            FormClosing += (_, _) =>
            {
                _isClosing = true;
                _cancellationTokenSource?.Cancel();
            };
        }

        private static void AddCriteriaLabel(TableLayoutPanel layout, string text, int column)
        {
            layout.Controls.Add(new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            }, column, 0);
        }

        private static void ConfigureDatePicker(DateTimePicker picker, DateTime value)
        {
            picker.Dock = DockStyle.Fill;
            picker.Format = DateTimePickerFormat.Short;
            picker.Value = value;
        }

        private async void AnalyzeButton_Click(object? sender, EventArgs e)
        {
            var fromDate = dtpFromDate.Value.Date;
            var toDate = dtpToDate.Value.Date;
            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu không được sau ngày kết thúc.", "Ngày không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedBranch = cboBranch.SelectedItem?.ToString() ?? "Tất cả chi nhánh";
            using var cancellation = new CancellationTokenSource();
            _cancellationTokenSource = cancellation;
            btnAnalyze.Enabled = false;
            btnCancel.Enabled = true;
            pgbLoading.Value = 0;
            lblProcessingStatus.Text = "Đang truy xuất và tổng hợp dữ liệu...";

            IProgress<int> progress = new Progress<int>(value =>
            {
                if (_isClosing || IsDisposed)
                    return;
                pgbLoading.Value = value;
                lblProcessingStatus.Text = $"Đang xử lý... {value}%";
            });

            try
            {
                var transactions = await Task.Run(async () =>
                {
                    const int totalSteps = 50;
                    for (var step = 1; step <= totalSteps; step++)
                    {
                        await Task.Delay(100, cancellation.Token);
                        progress.Report(step * 100 / totalSteps);
                    }

                    cancellation.Token.ThrowIfCancellationRequested();
                    return CreateTransactions(fromDate, toDate, selectedBranch, cancellation.Token);
                }, cancellation.Token);

                if (_isClosing || IsDisposed)
                    return;

                dgvRevenueData.DataSource = transactions;
                lblProcessingStatus.Text = $"Hoàn tất. Đã thống kê {transactions.Count} giao dịch.";
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                if (!_isClosing && !IsDisposed)
                {
                    pgbLoading.Value = 0;
                    lblProcessingStatus.Text = "Đã hủy thao tác!";
                }
            }
            catch (Exception exception)
            {
                if (!_isClosing && !IsDisposed)
                {
                    lblProcessingStatus.Text = "Không thể hoàn tất thống kê.";
                    MessageBox.Show($"Đã xảy ra lỗi khi thống kê doanh thu:\n{exception.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (ReferenceEquals(_cancellationTokenSource, cancellation))
                    _cancellationTokenSource = null;
                if (!_isClosing && !IsDisposed)
                {
                    btnAnalyze.Enabled = true;
                    btnCancel.Enabled = false;
                }
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            btnCancel.Enabled = false;
            lblProcessingStatus.Text = "Đang hủy thao tác...";
        }

        private static List<RevenueTransaction> CreateTransactions(
            DateTime fromDate, DateTime toDate, string selectedBranch, CancellationToken cancellationToken)
        {
            string[] branches = ["Chi nhánh Hà Nội", "Chi nhánh Đà Nẵng", "Chi nhánh TP. Hồ Chí Minh"];
            string[] products = ["Laptop", "Điện thoại", "Máy tính bảng", "Phụ kiện"];
            var random = new Random();
            var dayRange = Math.Max(1, (toDate - fromDate).Days + 1);
            var transactions = new List<RevenueTransaction>(60);

            for (var i = 1; i <= 60; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var branch = selectedBranch == "Tất cả chi nhánh"
                    ? branches[random.Next(branches.Length)]
                    : selectedBranch;
                var product = products[random.Next(products.Length)];
                var quantity = random.Next(1, 6);
                var unitPrice = random.Next(100, 3001) * 1000m;
                var transactionDate = fromDate.AddDays(random.Next(dayRange))
                    .AddHours(random.Next(24)).AddMinutes(random.Next(60));
                transactions.Add(new RevenueTransaction(
                    $"GD{i:0000}", transactionDate, branch, product, quantity, unitPrice, quantity * unitPrice));
            }

            return transactions.OrderByDescending(transaction => transaction.TransactionDate).ToList();
        }

        private sealed record RevenueTransaction(
            string TransactionCode,
            DateTime TransactionDate,
            string Branch,
            string Product,
            int Quantity,
            decimal UnitPrice,
            decimal Total);
    }
}
