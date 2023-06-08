using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Common;

public class LogModel
{
    public LogModel(LogLevelEnum level, string methodName, string message, string? stackTrace = null)
    {
        Level = (short)level;
        MethodName = methodName;
        Message = message;
        StackTrace = stackTrace;
        CreateDate = DateTime.Now;
    }

    public LogModel()
    {

    }

    public long Id { get; set; }
    public short Level { get; set; }
    public string MethodName { get; set; }
    public string Message { get; set; }
    public string? StackTrace { get; set; }
    public DateTime CreateDate { get; set; }
}

public enum LogLevelEnum
{
    Info = 1,
    Warning = 2,
    Error = 3
}

