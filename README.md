# BÁO CÁO BÀI TẬP THỰC HÀNH LẬP TRÌNH C# WINFORMS

## I. THÔNG TIN SINH VIÊN

- **Họ và tên:** Lê Tuấn Anh
- **Mã số sinh viên:** 24810320204
- **Lớp học phần:** D19QTANM1
- **Môn học:** Lập trình C# / Windows Forms
- **Link Repository:** [LTNET_24810320204_LeTuanAnh](https://github.com/letuananh9206-creator/LTNET_24810320204_LeTuanAnh)

---

## II. DANH SÁCH BÀI TẬP HOÀN THÀNH VÀ TỰ ĐÁNH GIÁ

| Bài tập | Tên bài tập | Trạng thái | Mức độ hoàn thành |
|:---:|---|:---:|:---:|
| Bài 1 | Quản lý dịch vụ phòng Gym và hội viên | Đã thực hiện | Theo kết quả thực tế |
| Bài 2 | Quản lý và xuất báo cáo nhà sách | Đã thực hiện | Theo kết quả thực tế |
| Bài 3 | Hệ thống MDI quản lý bệnh viện và xử lý Async/Await | Đã thực hiện | Theo kết quả thực tế |
| Bài 4 | Bảng điều khiển thống kê doanh thu đa luồng | Đã thực hiện | Theo kết quả thực tế |
| Bài 5 | Mô phỏng giám sát tỷ giá tài chính thời gian thực | Đã thực hiện | Theo kết quả thực tế |

---

## III. MINH HỌA GIAO DIỆN VÀ KỸ THUẬT NỔI BẬT

### 1. Bài 1: Quản lý dịch vụ phòng Gym và hội viên

**Kỹ thuật áp dụng:**
- Cấu hình DataGridView bằng code với AutoGenerateColumns = false.
- Data Binding hai chiều với BindingSource.
- Kiểm tra dữ liệu đầu vào bằng ErrorProvider.

**Giao diện chính:**

![Giao diện chính Bài 1](docs/bai1_main.png)

**Kiểm tra dữ liệu không hợp lệ:**

![Validation Bài 1](docs/bai1_error.png)

---

### 2. Bài 2: Quản lý và xuất báo cáo nhà sách

**Kỹ thuật áp dụng:**
- OpenFileDialog để chọn ảnh bìa sách.
- Đọc và ghi dữ liệu bằng file CSV.
- Cập nhật trạng thái thao tác bằng StatusStrip.

**Giao diện quản lý sách:**

![Giao diện chính Bài 2](docs/bai2_main.png)

**Minh họa đọc hoặc xuất CSV:**

![Chức năng CSV Bài 2](docs/bai2_csv.png)

---

### 3. Bài 3: Hệ thống MDI quản lý bệnh viện và Async/Await

**Kỹ thuật áp dụng:**
- MDI Container quản lý các Form con.
- Ngăn mở trùng Form con.
- Sử dụng async/await để xử lý tác vụ bất đồng bộ.
- Sử dụng Invoke khi cần cập nhật giao diện từ luồng khác.

**Giao diện MDI:**

![Giao diện MDI Bài 3](docs/bai3_mdi.png)

**Minh họa xử lý bất đồng bộ:**

![Async/Await Bài 3](docs/bai3_async.gif)

---

### 4. Bài 4: Bảng điều khiển thống kê doanh thu đa luồng

**Kỹ thuật áp dụng:**
- Task.Run kết hợp async/await.
- Giữ giao diện phản hồi trong khi xử lý dữ liệu.
- CancellationTokenSource để hủy tác vụ.
- ProgressBar và Label hiển thị trạng thái xử lý.

**Giao diện thống kê doanh thu:**

![Giao diện doanh thu Bài 4](docs/bai4_main.png)

**Minh họa xử lý bất đồng bộ và hủy tác vụ:**

![Async và Cancel Bài 4](docs/bai4_async.png)
![Async và Cancel Bài 4](docs/bai4_async_run.png)
![Async và Cancel Bài 4](docs/bai4_async_cancel.png)

---

### 5. Bài 5: Mô phỏng giám sát tỷ giá tài chính thời gian thực

**Kỹ thuật áp dụng:**
- Task.Run chạy vòng lặp giám sát nền.
- Cập nhật dữ liệu định kỳ mỗi 2 giây.
- Invoke hoặc InvokeRequired bảo đảm an toàn luồng khi cập nhật giao diện.
- Ghi nhật ký biến động giá vào DataGridView.
- Đổi màu giá theo chiều hướng tăng hoặc giảm.

**Giao diện giám sát tỷ giá:**

![Giao diện giám sát Bài 5](docs/bai5_main.png)

**Lịch sử biến động giá:**

![Lịch sử giá Bài 5](docs/bai5_history.png)

**Minh họa thay đổi màu giá:**

![Thay đổi màu giá Bài 5](docs/bai5_price_change.png)

---

## IV. CẤU TRÚC THƯ MỤC DỰ ÁN

```text
LTNET_24810320204_LeTuanAnh/
├── .gitignore
├── README.md
├── docs/
│   ├── bai1_main.png
│   ├── bai1_error.png
│   ├── bai2_main.png
│   ├── bai2_csv.png
│   ├── bai3_mdi.png
│   ├── bai3_async.gif
│   ├── bai4_main.png
│   ├── bai4_async_cancel.png
│   ├── bai5_main.png
│   ├── bai5_history.png
│   └── bai5_price_change.png
└── src/
    └── WinFormsAppSolution.sln
```

---

## V. HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN

### 1. Yêu cầu môi trường

- Windows.
- Visual Studio 2022.
- Workload .NET desktop development.
- .NET SDK phù hợp với phiên bản dự án.

### 2. Các bước thực hiện

**Bước 1:** Clone repository về máy.

```bash
git clone https://github.com/letuananh9206-creator/LTNET_24810320204_LeTuanAnh.git
```

**Bước 2:** Mở file `src/WinFormsAppSolution.sln` bằng Visual Studio 2022.

**Bước 3:** Chọn dự án cần chạy và đặt làm Startup Project nếu Solution có nhiều dự án.

**Bước 4:** Nhấn F5 hoặc chọn Start để biên dịch và chạy chương trình.

---

## VI. KẾT LUẬN

Qua các bài tập thực hành, sinh viên vận dụng kiến thức lập trình C# Windows Forms, quản lý và liên kết dữ liệu, xử lý file, xây dựng giao diện MDI và lập trình bất đồng bộ. Các bài tập giúp rèn luyện kỹ năng kiểm tra dữ liệu đầu vào, xử lý tác vụ nền và cập nhật giao diện an toàn giữa các luồng.

**Sinh viên thực hiện:** Lê Tuấn Anh

**Mã số sinh viên:** 24810320204

**Lớp:** D19QTANM1
