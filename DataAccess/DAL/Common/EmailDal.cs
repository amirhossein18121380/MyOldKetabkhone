using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Common;

namespace DataAccess.DAL.Common;

public class EmailDal : IEmailDal
{
    #region DataMember
    private const string TableName = @"Common.EmailTemplate";
    #endregion

    #region Fetch
    public async Task<List<EmailTemplate>> GetList()
    {
        using var db = new DbEntityObject().GetConnectionString();

        return (await db.QueryAsync<EmailTemplate>($@"Select * From {TableName}")).ToList();
    }

    public async Task<EmailTemplate?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var result = await db.QueryAsync<EmailTemplate>($@"Select * From {TableName} WHERE Id = @id",
            new { id });

        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(EmailTemplate entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@Subject", entity.Subject);
        prams.Add("@EmailContent", entity.EmailContent);
        prams.Add("@SendType", entity.SendType);
        prams.Add("@IsSend", entity.IsSend);
        prams.Add("@SenderId", entity.SenderId);
        prams.Add("@SendDate", entity.SendDate);
        prams.Add("@FilterValue", entity.FilterValue);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       Subject
                                      ,EmailContent
                                      ,SendType
                                      ,IsSend
                                      ,SenderId
                                      ,SendDate
                                      ,FilterValue
                                      ,CreatorId
                                      ,CreateOn
                               )
                               VALUES
                               (
                                       @Subject
                                      ,@EmailContent
                                      ,@SendType
                                      ,@IsSend
                                      ,@SenderId
                                      ,@SendDate
                                      ,@FilterValue
                                      ,@CreatorId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(EmailTemplate entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        Subject = @Subject
                                       ,EmailContent = @EmailContent
                                       ,SendType = @SendType
                                       ,IsSend = @IsSend
                                       ,SenderId = @SenderId
                                       ,SendDate = @SendDate
                                       ,FilterValue = @FilterValue
                                       ,CreatorId = @CreatorId
                                       ,CreateOn = @CreateOn
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.Subject,
            entity.EmailContent,
            entity.SendType,
            entity.IsSend,
            entity.SenderId,
            entity.SendDate,
            entity.FilterValue,
            entity.CreatorId,
            entity.CreateOn,
            entity.Id
        });

        return rowsAffected;
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
