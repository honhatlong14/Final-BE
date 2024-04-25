using Common.Constants;

namespace Data.ViewModels;

public class AppErrorModel
{
    public string Message { get; set; }
    public string Detail { get; set; }

    public StringEnum.AppErrorCode ErrorCode { get; set; }

    public AppErrorModel()
    {
    }

    public AppErrorModel(string message, StringEnum.AppErrorCode errorCode = StringEnum.AppErrorCode.Error)
    {
        Message = message;
        ErrorCode = errorCode;
    }

    public AppErrorModel(string message, string detail, StringEnum.AppErrorCode errorCode = StringEnum.AppErrorCode.Error) : this(message)
    {
        Detail = detail;
        ErrorCode = errorCode;
    }
}