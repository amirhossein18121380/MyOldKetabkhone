using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models.Security;

namespace DataAccess.DAL.Security;

public class MessagesDal : IMessagesDal
{
    #region DataMember
    private const string TableName = "[Security].[Messages]";
    #endregion

    #region Fetch
    public async Task<List<Messages>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<Messages>($@"Select * From {TableName} WHERE IsDeleted = false")).ToList();
    }

    public async Task<(List<Messages> data, int totalCount)> GetListByUserId(long userId, int pageSize, int pageNumber)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var skip = 0;
        if (pageNumber > 0)
        {
            skip = pageNumber * pageSize;
        }

        var sqlQuery = $@"Select * From {TableName}
                                  WHERE UserId = @userId AND IsDeleted = 'False'
                                  ORDER BY Id DESC OFFSET @skip ROWS FETCH NEXT {pageSize} ROWS ONLY;

                                  Select COUNT(1) From {TableName}
                                  WHERE UserId = @userId AND IsDeleted = 'False'";

        using var multiData = await db.QueryMultipleAsync(sqlQuery, new { userId, skip });
        var data = (await multiData.ReadAsync<Messages>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
    }

    public async Task<Messages?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<Messages>($@"Select * From {TableName} WHERE Id = @id",
            new { id });

        return result.SingleOrDefault();
    }

    public async Task<int> GetUserUnreadMessageCount(long userId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<int>($@"SELECT COUNT(1) FROM {TableName}
                                                            WHERE UserId = @userId AND IsRead = false AND IsDeleted = false",
            new { userId });

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Messages entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@UserId", entity.UserId);
        prams.Add("@PanelMessageId", entity.PanelMessageId);
        prams.Add("@Subject", entity.Subject);
        prams.Add("@MessageContent", entity.MessageContent);
        prams.Add("@IsRead", entity.IsRead);
        prams.Add("@ReadDate", entity.ReadDate);
        prams.Add("@IsDeleted", entity.IsDeleted);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       UserId
                                      ,PanelMessageId
                                      ,Subject
                                      ,MessageContent
                                      ,IsRead
                                      ,ReadDate
                                      ,IsDeleted
                                      ,CreatorId
                                      ,CreateOn
                               )
                               VALUES
                               (
                                       @UserId
                                      ,@PanelMessageId
                                      ,@Subject
                                      ,@MessageContent
                                      ,@IsRead
                                      ,@ReadDate
                                      ,@IsDeleted
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(Messages entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        UserId = @UserId
                                       ,PanelMessageId = @PanelMessageId
                                       ,Subject = @Subject
                                       ,MessageContent = @MessageContent
                                       ,IsRead = @IsRead
                                       ,ReadDate = @ReadDate
                                       ,IsDeleted = @IsDeleted
                                       ,CreatorId = @CreatorId
                                       ,CreateOn = @CreateOn
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.UserId,
            entity.PanelMessageId,
            entity.Subject,
            entity.MessageContent,
            entity.IsRead,
            entity.ReadDate,
            entity.IsDeleted,
            entity.CreatorId,
            entity.CreateOn,
            entity.Id
        });

        return rowsAffected;
    }
    public async Task<bool> UpdateMessageIsRead(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                  SET IsRead = true
                                     ,ReadDate = @ReadDate                                       
                                  WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            ReadDate = DateTime.Now,
            Id = id
        });

        return rowsAffected > 0;
    }
    public async Task<bool> UpdateIsDeleted(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                  SET IsDeleted = true
                                  WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            Id = id
        });

        return rowsAffected > 0;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"DELETE FROM {TableName} WHERE Id = @Id";
        var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });
        return rowsCount > 0;
    }
    #endregion
}

