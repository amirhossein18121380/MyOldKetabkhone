
using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL;

public class BookCategoryDal : IBookCategoryDal
{
    #region DataMember
    private const string TbName = "[dbo].[BookCategory]";
    #endregion

    #region Fetch

    public async Task<List<BookCategory>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<BookCategory>($@"SELECT * from {TbName}");
        return result.ToList();
    }

    public async Task<bool> DeleteByBookId(long bookId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"DELETE FROM {TbName} WHERE BookId = @bookId";
        var rowsCount = await db.ExecuteAsync(query, new { bookId });
        return rowsCount > 0;
    }
    public async Task<BookCategory?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<BookCategory>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(BookCategory bookCategory)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@BookId", bookCategory.BookId);
        prams.Add("@CategoryId", bookCategory.CategoryId);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [BookId]
                                      ,[CategoryId]
                               )
                               VALUES
                               (
                                       @BookId
                                      ,@CategoryId
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(BookCategory bookCategory)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [BookId] = @BookId
                                       ,[CategoryId] = @CategoryId
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            bookCategory.BookId,
            bookCategory.CategoryId,
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
