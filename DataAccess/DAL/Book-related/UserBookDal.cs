using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Book_related;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL.Book_related;

public class UserBookDal : IUserBookDal
{
    #region DataMember
    private const string TbName = "[dbo].[UserBook]";
    #endregion

    #region Fetch
    public async Task<List<UserBook>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"Select * from {TbName}");
        return result.ToList();
    }

    public async Task<List<UserBook>> GetAllByUserId(long userid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"Select * from {TbName} where UserId = @userid", new {userid});
        return result.ToList();
    }

    public async Task<UserBook?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }

    public async Task<UserBook?> GetByBookId(long bookid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"Select * From {TbName} WHERE BookId = @bookid", new { bookid });
        return result.SingleOrDefault();
    }

    public async Task<UserBook?> GetByBookIdAndUserId(long userid, long bookid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<UserBook>($@"Select * From {TbName} WHERE UserId = @userid and BookId = @bookid", new { userid, bookid });
        return result.SingleOrDefault();
    }

    public async Task<List<Book>> GetUserLibrary(long userid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"select * from UserBook  left join Book on Book.Id = UserBook.BookId where UserId = @userid and IsAdded = 'true'";
        var result = await db.QueryAsync<Book>(query, new { userid });
        return result.ToList();
    }

    public async Task<List<Book>> GetUserBookMarks(long userid)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"select * from UserBook left join Book on Book.Id = UserBook.BookId where UserId = @userid and IsMarked = 'true'";
        var result = await db.QueryAsync<Book>(query, new { userid });
        return result.ToList();
    }


    #endregion

    #region Insert
    //public async Task<long> AddToLibrary(UserBook ub)
    //{
    //    using var db = new DbEntityObject().GetConnectionString();

    //    var prams = new DynamicParameters();
    //    prams.Add("@UserId", ub.UserId);
    //    prams.Add("@BookId", ub.BookId);
    //    prams.Add("@CreateOn", ub.CreateOn);
    //    //prams.Add("@IsMarked", ub.IsAdded);
    //    prams.Add("@IsAdded", ub.IsAdded);
    //    //prams.Add("@IsPurchase", ub.IsPurchase);



    //    var query = $@"insert into {TbName} (
    //                               UserId,
    //                               BookId, 
    //                               CreateOn, 
    //                               IsMarked                               
                             
    //                               )
    //                        Values (
    //                               @UserId, 
    //                               @BookId, 
    //                               @CreateOn, 
    //                               'true'
    //                               );
    //                               SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

    //    var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
    //    return result;
    //}

    public async Task<long> Insert(UserBook ub)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@UserId", ub.UserId);
        prams.Add("@BookId", ub.BookId);
        prams.Add("@CreateOn", ub.CreateOn);
        prams.Add("@IsMarked", ub.IsMarked);
        prams.Add("@IsAdded", ub.IsAdded);
        prams.Add("@IsPurchase", ub.IsPurchase);



        var query = $@"insert into {TbName} (
                                   UserId,
                                   BookId, 
                                   CreateOn, 
                                   IsMarked,                                
                                   IsAdded, 
                                   IsPurchase
                                   ) 
                            Values (
                                   @UserId, 
                                   @BookId, 
                                   @CreateOn, 
                                   @IsMarked, 
                                   @IsAdded, 
                                   @IsPurchase
                                   );
                                   SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(UserBook ub)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [UserId] = @UserId
                                       ,[BookId] = @BookId
                                       ,[CreateOn] = @CreateOn
                                       ,[IsMarked] = @IsMarked
                                       ,[IsAdded] = @IsAdded
                                       ,[IsPurchase] = @IsPurchase
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            ub.UserId,
            ub.BookId,
            ub.CreateOn,
            ub.IsMarked,
            ub.IsAdded,
            ub.IsPurchase,
            ub.Id,
        });

        return result;
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(long Id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.ExecuteAsync($@"DELETE from {TbName} where Id=@Id", new { Id });
        return result > 0;
    }
    #endregion
}
