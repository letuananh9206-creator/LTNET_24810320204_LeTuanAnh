namespace bt5
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Button btnStart;
        private Button btnPause;
        private Label lblBTCPrice;
        private Label lblPriceCaption;
        private DataGridView dgvHistoryLog;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblLastUpdate;
        private ToolStripStatusLabel lblSyncCount;

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
            btnStart = new Button();
            btnPause = new Button();
            lblBTCPrice = new Label();
            lblPriceCaption = new Label();
            dgvHistoryLog = new DataGridView();
            statusStrip = new StatusStrip();
            lblLastUpdate = new ToolStripStatusLabel();
            lblSyncCount = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)dgvHistoryLog).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(24, 24);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(180, 40);
            btnStart.TabIndex = 0;
            btnStart.Text = "Bắt đầu Giám sát";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnPause
            // 
            btnPause.Enabled = false;
            btnPause.Location = new Point(216, 24);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(140, 40);
            btnPause.TabIndex = 1;
            btnPause.Text = "Tạm dừng";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // lblPriceCaption
            // 
            lblPriceCaption.AutoSize = true;
            lblPriceCaption.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblPriceCaption.Location = new Point(24, 91);
            lblPriceCaption.Name = "lblPriceCaption";
            lblPriceCaption.Size = new Size(90, 28);
            lblPriceCaption.TabIndex = 2;
            lblPriceCaption.Text = "BTC/USD";
            // 
            // lblBTCPrice
            // 
            lblBTCPrice.AutoSize = true;
            lblBTCPrice.Font = new Font("Segoe UI", 30F, FontStyle.Bold, GraphicsUnit.Point);
            lblBTCPrice.ForeColor = Color.DimGray;
            lblBTCPrice.Location = new Point(20, 119);
            lblBTCPrice.Name = "lblBTCPrice";
            lblBTCPrice.Size = new Size(246, 67);
            lblBTCPrice.TabIndex = 3;
            lblBTCPrice.Text = "$67,250.00";
            // 
            // dgvHistoryLog
            // 
            dgvHistoryLog.AllowUserToAddRows = false;
            dgvHistoryLog.AllowUserToDeleteRows = false;
            dgvHistoryLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistoryLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistoryLog.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Thời gian" },
                new DataGridViewTextBoxColumn { Name = "colPrice", HeaderText = "Giá BTC/USD" },
                new DataGridViewTextBoxColumn { Name = "colTrend", HeaderText = "Biến động" }
            });
            dgvHistoryLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHistoryLog.Location = new Point(24, 207);
            dgvHistoryLog.MultiSelect = false;
            dgvHistoryLog.Name = "dgvHistoryLog";
            dgvHistoryLog.ReadOnly = true;
            dgvHistoryLog.RowHeadersVisible = false;
            dgvHistoryLog.RowHeadersWidth = 51;
            dgvHistoryLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistoryLog.Size = new Size(852, 323);
            dgvHistoryLog.TabIndex = 4;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { lblLastUpdate, lblSyncCount });
            statusStrip.Location = new Point(0, 555);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(900, 26);
            statusStrip.TabIndex = 5;
            // 
            // lblLastUpdate
            // 
            lblLastUpdate.Name = "lblLastUpdate";
            lblLastUpdate.Size = new Size(150, 20);
            lblLastUpdate.Text = "Cập nhật cuối: --:--:--";
            // 
            // lblSyncCount
            // 
            lblSyncCount.Name = "lblSyncCount";
            lblSyncCount.Size = new Size(150, 20);
            lblSyncCount.Text = "Số lượt đồng bộ: 0";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 581);
            Controls.Add(dgvHistoryLog);
            Controls.Add(lblBTCPrice);
            Controls.Add(lblPriceCaption);
            Controls.Add(btnPause);
            Controls.Add(btnStart);
            Controls.Add(statusStrip);
            MinimumSize = new Size(700, 500);
            Name = "frmCryptoStockTicker";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Giám sát tỷ giá BTC/USD";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)dgvHistoryLog).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
