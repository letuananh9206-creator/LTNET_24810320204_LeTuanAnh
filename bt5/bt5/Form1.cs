namespace bt5
{
    public partial class Form1 : Form
    {
        private const int MaximumHistoryRows = 500;
        private CancellationTokenSource? _monitoringCancellation;
        private Task? _monitoringTask;
        private decimal _currentPrice = 67250.00m;
        private int _successfulSyncCount;
        private bool _allowClose;

        public Form1()
        {
            InitializeComponent();
            lblBTCPrice.Text = $"${_currentPrice:N2}";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_monitoringTask is { IsCompleted: false })
            {
                return;
            }

            _monitoringCancellation?.Dispose();
            _monitoringCancellation = new CancellationTokenSource();
            var cancellationToken = _monitoringCancellation.Token;

            btnStart.Enabled = false;
            btnPause.Enabled = true;
            _monitoringTask = Task.Run(() => MonitorPricesAsync(cancellationToken));
        }

        private async void btnPause_Click(object sender, EventArgs e)
        {
            await StopMonitoringAsync();
        }

        private async Task StopMonitoringAsync()
        {
            var cancellation = _monitoringCancellation;
            if (cancellation is null)
            {
                return;
            }

            btnPause.Enabled = false;
            cancellation.Cancel();

            if (_monitoringTask is not null)
            {
                await _monitoringTask;
            }

            cancellation.Dispose();
            if (ReferenceEquals(_monitoringCancellation, cancellation))
            {
                _monitoringCancellation = null;
                _monitoringTask = null;
                btnStart.Enabled = true;
                btnPause.Enabled = false;
            }
        }

        private async Task MonitorPricesAsync(CancellationToken cancellationToken)
        {
            var price = _currentPrice;
            var random = Random.Shared;

            try
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var previousPrice = price;
                    var changeRate = (decimal)(random.NextDouble() * 0.008 - 0.004);
                    price = Math.Round(price * (1 + changeRate), 2);
                    UpdateTicker(price, previousPrice);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private void UpdateTicker(decimal price, decimal previousPrice)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateTicker(price, previousPrice)));
                return;
            }

            _currentPrice = price;
            var updatedAt = DateTime.Now;
            var isRising = price >= previousPrice;

            lblBTCPrice.Text = $"${price:N2}";
            lblBTCPrice.ForeColor = isRising ? Color.ForestGreen : Color.Firebrick;
            dgvHistoryLog.Rows.Insert(0, updatedAt.ToString("HH:mm:ss"), $"${price:N2}", isRising ? "Tăng" : "Giảm");
            if (dgvHistoryLog.Rows.Count > MaximumHistoryRows)
            {
                dgvHistoryLog.Rows.RemoveAt(dgvHistoryLog.Rows.Count - 1);
            }

            _successfulSyncCount++;
            lblLastUpdate.Text = $"Cập nhật cuối: {updatedAt:HH:mm:ss}";
            lblSyncCount.Text = $"Số lượt đồng bộ: {_successfulSyncCount}";
        }

        private async void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_allowClose || _monitoringTask is null)
            {
                return;
            }

            e.Cancel = true;
            await StopMonitoringAsync();
            _allowClose = true;
            Close();
        }
    }
}
