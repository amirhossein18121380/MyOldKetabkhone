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

public class BookSubjectDal : IBookSubjectDal
{
    #region DataMember
    private const string TbName = "[dbo].[BookSubject]";
    #endregion

    #region Fetch

    public async Task<List<BookSubject>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<BookSubject>($@"SELECT * from {TbName}");
        return result.ToList();
    }

    public async Task<bool> DeleteByBookId(long bookId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE FROM {TbName} WHERE BookId = @bookId";
        var rowsCount = await db.ExecuteAsync(query, new { bookId });
        return rowsCount > 0;
    }

    public async Task<BookSubject?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<BookSubject>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(BookSubject bookCategory)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@BookId", bookCategory.BookId);
        prams.Add("@SubjectId", bookCategory.SubjectId);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [BookId]
                                      ,[SubjectId]
                               )
                               VALUES
                               (
                                       @BookId
                                      ,@SubjectId
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(BookSubject bookCategory)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        BookId = @BookId,
                                        SubjectId = @SubjectId
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            bookCategory.BookId,
            bookCategory.SubjectId,
            bookCategory.Id
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