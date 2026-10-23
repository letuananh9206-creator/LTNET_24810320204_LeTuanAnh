using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace bt2;

public sealed class frmBookManager : Form
{
    private readonly BindingList<BookModel> books = [];
    private readonly BindingSource bookSource = new();
    private readonly TextBox titleTextBox = new();
    private readonly TextBox authorTextBox = new();
    private readonly TextBox isbnTextBox = new();
    private readonly TextBox categoryTextBox = new();
    private readonly TextBox priceTextBox = new();
    private readonly TextBox quantityTextBox = new();
    private readonly TextBox searchTextBox = new();
    private readonly PictureBox picBookCover = new();
    private readonly DataGridView dgvBooks = new();
    private readonly ToolStripStatusLabel countStatusLabel = new();
    private readonly ToolStripStatusLabel valueStatusLabel = new();
    private readonly ToolStripStatusLabel timeStatusLabel = new();
    private readonly System.Windows.Forms.Timer clockTimer = new() { Interval = 1000 };
    private string coverPath = string.Empty;
    private BookModel? selectedBook;

    public frmBookManager()
    {
        Text = "Quản lý tài liệu sách & xuất báo cáo nhà sách";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 650);
        Size = new Size(1200, 760);
        BuildInterface();

        bookSource.DataSource = books;
        dgvBooks.DataSource = bookSource;
        books.ListChanged += (_, _) => UpdateStatus();
        clockTimer.Tick += (_, _) => UpdateClock();
        clockTimer.Start();
        UpdateStatus();
        UpdateClock();
        FormClosed += (_, _) =>
        {
            clockTimer.Stop();
            clockTimer.Dispose();
            picBookCover.Image?.Dispose();
        };
    }

    private void BuildInterface()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = Padding.Empty
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var menu = BuildMenu();
        var toolStrip = BuildToolStrip();
        var statusStrip = BuildStatusStrip();
        var content = BuildContent();
        root.Controls.Add(menu, 0, 0);
        root.Controls.Add(toolStrip, 0, 1);
        root.Controls.Add(content, 0, 2);
        root.Controls.Add(statusStrip, 0, 3);
        Controls.Add(root);
        MainMenuStrip = menu;
    }

    private MenuStrip BuildMenu()
    {
        var menu = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("File");
        fileMenu.DropDownItems.Add("Mở file CSV", null, (_, _) => OpenCsv());
        fileMenu.DropDownItems.Add("Lưu file CSV", null, (_, _) => SaveCsv());
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Thoát", null, (_, _) => Close());

        var editMenu = new ToolStripMenuItem("Edit");
        editMenu.DropDownItems.Add("Thêm sách", null, (_, _) => AddBook());
        editMenu.DropDownItems.Add("Xóa sách", null, (_, _) => DeleteSelectedBook());
        editMenu.DropDownItems.Add("Tìm kiếm", null, (_, _) =>
        {
            searchTextBox.Focus();
            searchTextBox.SelectAll();
        });
        menu.Items.Add(fileMenu);
        menu.Items.Add(editMenu);
        return menu;
    }

    private ToolStrip BuildToolStrip()
    {
        var toolStrip = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };
        toolStrip.Items.Add(new ToolStripButton("📂 Mở file", null, (_, _) => OpenCsv()));
        toolStrip.Items.Add(new ToolStripButton("💾 Lưu file", null, (_, _) => SaveCsv()));
        toolStrip.Items.Add(new ToolStripSeparator());
        toolStrip.Items.Add(new ToolStripButton("➕ Thêm mới", null, (_, _) => ClearFields()));
        return toolStrip;
    }

    private StatusStrip BuildStatusStrip()
    {
        var status = new StatusStrip();
        status.Items.Add(countStatusLabel);
        status.Items.Add(new ToolStripStatusLabel("  |  "));
        status.Items.Add(valueStatusLabel);
        status.Items.Add(new ToolStripStatusLabel { Spring = true });
        status.Items.Add(timeStatusLabel);
        return status;
    }

    private Control BuildContent()
    {
        var split = new SplitContainer
        {
            Size = new Size(900, 650),
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 400,
            Panel1MinSize = 350,
            Panel2MinSize = 500
        };

        var details = new GroupBox
        {
            Text = "Thông tin sách",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        var detailLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(4)
        };
        detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 222));
        detailLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        detailLayout.Controls.Add(BuildBookFields(), 0, 0);
        detailLayout.Controls.Add(BuildCoverPanel(), 0, 1);
        detailLayout.Controls.Add(BuildActionButtons(), 0, 2);
        details.Controls.Add(detailLayout);
        split.Panel1.Controls.Add(details);

        var listPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(8)
        };
        listPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        listPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        listPanel.Controls.Add(BuildSearchPanel(), 0, 0);
        listPanel.Controls.Add(BuildBooksGrid(), 0, 1);
        split.Panel2.Controls.Add(listPanel);
        return split;
    }

    private Control BuildBookFields()
    {
        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6,
            Padding = new Padding(0, 4, 0, 0)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < 6; i++)
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6));

        AddField(fields, 0, "Tên sách:", titleTextBox);
        AddField(fields, 1, "Tác giả:", authorTextBox);
        AddField(fields, 2, "ISBN:", isbnTextBox);
        AddField(fields, 3, "Thể loại:", categoryTextBox);
        AddField(fields, 4, "Đơn giá:", priceTextBox);
        AddField(fields, 5, "Số lượng:", quantityTextBox);
        return fields;
    }

    private static void AddField(TableLayoutPanel layout, int row, string labelText, TextBox textBox)
    {
        layout.Controls.Add(new Label
        {
            Text = labelText,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            AutoSize = false
        }, 0, row);
        textBox.Dock = DockStyle.Fill;
        textBox.Margin = new Padding(3, 4, 3, 4);
        layout.Controls.Add(textBox, 1, row);
    }

    private Control BuildCoverPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 4, 0, 4) };
        picBookCover.Name = "picBookCover";
        picBookCover.Dock = DockStyle.Fill;
        picBookCover.BorderStyle = BorderStyle.FixedSingle;
        picBookCover.SizeMode = PictureBoxSizeMode.Zoom;
        picBookCover.BackColor = SystemColors.ControlLight;

        var loadCoverButton = new Button
        {
            Text = "Tải ảnh bìa",
            Dock = DockStyle.Bottom,
            Height = 34
        };
        loadCoverButton.Click += (_, _) => LoadCover();
        panel.Controls.Add(picBookCover);
        panel.Controls.Add(loadCoverButton);
        return panel;
    }

    private Control BuildActionButtons()
    {
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 5, 0, 0)
        };
        var addButton = new Button { Text = "Thêm sách", AutoSize = true };
        var updateButton = new Button { Text = "Cập nhật", AutoSize = true };
        var deleteButton = new Button { Text = "Xóa", AutoSize = true };
        var clearButton = new Button { Text = "Làm mới", AutoSize = true };
        addButton.Click += (_, _) => AddBook();
        updateButton.Click += (_, _) => UpdateSelectedBook();
        deleteButton.Click += (_, _) => DeleteSelectedBook();
        clearButton.Click += (_, _) => ClearFields();
        buttons.Controls.AddRange([addButton, updateButton, deleteButton, clearButton]);
        return buttons;
    }

    private Control BuildSearchPanel()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 5, 0, 0)
        };
        panel.Controls.Add(new Label
        {
            Text = "Tìm kiếm:",
            AutoSize = true,
            Margin = new Padding(3, 6, 8, 3)
        });
        searchTextBox.Width = 270;
        searchTextBox.PlaceholderText = "Tên sách, tác giả, ISBN hoặc thể loại";
        searchTextBox.TextChanged += (_, _) => ApplySearchFilter();
        panel.Controls.Add(searchTextBox);
        return panel;
    }

    private Control BuildBooksGrid()
    {
        dgvBooks.Dock = DockStyle.Fill;
        dgvBooks.ReadOnly = true;
        dgvBooks.AllowUserToAddRows = false;
        dgvBooks.AllowUserToDeleteRows = false;
        dgvBooks.AutoGenerateColumns = false;
        dgvBooks.MultiSelect = false;
        dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBooks.RowHeadersVisible = false;
        dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvBooks.Columns.Add(CreateColumn("Tên sách", nameof(BookModel.Title), 2));
        dgvBooks.Columns.Add(CreateColumn("Tác giả", nameof(BookModel.Author), 1.4f));
        dgvBooks.Columns.Add(CreateColumn("ISBN", nameof(BookModel.Isbn), 1.1f));
        dgvBooks.Columns.Add(CreateColumn("Thể loại", nameof(BookModel.Category), 1));
        dgvBooks.Columns.Add(CreateColumn("Đơn giá", nameof(BookModel.UnitPrice), 1));
        dgvBooks.Columns.Add(CreateColumn("SL", nameof(BookModel.Quantity), 0.5f));
        dgvBooks.Columns.Add(CreateColumn("Ảnh bìa", nameof(BookModel.CoverFileName), 1));
        dgvBooks.SelectionChanged += (_, _) => ShowSelectedBook();
        return dgvBooks;
    }

    private static DataGridViewTextBoxColumn CreateColumn(string header, string property, float fillWeight)
    {
        return new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            DataPropertyName = property,
            FillWeight = fillWeight,
            SortMode = DataGridViewColumnSortMode.Automatic,
            DefaultCellStyle = property == nameof(BookModel.UnitPrice)
                ? new DataGridViewCellStyle { Format = "N0" }
                : new DataGridViewCellStyle()
        };
    }

    private void LoadCover()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh bìa sách",
            Filter = "Tệp ảnh (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            SetCoverImage(dialog.FileName);
            coverPath = dialog.FileName;
        }
        catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is OutOfMemoryException || ex is System.Runtime.InteropServices.ExternalException)
        {
            MessageBox.Show(this, $"Không thể tải ảnh bìa: {ex.Message}", "Lỗi tải ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetCoverImage(string path)
    {
        Image? image = null;
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            using var source = Image.FromFile(path);
            image = new Bitmap(source);
        }

        var previous = picBookCover.Image;
        picBookCover.Image = image;
        previous?.Dispose();
    }

    private void AddBook()
    {
        if (!TryReadFields(out var book))
            return;

        books.Add(book);
        ClearFields();
        MessageBox.Show(this, "Đã thêm sách vào danh sách.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateSelectedBook()
    {
        if (selectedBook is null)
        {
            MessageBox.Show(this, "Vui lòng chọn sách cần cập nhật.", "Chưa chọn sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!TryReadFields(out var editedBook))
            return;

        selectedBook.Title = editedBook.Title;
        selectedBook.Author = editedBook.Author;
        selectedBook.Isbn = editedBook.Isbn;
        selectedBook.Category = editedBook.Category;
        selectedBook.UnitPrice = editedBook.UnitPrice;
        selectedBook.Quantity = editedBook.Quantity;
        selectedBook.CoverPath = editedBook.CoverPath;
        UpdateStatus();
        ApplySearchFilter();
    }

    private bool TryReadFields(out BookModel book)
    {
        book = new BookModel();
        var title = titleTextBox.Text.Trim();
        var author = authorTextBox.Text.Trim();
        if (title.Length == 0 || author.Length == 0)
        {
            MessageBox.Show(this, "Tên sách và tác giả không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (!decimal.TryParse(priceTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price) || price < 0)
        {
            MessageBox.Show(this, "Đơn giá phải là số không âm.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            priceTextBox.Focus();
            return false;
        }
        if (!int.TryParse(quantityTextBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var quantity) || quantity < 0)
        {
            MessageBox.Show(this, "Số lượng phải là số nguyên không âm.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            quantityTextBox.Focus();
            return false;
        }

        book.Title = title;
        book.Author = author;
        book.Isbn = isbnTextBox.Text.Trim();
        book.Category = categoryTextBox.Text.Trim();
        book.UnitPrice = price;
        book.Quantity = quantity;
        book.CoverPath = coverPath;
        return true;
    }

    private void DeleteSelectedBook()
    {
        if (selectedBook is null)
        {
            MessageBox.Show(this, "Vui lòng chọn sách cần xóa.", "Chưa chọn sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var result = MessageBox.Show(this, $"Xóa sách ‘{selectedBook.Title}’ khỏi danh sách?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
            return;

        books.Remove(selectedBook);
        ClearFields();
    }

    private void ClearFields()
    {
        selectedBook = null;
        dgvBooks.ClearSelection();
        titleTextBox.Clear();
        authorTextBox.Clear();
        isbnTextBox.Clear();
        categoryTextBox.Clear();
        priceTextBox.Clear();
        quantityTextBox.Clear();
        coverPath = string.Empty;
        SetCoverImage(string.Empty);
        titleTextBox.Focus();
    }

    private void ShowSelectedBook()
    {
        if (dgvBooks.CurrentRow?.DataBoundItem is not BookModel book)
            return;

        selectedBook = book;
        titleTextBox.Text = book.Title;
        authorTextBox.Text = book.Author;
        isbnTextBox.Text = book.Isbn;
        categoryTextBox.Text = book.Category;
        priceTextBox.Text = book.UnitPrice.ToString(CultureInfo.CurrentCulture);
        quantityTextBox.Text = book.Quantity.ToString(CultureInfo.CurrentCulture);
        coverPath = book.CoverPath;
        SetCoverImage(coverPath);
    }

    private void ApplySearchFilter()
    {
        var query = searchTextBox.Text.Trim();
        selectedBook = null;
        dgvBooks.CurrentCell = null;
        dgvBooks.ClearSelection();
        foreach (DataGridViewRow row in dgvBooks.Rows)
        {
            if (row.DataBoundItem is not BookModel book)
                continue;
            row.Visible = query.Length == 0
                || book.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || book.Author.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || book.Isbn.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || book.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    private void OpenCsv()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Mở danh sách sách từ CSV",
            Filter = "Tệp CSV (*.csv)|*.csv",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var importedBooks = ReadCsv(dialog.FileName);
            books.Clear();
            foreach (var book in importedBooks)
                books.Add(book);
            ClearFields();
            MessageBox.Show(this, $"Đã nhập {importedBooks.Count} đầu sách.", "Nhập dữ liệu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is FormatException)
        {
            MessageBox.Show(this, $"Không thể đọc file CSV: {ex.Message}", "Lỗi nhập dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static List<BookModel> ReadCsv(string filePath)
    {
        var importedBooks = new List<BookModel>();
        using var reader = new StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var recordNumber = 0;
        foreach (var fields in ParseCsvRecords(reader))
        {
            recordNumber++;
            if (fields.All(string.IsNullOrWhiteSpace))
                continue;
            if (recordNumber == 1 && fields.Count > 0 && fields[0].Equals("Title", StringComparison.OrdinalIgnoreCase))
                continue;
            if (fields.Count != 7)
                throw new FormatException($"Bản ghi {recordNumber} phải có đúng 7 cột.");
            if (!decimal.TryParse(fields[4], NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price < 0)
                throw new FormatException($"Đơn giá không hợp lệ ở bản ghi {recordNumber}.");
            if (!int.TryParse(fields[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) || quantity < 0)
                throw new FormatException($"Số lượng không hợp lệ ở bản ghi {recordNumber}.");

            var cover = fields[6];
            if (cover.Length > 0 && !Path.IsPathRooted(cover))
                cover = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath)!, cover));
            importedBooks.Add(new BookModel
            {
                Title = fields[0],
                Author = fields[1],
                Isbn = fields[2],
                Category = fields[3],
                UnitPrice = price,
                Quantity = quantity,
                CoverPath = cover
            });
        }
        return importedBooks;
    }

    private static IEnumerable<List<string>> ParseCsvRecords(TextReader reader)
    {
        var fields = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;
        int nextCharacter;
        while ((nextCharacter = reader.Read()) != -1)
        {
            var character = (char)nextCharacter;
            if (character == '"')
            {
                if (inQuotes && reader.Peek() == '"')
                {
                    field.Append('"');
                    reader.Read();
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (character == ',' && !inQuotes)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else if ((character == '\r' || character == '\n') && !inQuotes)
            {
                if (character == '\r' && reader.Peek() == '\n')
                    reader.Read();
                fields.Add(field.ToString());
                yield return fields;
                fields = [];
                field.Clear();
            }
            else
            {
                field.Append(character);
            }
        }

        if (inQuotes)
            throw new FormatException("Chuỗi được đặt trong dấu ngoặc kép chưa đóng.");
        if (fields.Count > 0 || field.Length > 0)
        {
            fields.Add(field.ToString());
            yield return fields;
        }
    }

    private void SaveCsv()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Lưu danh sách sách thành CSV",
            Filter = "Tệp CSV (*.csv)|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            FileName = "DanhSachSach.csv",
            OverwritePrompt = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            using var writer = new StreamWriter(dialog.FileName, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            writer.WriteLine("Title,Author,Isbn,Category,UnitPrice,Quantity,CoverPath");
            foreach (var book in books)
            {
                var values = new[]
                {
                    book.Title,
                    book.Author,
                    book.Isbn,
                    book.Category,
                    book.UnitPrice.ToString(CultureInfo.InvariantCulture),
                    book.Quantity.ToString(CultureInfo.InvariantCulture),
                    book.CoverPath
                };
                writer.WriteLine(string.Join(',', values.Select(EscapeCsvField)));
            }
            MessageBox.Show(this, "Đã lưu danh sách sách.", "Xuất dữ liệu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"Không thể lưu file CSV: {ex.Message}", "Lỗi xuất dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string EscapeCsvField(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\r') && !value.Contains('\n'))
            return value;
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private void UpdateStatus()
    {
        countStatusLabel.Text = $"Tổng số đầu sách: {books.Count}";
        var inventoryValue = books.Sum(book => book.UnitPrice * book.Quantity);
        valueStatusLabel.Text = $"Tổng giá trị kho: {inventoryValue.ToString("N0", CultureInfo.CurrentCulture)} đ";
    }

    private void UpdateClock()
    {
        timeStatusLabel.Text = DateTime.Now.ToString("HH:mm:ss  dd/MM/yyyy", CultureInfo.CurrentCulture);
    }
}

public sealed class BookModel : INotifyPropertyChanged
{
    private string title = string.Empty;
    private string author = string.Empty;
    private string isbn = string.Empty;
    private string category = string.Empty;
    private decimal unitPrice;
    private int quantity;
    private string coverPath = string.Empty;

    public string Title { get => title; set => SetProperty(ref title, value); }
    public string Author { get => author; set => SetProperty(ref author, value); }
    public string Isbn { get => isbn; set => SetProperty(ref isbn, value); }
    public string Category { get => category; set => SetProperty(ref category, value); }
    public decimal UnitPrice { get => unitPrice; set => SetProperty(ref unitPrice, value); }
    public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }
    public string CoverPath { get => coverPath; set => SetProperty(ref coverPath, value); }
    public string CoverFileName => string.IsNullOrWhiteSpace(CoverPath) ? string.Empty : Path.GetFileName(CoverPath);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if (propertyName == nameof(CoverPath))
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoverFileName)));
    }
}
