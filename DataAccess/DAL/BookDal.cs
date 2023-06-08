using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.DAL;

public class BookDal : IBookDal
{
    #region DataMember
    private const string TbName = "[dbo].[Book]";
    #endregion

    #region Search & Fetch

    public async Task<(List<BookListViewModel> data, int totalCount)> GetList(BookGetListFilterViewModel filterModel)
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
            if (!string.IsNullOrEmpty(filterModel.BookName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.BookName) LIKE @BookName ";
                prams.Add("BookName", $"%{filterModel.BookName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.Author?.Trim()))
            {
                whereQuery += @"AND LOWER(us.Author) LIKE @Author ";
                prams.Add("Author", $"%{filterModel.Author.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.Translator?.Trim()))
            {
                whereQuery += @"AND LOWER(us.Translator) LIKE @Translator ";
                prams.Add("Translator", $"%{filterModel.Translator.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.Publisher?.Trim()))
            {
                whereQuery += @"AND LOWER(us.Publisher) LIKE @Publisher ";
                prams.Add("Publisher", $"%{filterModel.Publisher.Trim().ToLower()}%");
            }

            if (filterModel.YearOfPublication.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('year', us.YearOfPublication) = @YearOfPublication ";
                prams.Add("YearOfPublication", filterModel.YearOfPublication.Value);
            }

            if (filterModel.BookFormat.HasValue)
            {
                whereQuery += @"AND us.BookFormat = @BookFormat ";
                prams.Add("BookFormat", filterModel.BookFormat.Value);
            }

            if (filterModel.BookType.HasValue)
            {
                whereQuery += @"AND us.BookType = @BookType ";
                prams.Add("BookType", filterModel.BookType.Value);
            }

            if (filterModel.NumberOfPages > 0)
            {
                whereQuery += @"AND us.MobileNumber LIKE @NumberOfPages ";
                prams.Add("NumberOfPages", $"%{filterModel.NumberOfPages}%");
            }

            if (!string.IsNullOrEmpty(filterModel.Language?.Trim()))
            {
                whereQuery += @"AND LOWER(us.Language) LIKE @Language ";
                prams.Add("Language", $"%{filterModel.Language.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.BookSubject?.Trim()))
            {
                whereQuery += @"AND LOWER(us.BookSubject) LIKE @BookSubject ";
                prams.Add("BookSubject", $"%{filterModel.BookSubject.Trim().ToLower()}%");
            }

            if (filterModel.ElectronicVersionPrice > 0)
            {
                whereQuery += @"AND us.ElectronicVersionPrice LIKE @ElectronicVersionPrice ";
                prams.Add("ElectronicVersionPrice", $"%{filterModel.ElectronicVersionPrice}%");
            }

            if (filterModel.IsActive.HasValue)
            {
                whereQuery += @"AND us.IsActive = @IsActive ";
                prams.Add("IsActive", filterModel.IsActive.Value);
            }

            if (filterModel.IsDeleted.HasValue)
            {
                whereQuery += @"AND us.IsDeleted = @IsDeleted ";
                prams.Add("IsDeleted", filterModel.IsDeleted.Value);
            }

            if (filterModel.IsModified.HasValue)
            {
                whereQuery += @"AND us.IsModified = @IsModified ";
                prams.Add("IsModified", filterModel.IsModified.Value);
            }

            if (filterModel.LastModified.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.LastModified) = @LastModified ";
                prams.Add("LastModified", filterModel.LastModified.Value);
            }

            if (filterModel.CreateOn.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.CreateOn) = @CreateOn ";
                prams.Add("CreateOn", filterModel.CreateOn.Value);
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
                                      ,us.BookName
                                     
                                      ,us.Publisher
                                      ,us.YearOfPublication
                                      ,us.BookType
                                      ,us.BookFormat
                                      ,us.NumberOfPages
                                      ,us.Language
                                      ,us.ISBN
                               
                                      ,us.ElectronicVersionPrice
                                      ,us.BookPictureName
                                      ,us.IsActive
                                      ,us.IsDeleted
                                      ,us.IsModified
                                      ,us.ModifierId
                                      ,us.LastModified    
                                      ,us.CreateOn
                                  FROM {TbName} AS us
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;


                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";


        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<BookListViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();
        //var totalCount = data.Count();

        return (data, totalCount);
        #endregion
    }

    public async Task<Book?> GetBookById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"Select * From {TbName} WHERE Id=@id ", new { id });
        return result.SingleOrDefault();
    }

    public async Task<List<Author>> GetAuthorsByBookId(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"select au.Id, au.AuthorFirstName, au.AuthorLastName from AuthorBook as ab
                       left join Author as au on au.Id = ab.AuthorId
                       left join Book as bo on bo.Id = ab.BookId
                       where bo.Id = @bookid";

        var result = (await db.QueryAsync<Author>(query, new { bookid = id })).ToList();
        return result;
    }

    //public async Task<List<long>> GetAuthorsidByBookId(long id)
    //{
    //    using var db = new DbEntityObject().GetConnectionString();
    //    var query = $@"select ab.AuthorId from AuthorBook as ab where ab.BookId = @bookid";

    //    var result = (await db.QueryAsync<long>(query, new { bookid = id })).ToList();
    //    return result;
    //}

    public async Task<List<Translator>> GetTranslatorByBookId(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"
                       select tr.Id, tr.TranslatorFirstName, tr.TranslatorLastName from TranslatorBook as tb 
                       left join Translator as tr on tr.Id = tb.TranslatorId
                       left join Book as bo on bo.Id = tb.BookId
                       where bo.Id = @bookid";

        var result = (await db.QueryAsync<Translator>(query, new { bookid = id })).ToList();
        return result;
    }

    public async Task<List<Category>> GetCategoryByBookId(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"
                       select ca.Id, ca.Title from BookCategory as bc 
                       left join Category as ca on ca.Id = bc.CategoryId
                       left join Book as bo on bo.Id = bc.BookId
                       where bo.Id = @bookid";

        var result = (await db.QueryAsync<Category>(query, new { bookid = id })).ToList();
        return result;
    }

    public async Task<List<Subjects>> GetSubjectsByBookId(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"
                       select su.Id, su.Title from BookSubject as bs 
                       left join Subject as su on su.Id = bs.SubjectId
                       left join Book as bo on bo.Id = bs.BookId
                       where bo.Id = @bookid";
        var result = (await db.QueryAsync<Subjects>(query, new { bookid = id })).ToList();
        return result;
    }

    public async Task<List<Book>> GetAllPublishers()
    {
        //yuo can use (using) instead of using try and finally because (using) keyword by default put that in scope and close or dispose that.
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"select Book.Publisher from {TbName} where ");
        return result.ToList();
    }

    public async Task<List<Book>> GetByAuthor(string author)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"select * from {TbName} where Author=@author", new { author });
        return result.ToList();
    }

    public async Task<List<Book>> GetByBookName(string bookName)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"select * from {TbName} where BookName=@bookName", new { bookName });
        return result.ToList();
    }

    public async Task<List<Book>> GetByPublisher(string publisher)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"select * from {TbName} where Publisher=@publisher", new { publisher });
        return result.ToList();
    }

    public async Task<List<Book>> GetByTranslator(string translator)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"select * from {TbName} where Translator=@translator", new { translator });
        return result.ToList();
    }

    public async Task<List<Book>> GetByBookSubject(string subject)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"select * from {TbName} where BookSubject=@subject", new { subject });
        return result.ToList();
    }

    public async Task<Book?> GetByISBN(long isbn)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = (await db.QueryAsync<Book>($@"Select * from {TbName} where ISBN=@ISBN", new { isbn })).SingleOrDefault();
        return result;
    }

    public async Task<Book?> GetByBookPictureId(long bookPictureId)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Book>($@"Select * from {TbName} where BookPictureId=@BookPictureId", new { bookPictureId });
        return result.SingleOrDefault();
    }

    #endregion

    #region Update
    public async Task<int> UpdateBook(Book book)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"update {TbName} " +
            "set " +
            "ParentId=@ParentId," +
            "BookName=@BookName," +

       
            "Publisher=@Publisher," +
            "YearOfPublication=@YearOfPublication," +
            "BookFormat=@BookFormat," +
            "BookType=@BookType," +
            "NumberOfPages=@NumberOfPages," +
            "Language=@Language," +
            "ISBN=@ISBN," +
         
            "ElectronicVersionPrice=@ElectronicVersionPrice," +
            "BookPictureName=@BookPictureName," +
            "BookPictureId=@BookPictureId," +
            "IsActive=@IsActive," +
            "IsDeleted=@IsDeleted," +
            "IsModified=@IsModified," +
            "ModifierId=@ModifierId, " +
            "LastModified=@LastModified, " +
            "CreateOn=@CreateOn " +
            "WHERE Id = @Id";

        var parameter = new DynamicParameters();
        parameter.Add("ParentId", book.ParentId);
        parameter.Add("BookName", book.BookName);
        //parameter.Add("Author", book.Author);
        //parameter.Add("Translator", book.Translator);
        parameter.Add("Publisher", book.Publisher);
        parameter.Add("YearOfPublication", book.YearOfPublication);
        parameter.Add("BookFormat", book.BookFormat);
        parameter.Add("BookType", book.BookType);
        parameter.Add("NumberOfPages", book.NumberOfPages);
        parameter.Add("Language", book.Language);
        parameter.Add("ISBN", book.ISBN);
        //parameter.Add("BookSubject", book.BookSubject);
        parameter.Add("ElectronicVersionPrice", book.ElectronicVersionPrice);
        parameter.Add("BookPictureName", book.BookPictureName);
        parameter.Add("BookPictureId", book.BookPictureId);
        parameter.Add("IsActive", book.IsActive);
        parameter.Add("IsDeleted", book.IsDeleted);
        parameter.Add("IsModified", book.IsModified);
        parameter.Add("ModifierId", book.ModifierId);
        parameter.Add("LastModified", book.LastModified);
        parameter.Add("CreateOn", book.CreateOn);
        parameter.Add("Id", book.Id);

        var result = await db.ExecuteAsync(query, parameter);

        return result;
    }

    #endregion

    #region InsertBook

    public async Task<long> AddBook(Book book)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"Insert into {TbName}
                                (   
                                   ParentId
                                  ,BookName
                            
                                  ,Publisher
                                  ,YearOfPublication
                                  ,BookFormat
                                  ,BookType
                                  ,NumberOfPages
                                  ,Language
                                  ,ISBN
                           
                                  ,ElectronicVersionPrice
                                  ,BookPictureName
                                  ,BookPictureId
                                  ,IsActive
                                  ,IsDeleted
                                  ,IsModified
                                  ,ModifierId
                                  ,LastModified
                                  ,CreateOn
                                )
                                VALUES
                                (
                                  @ParentId
                                 ,@BookName
                               
                                 ,@Publisher
                                 ,@YearOfPublication
                                 ,@BookFormat
                                 ,@BookType
                                 ,@NumberOfPages
                                 ,@Language
                                 ,@ISBN
                              
                                 ,@ElectronicVersionPrice
                                 ,@BookPictureName
                                 ,@BookPictureId
                                 ,@IsActive
                                 ,@IsDeleted
                                 ,@IsModified
                                 ,@ModifierId
                                 ,@LastModified
                                 ,@CreateOn
                                 )

                                 SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var parameteres = new DynamicParameters();
        parameteres.Add("ParentId", book.ParentId);
        parameteres.Add("BookName", book.BookName);
        //parameteres.Add("Author", book.Author);
        //parameteres.Add("Translator", book.Translator);
        parameteres.Add("Publisher", book.Publisher);
        parameteres.Add("YearOfPublication", book.YearOfPublication);
        parameteres.Add("BookFormat", book.BookFormat);
        parameteres.Add("BookType", book.BookType);
        parameteres.Add("NumberOfPages", book.NumberOfPages);
        parameteres.Add("Language", book.Language);
        parameteres.Add("ISBN", book.ISBN);
        //parameteres.Add("BookSubject", book.BookSubject);
        parameteres.Add("ElectronicVersionPrice", book.ElectronicVersionPrice);
        parameteres.Add("BookPictureName", book.BookPictureName);
        parameteres.Add("BookPictureId", book.BookPictureId);
        parameteres.Add("IsActive", book.IsActive);
        parameteres.Add("IsDeleted", book.IsDeleted);
        parameteres.Add("IsModified", book.IsModified);
        parameteres.Add("ModifierId", book.ModifierId);
        parameteres.Add("LastModified", book.LastModified);
        parameteres.Add("CreateOn", book.CreateOn);


        var result = (await db.QueryAsync<long>(query, parameteres)).SingleOrDefault();
        return result;
    }

    #endregion

    public async Task<int> GetTheAverageRateOfBookById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = "SELECT AVG(Rate) FROM Vote join Book on Vote.BookRefrenceId = Book.Id where Book.Id=@Id";
        var result = await db.QuerySingleOrDefault(query, new { id });
        return result;
    }

}