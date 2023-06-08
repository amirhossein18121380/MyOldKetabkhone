using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface;
using DataAccess.Tool;
using DataModel.Models;
using DataModel.ViewModel;

namespace DataAccess.DAL;

public class TranslatorDal : ITranslatorDal
{
    #region DataMember
    private const string TbName = "[dbo].[Translator]";
    #endregion

    #region Fetch

    public async Task<(List<TranslatorGetListViewModel> data, int totalCount)> GetList(TranslatorGetListFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = string.Empty;

        if (filterModel.TranslatorId.HasValue)
        {
            whereQuery += @"AND us.Id = @Id ";
            prams.Add("Id", filterModel.TranslatorId.Value);
        }
        else
        {
            if (!string.IsNullOrEmpty(filterModel.TranslatorFirstName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.TranslatorFirstName) LIKE @TranslatorFirstName ";
                prams.Add("TranslatorFirstName", $"%{filterModel.TranslatorFirstName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.TranslatorLastName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.TranslatorLastName) LIKE @TranslatorLastName ";
                prams.Add("TranslatorLastName", $"%{filterModel.TranslatorLastName.Trim().ToLower()}%");
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
                whereQuery += @"AND LOWER(us.Language) LIKE @Language ";
                prams.Add("Language", $"%{filterModel.Language.Trim().ToLower()}%");
            }

            if (filterModel.Age > 0)
            {
                whereQuery += @"AND us.Age LIKE @Age ";
                prams.Add("Age", $"%{filterModel.Age}%");
            }

            //if (!string.IsNullOrEmpty((filterModel.Age)?.ToString().Trim()))
            //{
            //    whereQuery += @"AND us.Age LIKE @Age ";
            //    prams.Add("Age", $"%{(filterModel.Age)?.ToString().Trim()}%");
            //}
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
                                      ,us.TranslatorFirstName
                                      ,us.TranslatorLastName
                                      ,us.Birthday
                                      ,us.Country
                                      ,us.Language
                                      ,us.Age
                                      ,us.Bio
                                      ,us.ProfilePictureName
                                  FROM {TbName} AS us
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;


                                  Select COUNT(1) 
                                  FROM {TbName}
                                  {whereQuery}";
        #endregion
        whereQuery = whereQuery.StartsWith("AND") ? $"WHERE{whereQuery.Substring(3, whereQuery.Length - 3)}" : whereQuery;
        #region Get Data From Db

        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);

        var data = (await multiData.ReadAsync<TranslatorGetListViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);

        #endregion
    }

    public async Task<List<Translator>> GetAll()
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Translator>($@"Select * from {TbName}");
        return result.ToList();
    }

    public async Task<Translator?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<Translator>($@"Select * From {TbName} WHERE Id = @id", new { id });
        return result.SingleOrDefault();
    }

    //public async Task<Translator?> GetByEmail(string? email)
    //{
    //    using var db = new DbEntityObject().GetConnectionString();
    //    email = email?.ToLower();
    //    var result = await db.QueryAsync<Translator>($@"Select * From {TbName} WHERE lower(Email) = lower(@email)", new { email });
    //    return result.SingleOrDefault();
    //}

    public async Task<Translator?> GetByFirstnameAndLastname(string? translatorfirstname, string? translatorlastname)
    {
        using var db = new DbEntityObject().GetConnectionString();
        translatorfirstname = translatorfirstname?.ToLower();
        translatorlastname = translatorlastname?.ToLower();
        var query = $@"Select * From {TbName} WHERE lower(TranslatorFirstName)=lower(@TranslatorFirstName) and lower(TranslatorLastName)=lower(@TranslatorLastName);";
        var result = await db.QueryAsync<Translator>(query, new { translatorfirstname, translatorlastname });
        return result.SingleOrDefault();
    }
    #endregion

    #region Insert
    public async Task<long> Insert(Translator translator)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@TranslatorFirstName", translator.TranslatorFirstName);
        prams.Add("@TranslatorLastName", translator.TranslatorLastName);
        prams.Add("@Birthday", translator.Birthday);
        prams.Add("@Country", translator.Country);
        prams.Add("@Language", translator.Language);
        prams.Add("@Age", translator.Age);
        prams.Add("@Bio", translator.Bio);
        prams.Add("@ProfilePictureId", translator.ProfilePictureId);
        prams.Add("@ProfilePictureName", translator.ProfilePictureName);


        var query = $@"insert into {TbName} (
                                   TranslatorFirstName,
                                   TranslatorLastName,                            
                                   Birthday, 
                                   Country, 
                                   Language,                                  
                                   Age,                                  
                                   Bio,
                                   ProfilePictureId,
                                   ProfilePictureName) 
                            Values (
                                   @TranslatorFirstName, 
                                   @TranslatorLastName, 
                                   @Birthday, 
                                   @Country, 
                                   @Language, 
                                   @Age, 
                                   @Bio,
                                   @ProfilePictureId,
                                   @ProfilePictureName
                                   )
                                   SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

        var result = (await db.QueryAsync<long>(query, prams)).SingleOrDefault();
        return result;
    }
    #endregion

    #region Update
    public async Task<int> Update(Translator translator)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var query = $@"UPDATE {TbName} 
                                   SET 
                                        [TranslatorFirstName] = @TranslatorFirstName
                                       ,[TranslatorLastName] = @TranslatorLastName                                
                                       ,[Birthday] = @Birthday
                              
                                       ,[Country] = @Country
                                       ,[Language] = @Language                                      
                                       ,[Age] = @Age                                      
                                       ,[Bio] = @Bio
                                       ,[ProfilePictureId] = @ProfilePictureId
                                       ,[ProfilePictureName] = @ProfilePictureName
                                   WHERE Id = @Id";

        var result = await db.ExecuteAsync(query, new
        {
            translator.TranslatorFirstName,
            translator.TranslatorLastName,
            translator.Birthday,
            translator.Country,
            translator.Language,
            translator.Age,
            translator.Bio,
            translator.ProfilePictureId,
            translator.ProfilePictureName,
            translator.Id
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
