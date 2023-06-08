using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.DAL;

public class AuthorDal : IAuthorDal
{
    #region DataMember
    private const string TbName = "[dbo].[Author]";
    #endregion

    #region Fetch
    public async Task<(List<AuthorViewModel> data, int totalCount)> GetList(AuthorGetListFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = string.Empty;

        if (filterModel.AuthorId.HasValue)
        {
            whereQuery += @"AND us.Id = @Id ";
            prams.Add("Id", filterModel.AuthorId.Value);
        }
        else
        {

            if (!string.IsNullOrEmpty(filterModel.AuthorFirstName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.AuthorFirstName) LIKE @AuthorFirstName ";
                prams.Add("AuthorFirstName", $"%{filterModel.AuthorFirstName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.AuthorLastName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.AuthorLastName) LIKE @AuthorLastName ";
                prams.Add("AuthorLastName", $"%{filterModel.AuthorLastName.Trim().ToLower()}%");
            }

            if (filterModel.Birthday.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.Birthday) = @Birthday ";
                prams.Add("Birthday", filterModel.Birthday.Value);
            }


            if (!string.IsNullOrEmpty(filterModel.Country?.Trim()))
            {
                whereQuery += @"AND LOWER(us.Country) LIKE @Country ";
                prams.Add("Country", $"%{filterModel.Country.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.Language?.Trim()))
            {
                whereQuery += @"AND us.Language LIKE @Language ";
                prams.Add("Language", $"%{filterModel.Language.Trim()}%");
            }

            if (filterModel.Age > 0)
            {
                whereQuery += @"AND us.Age LIKE @Age ";
                prams.Add("Age", $"%{filterModel.Age}%");
            }

        }
        #endregion

        #region Sql Query
        var skip = 0;
        if (filterModel.PageNumber > 0)
        {
            skip = filterModel.PageNumber * filterModel.PageSize;
        }

        prams.Add("Skip", skip);

        whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;

        var sqlQuery = $@"SELECT 
                                       us.Id
                                      ,us.AuthorFirstName
                                      ,us.AuthorLastName
                                      ,us.ProfilePictureName
                                      ,us.Birthday
                                      ,us.Country
                                      ,us.Language
                                      ,us.Age
                                      ,us.Bio                                                                 
                                  FROM {TbName} AS us  
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";

        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<AuthorViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
  
        #endregion
    }
    public async Task<List<Author>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Author>($@"Select * from {TbName}");
        return result.ToList();
    }
    public async Task<Author?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Author>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }

    //public async Task<Author?> GetByEmail(string? email)
    //{
    //    using var db = new DbEntityObject().GetConnectionString();
    //    email = email?.ToLower();
    //    var result = await db.QueryAsync<Author>($@"Select * From {TbName} WHERE lower(Email) = lower(@email)", new { email });
    //    return result.SingleOrDefault();
    //}

    public async Task<Author?> GetByFirstnameAndLastname(string? authorfirstname, string? authorlastname)
    {
        using var db = new DbEntityObject().GetConnectionString();
        authorfirstname = authorfirstname?.ToLower();
        authorlastname = authorlastname?.ToLower();
        var query = $@"Select * From {TbName} WHERE lower(AuthorFirstName)=lower(@AuthorFirstName) and lower(AuthorLastName)=lower(@AuthorLastName);";
        var result = await db.QueryAsync<Author>(query, new { authorfirstname, authorlastname });
        return result.SingleOrDefault();
    }

