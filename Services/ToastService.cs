namespace CustomerShop.Services
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public ToastType Type { get; set; } = ToastType.Info;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public interface IToastService
    {
        event Action<ToastMessage>? OnShow;
        event Action<string>? OnHide;
        void ShowSuccess(string message, string title = "Thành công");
        void ShowError(string message, string title = "Lỗi");
        void ShowWarning(string message, string title = "Cảnh báo");
        void ShowInfo(string message, string title = "Thông báo");
    }

    public class ToastService : IToastService
    {
        public event Action<ToastMessage>? OnShow;
        public event Action<string>? OnHide;

        public void ShowSuccess(string message, string title = "Thành công")
        {
            OnShow?.Invoke(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Success
            });
        }

        public void ShowError(string message, string title = "Lỗi")
        {
            OnShow?.Invoke(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Error
            });
        }

        public void ShowWarning(string message, string title = "Cảnh báo")
        {
            OnShow?.Invoke(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Warning
            });
        }

        public void ShowInfo(string message, string title = "Thông báo")
        {
            OnShow?.Invoke(new ToastMessage
            {
                Title = title,
                Message = message,
                Type = ToastType.Info
            });
        }
    }
}
