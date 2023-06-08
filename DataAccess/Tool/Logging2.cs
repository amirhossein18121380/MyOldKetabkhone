using DataAccess.DAL.Common;
using DataModel.Common;

namespace DataAccess.Tool;

public static  class Logging2
{
    public static async Task InfoLogAsync(string methodName, string infoMessage)
    {
        await LogDal.InsertLogAsync(new LogModel(LogLevelEnum.Info, methodName, infoMessage));
    }

    public static void InfoLogSync(string methodName, string infoMessage)
    {
        LogDal.InsertLog(new LogModel(LogLevelEnum.Info, methodName, infoMessage));
    }

    public async static Task WarningLogAsync(string methodName, string warningMessage)
    {
        await LogDal.InsertLogAsync(new LogModel(LogLevelEnum.Warning, methodName, warningMessage));
    }

    public static void WarningLogSync(string methodName, string warningMessage)
    {
        LogDal.InsertLog(new LogModel(LogLevelEnum.Info, methodName, warningMessage));
    }

    public static async Task ErrorLogAsync(string methodName, string errorMessage, string? stackTrace)
    {
        await LogDal.InsertLogAsync(new LogModel(LogLevelEnum.Error, methodName, errorMessage, stackTrace));
    }

    public static void ErrorLogSync(string methodName, string errorMessage, string? stackTrace)
    {
        LogDal.InsertLog(new LogModel(LogLevelEnum.Error, methodName, errorMessage, stackTrace));
    }
}


//example of how to use Logging2 is down below

//catch (Exception ex)
//{
//      await Logging2.ErrorLogAsync("Controllername|methodname", ex.Message, ex.StackTrace);
//      return HttpHelper.ExceptionContent(ex);
//}