using Dapper;
using DataAccess.Tool;
using DataModel.Common;
using DataModel.ViewModel.Common;

namespace DataAccess.DAL.Common;

public class LogDal
{
    #region DataMember
    private const string TbName = "[dbo].[LogHistory]";
    #endregion

    #region Log History
    public static async Task InsertLogAsync(LogModel log)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var parameters = new DynamicParameters();
        parameters.Add("Level", log.Level);
        parameters.Add("MethodName", log.MethodName);
        parameters.Add("Message", log.Message);
        parameters.Add("StackTrace", log.StackTrace);
        parameters.Add("CreateDate", log.CreateDate);

        var query = $@"INSERT into {TbName} (
                                              Level
                                              ,MethodName
                                              ,Message
                                              ,StackTrace
                                              ,CreateDate) 
                                              values
                                            (
                                              @Level
                                              ,@MethodName
                                              ,@Message
                                              ,@StackTrace
                                              ,@CreateDate
                                            )";
        await db.QueryAsync(query, parameters);
    }

    public static void InsertLog(LogModel log)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var parameters = new DynamicParameters();
        parameters.Add("Level", log.Level);
        parameters.Add("MethodName", log.MethodName);
        parameters.Add("Message", log.Message);
        parameters.Add("StackTrace", log.StackTrace);
        parameters.Add("CreateDate", log.CreateDate);

        var query = $@"INSERT into {TbName} (Level,MethodName,Message,StackTrace,CreateDate) values(@Level,@MethodName,@Message,@StackTrace,@CreateDate)";
        db.QueryAsync(query, parameters);
    }

    public static async Task<(long totalCount, List<LogModel> items)> GetLogs(LogFilterViewModel filterModel)
    {
        try
        {
            using var db = new DbEntityObject().GetConnectionString();

            #region Validation
            if (filterModel.PageSize <= 0)
            {
                return (0, new List<LogModel>());
            }
            #endregion

            #region Query Builder
            var skip = 0;
            if (filterModel.PageNumber > 0)
            {
                skip = filterModel.PageNumber * filterModel.PageSize;
            }

            var prams = new DynamicParameters();
            prams.Add("Skip", skip);
            prams.Add("PageSize", filterModel.PageSize);

            var whereQuery = @"WHERE [Level] > 0 ";

            if (filterModel.Level.HasValue)
            {
                whereQuery += @"AND [Level] = @LogLevel ";
                prams.Add("LogLevel", filterModel.Level.Value);
            }

            if (!string.IsNullOrEmpty(filterModel.Message?.Trim()))
            {
                whereQuery += @"AND LOWER(Message) LIKE @Message ";
                prams.Add("Message", $"%{filterModel.Message.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.MethodName?.Trim()))
            {
                whereQuery += @"AND LOWER(MethodName) LIKE @MethodName ";
                prams.Add("MethodName", $"%{filterModel.MethodName.Trim().ToLower()}%");

            }

            if (!string.IsNullOrEmpty(filterModel.StackTrace?.Trim()))
            {
                whereQuery += @"AND LOWER(StackTrace) LIKE @StackTrace ";
                prams.Add("StackTrace", $"%{filterModel.StackTrace.Trim().ToLower()}%");
            }

            if (filterModel.FromDate.HasValue)
            {
                whereQuery += @"AND CreateDate >= @FromDate ";
                prams.Add("FromDate", filterModel.FromDate.Value.ToUniversalTime());
            }

            if (filterModel.ToDate.HasValue)
            {
                whereQuery += @"AND CreateDate <= @ToDate ";
                prams.Add("ToDate", filterModel.ToDate.Value.ToUniversalTime());
            }
            #endregion

            #region Create Result
            var sqlQuery = $@"SELECT *
                                  FROM {TbName}
                                  {whereQuery}
                                  ORDER BY Id DESC OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY

                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";

            using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

            var data = (await multiData.ReadAsync<LogModel>()).ToList();
            var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();


            return (totalCount, data);
            #endregion
        }
        catch (Exception exp)
        {
            //await MongoLogging.ErrorLogAsync("MongoDal|GetLogs", exp.Message, exp.StackTrace);
            return (0, new List<LogModel>());
        }
    }

    #endregion
}
