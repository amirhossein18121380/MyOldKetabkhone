using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL;

public class SubjectDal : ISubjectDal
{
    #region DataMember
    private const string TbName = "[dbo].[Subject]";
    #endregion

    #region Fetch

    public async Task<List<Subjects>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Subjects>($@"SELECT * from {TbName}");
        return result.ToList();
    }
    public async Task<Subjects?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Subjects>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Subjects sub)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@Title", sub.Title);

        var query = $@"INSERT INTO {TbName} 
                               (
                                      ,[Title]
                               )
                               VALUES
                               (
                                      ,@Title
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }

    #endregion

    #region Update
    public async Task<int> Update(Subjects sub)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                       ,[Title] = @Title
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            sub.Title,
            sub.Id
        });

        return result;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long Id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.ExecuteAsync($@"Delete from {TbName} where Id=@Id", new { Id });
        return result > 0;
    }
    #endregion
}
