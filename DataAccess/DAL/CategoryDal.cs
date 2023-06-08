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

public class CategoryDal : ICategoryDal
{
    #region DataMember
    private const string TbName = "[dbo].[Category]";
    #endregion

    #region Fetch

    public async Task<List<Category>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Category>($@"SELECT * from {TbName}");
        return result.ToList();
    }
    public async Task<Category?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Category>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }
    public async Task<List<Category>> GetChild(long parentid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Category>($@"Select * from {TbName} where ParentId=@ParentId", new { parentid });
        return result.ToList();
    }
    public async Task<List<Category>> GetParent()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Category>($@"Select * from {TbName} where ParentId IS NULL");
        return result.ToList();
    }
    #endregion

    #region Insert
    public async Task<bool> Insert(Category category)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@ParentId", category.ParentId);
        prams.Add("@Title", category.Title);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [ParentId]
                                      ,[Title]
                               )
                               VALUES
                               (
                                       @ParentId
                                      ,@Title
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = await db.ExecuteAsync(query, prams);
        return result > 0;
    }

    public async Task<bool> AddChild(Category category)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@ParentId", category.Id);
        prams.Add("@Title", category.Title);

        var query = $@"INSERT INTO {TbName} 
                               (
                                       [ParentId]
                                      ,[Title]
                               )
                               VALUES
                               (
                                       @ParentId
                                      ,@Title
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = await db.QuerySingleOrDefaultAsync<long>(query, prams);
        return result > 0;
    }

    #endregion

    #region Update
    public async Task<int> Update(Category category)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [ParentId] = @ParentId
                                       ,[Title] = @Title
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            category.ParentId,
            category.Title,
            category.Id
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
