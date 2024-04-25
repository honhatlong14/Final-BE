using Common.Constants;

namespace Common.Exception;

public class AppException : System.Exception
{
    public StringEnum.AppErrorCode ErrorCode { get; set; }

    public AppException(string message, StringEnum.AppErrorCode errorCode = StringEnum.AppErrorCode.Error) :
        base(message)
    {
        ErrorCode = errorCode;
    }

    public AppException(string message, System.Exception ex,
        StringEnum.AppErrorCode errorCode = StringEnum.AppErrorCode.Error) : base(message, ex)
    {
        ErrorCode = errorCode;
    }
    
}