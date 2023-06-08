using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL;

public class AuthorBookDal : IAuthorBookDal
{
    #region DataMember
    private const string TbName = "[dbo].[AuthorBook]";
    #endregion

    #region Fetch

    public async Task<List<AuthorBook>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<AuthorBook>($@"SELECT * from {TbName}");
        return result.ToList();
    }

    public async Task<AuthorBook?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<AuthorBook>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }

    public async Task<bool> DeleteByBookId(long bookId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE FROM {TbName} WHERE BookId = @bookId";
        var rowsCount = await db.ExecuteAsync(query, new { bookId });
        return rowsCount > 0;
    }

    //public async Task<AuthorBook?> GetByBookId(long bookid)
    //{
    //    using var db = new DbEntityObject().GetConnectionString();
    //    var result = await db.QueryAsync<AuthorBook>($@"Select * From {TbName} WHERE BookId = @bookid", new { bookid });
    //    return result.SingleOrDefault();
    //}
    #endregion

    #region Insert
    public async Task<long> Insert(AuthorBook authorBook)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@BookId", authorBook.BookId);
        prams.Add("@AuthorId", authorBook.AuthorId);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [BookId]
                                      ,[AuthorId]
                               )
                               VALUES
                               (
                                       @BookId
                                      ,@AuthorId
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(AuthorBook authorBook)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [BookId] = @BookId
                                       ,[AuthorId] = @AuthorId
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            authorBook.BookId,
            authorBook.AuthorId,
            authorBook.Id
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
