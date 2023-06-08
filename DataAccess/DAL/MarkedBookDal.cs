using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL;

public class MarkedBookDal
{
    #region DataMember
    private const string TbName = "[dbo].[MarkedBook]";
    #endregion

    #region Fetch

    public async Task<List<UserBook>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"SELECT * from {TbName}");
        return result.ToList();
    }

    public async Task<UserBook?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(UserBook markedBook)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@UserId", markedBook.UserId);
        prams.Add("@BookId", markedBook.BookId);
        prams.Add("@CreateOn", markedBook.CreateOn);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [UserId]
                                       [BookId]
                                      ,[CreateOn]
                               )
                               VALUES
                               (
                                       @UserId
                                       @BookId
                                      ,@CreateOn
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = await db.QuerySingleOrDefaultAsync<long>(query, prams);
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(UserBook markedBook)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [UserId] = @UserId
                                        [BookId] = @BookId
                                       ,[CreateOn] = @CreateOn
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            markedBook.UserId,
            markedBook.BookId,
            markedBook.CreateOn,
            markedBook.Id
        });

        return result;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long Id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QuerySingleOrDefaultAsync($@"DELETE * from {TbName} where Id=@Id", new { Id });
        return result > 0;
    }
    #endregion
}
