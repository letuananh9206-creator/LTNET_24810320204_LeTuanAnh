using System.ComponentModel;
using System.Globalization;

namespace bt1
{
    public class MemberModel : INotifyPropertyChanged
    {
        private string _memberId = string.Empty;
        private string _fullName = string.Empty;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private string _package = string.Empty;
        private DateTime _startDate = DateTime.Today;
        private DateTime _expirationDate = DateTime.Today.AddMonths(1);
        private long _fee;
        private string _feeInput = string.Empty;

        public string MemberId
        {
            get => _memberId;
            set => SetProperty(ref _memberId, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Package
        {
            get => _package;
            set => SetProperty(ref _package, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set => SetProperty(ref _expirationDate, value);
        }

        public long Fee
        {
            get => _fee;
            set => SetProperty(ref _fee, value);
        }

        public string FeeInput
        {
            get => _feeInput;
            set => SetProperty(ref _feeInput, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MemberModel Copy() => new()
        {
            MemberId = MemberId,
            FullName = FullName,
            Phone = Phone,
            Email = Email,
            Package = Package,
            StartDate = StartDate,
            ExpirationDate = ExpirationDate,
            Fee = Fee,
            FeeInput = Fee.ToString(CultureInfo.InvariantCulture)
        };

        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