    public async Task<AuthorProfileViewModel?> GetAuthorProfileById(long authorId)
    {
        using var db = new DbEntityObject().GetConnectionString();

        //down query is without join

        //var query = $@"SELECT Id,AuthorFirstName,AuthorLastName,ProfilePictureId,ProfilePictureName,Birthday,Email,Country,Language,Age,Bio
        //             ,(SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM Rate where EntityType = 1 and EntityId = @entityid) AS Rate FROM {TbName}";

        var query = $@" 
                       SELECT AuthorFirstName, AuthorLastName, ProfilePictureName, Birthday, Country, Language, Age, Bio, 
                       (SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM Rate where EntityType = 1 and EntityId = Author.Id) AS AverageRate, 
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 2) AS DisLikes,
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 1) AS Likes
                        FROM Author  
                      where Id = @authorId";
        var result = await db.QueryAsync<AuthorProfileViewModel>(query, new { authorId });
        return result.SingleOrDefault();
    }

    public async Task<List<AuthorProfileViewModel>> GetALlAuthorProfile()
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"
                       SELECT AuthorFirstName, AuthorLastName, ProfilePictureName, Birthday, Country, Language, Age, Bio, 
                       (SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM Rate where EntityType = 1 and EntityId = Author.Id) AS AverageRate, 
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 2) AS DisLikes
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 1) AS Likes,
                       FROM Author ";
        var result = await db.QueryAsync<AuthorProfileViewModel>(query);
        return result.ToList();
    }

    public async Task<List<AuthorScore>> GetLightprofile()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"SELECT
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 2) AS DisLikes,
                       (SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM Rate where EntityType = 1 and EntityId = Author.Id) AS AverageRate, 
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 1) AS Likes
                       FROM Author";
        var result = await db.QueryAsync<AuthorScore>(query);
        return result.ToList();
    }

    public async Task<AuthorScore?> GetlightprofileById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"SELECT
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 2) AS DisLikes,
                       (SELECT CAST(AVG(CAST(RateValue AS  DECIMAL(10,1))) AS DECIMAL(10,1)) AS Rate FROM Rate where EntityType = 1 and EntityId = Author.Id) AS AverageRate, 
                       (SELECT COUNT(*) from [Like] where EntityType = 1 and EntityId = Author.Id and Type = 1) AS Likes
                       FROM Author
                       where Author.Id = @id";
        var result = await db.QueryAsync<AuthorScore>(query, new { id });
        return result.SingleOrDefault();
    }

    //public async Task<List<Book>> BooksByAuthorId(long id)
    //{
    //    using var db = new DbEntityObject().GetConnectionString();
    //    var query = $@"select b.BookName,b.Author,b.Translator,b.Publisher,b.YearOfPublication,b.BookFormat,b.BookType,b.NumberOfPages,b.Language,b.ISBN,b.BookSubject from Book as b
    //                   Inner join AuthorBook on AuthorBook.BookId = b.Id
    //                   Inner join Author on Author.Id = AuthorBook.AuthorId
    //                   where Author.Id = @id";
    //    var result = await db.QueryAsync<Book>(query, new { id });
    //    return result.ToList();
    //}



    public async Task<List<Book>> BooksByAuthorId(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"select b.BookName,b.Publisher,b.YearOfPublication,b.BookFormat,b.BookType,b.NumberOfPages,b.Language,b.ISBN from Book as b
                        Inner join AuthorBook on AuthorBook.BookId = b.Id
                        Inner join Author on Author.Id = AuthorBook.AuthorId
                        where b.Id = @bookid 
                        
                        select au.AuthorFirstName, au.AuthorLastName from AuthorBook as ab
                        left join Author as au on ab.AuthorId = au.Id
                        left join Book as bo on bo.Id = ab.BookId
                        where bo.Id = @bookid
                        
                        select COUNT(*) from AuthorBook as ab
                        left join Author as au on ab.AuthorId = au.Id
                        left join Book as bo on bo.Id = ab.BookId
                        where bo.Id = @bookid
                        
                        select tr.TranslatorFirstName, tr.TranslatorLastName from TranslatorBook as tb
                        left join Translator as tr on tb.TranslatorId = tr.Id
                        left join Book as bo on bo.Id = tb.BookId
                        where bo.Id = @bookid
                        
                        
                        select COUNT(*) from TranslatorBook as tb
                        left join Translator as tr on tb.TranslatorId = tr.Id
                        left join Book as bo on bo.Id = tb.BookId
                       where bo.Id = @bookid";

        var multiData = await db.QueryMultipleAsync(query, new { BookId= id });
        var data = (await multiData.ReadAsync<Book>()).ToList();
        //var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return data;
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Author author)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@AuthorFirstName", author.AuthorFirstName);
        prams.Add("@AuthorLastName", author.AuthorLastName);
        prams.Add("@ProfilePictureId", author.ProfilePictureId);
        prams.Add("@ProfilePictureName", author.ProfilePictureName);
        prams.Add("@Birthday", author.Birthday);
        //prams.Add("@Email", author.Email);
        prams.Add("@Country", author.Country);
        prams.Add("@Language", author.Language);
        prams.Add("@Age", author.Age);
        prams.Add("@Bio", author.Bio);


        var query = $@"insert into {TbName} (
                                   AuthorFirstName,
                                   AuthorLastName, 
                                   ProfilePictureId, 
                                   ProfilePictureName,                                
                                   Birthday, 
               
                                   Country, 
                                   Language, 
                                   Age, 
                                   Bio) 
                            Values (
                                   @AuthorFirstName, 
                                   @AuthorLastName, 
                                   @ProfilePictureId, 
                                   @ProfilePictureName, 
                                   @Birthday, 
                   
                                   @Country, 
                                   @Language, 
                                   @Age, 
                                   @Bio
                                   );
                                   SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(Author author)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [AuthorFirstName] = @AuthorFirstName
                                       ,[AuthorLastName] = @AuthorLastName
                                       ,[ProfilePictureId] = @ProfilePictureId
                                       ,[ProfilePictureName] = @ProfilePictureName
                                       ,[Birthday] = @Birthday
                        
                                       ,[Country] = @Country
                                       ,[Language] = @Language
                                       ,[Age] = @Age
                                       ,[Bio] = @Bio
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            author.AuthorFirstName,
            author.AuthorLastName,
            author.ProfilePictureId,
            author.ProfilePictureName,
            author.Birthday,
            //author.Email,
            author.Country,
            author.Language,
            author.Age,
            author.Bio,
            author.Id
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