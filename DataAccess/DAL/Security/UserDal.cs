using Common.Enum;
using Dapper;
using DataAccess.Interface.Security;
using DataAccess.Tool;
using DataModel.Models;
using DataModel.ViewModel.Security.User;

namespace DataAccess.DAL.Security;

public class UserDal : IUser
{
    #region DataMember
    private const string TableName = "[dbo].[Users]";
    #endregion

    #region Fetch
    //public async Task<List<User>> GetList()
    //{
    //    using var db = new DbEntityObject().GetConnectionString();

    //    var result = await db.QueryAsync<User>($@"Select * From {TableName}");

    //    return result.ToList();
    //}

    public async Task<(List<UserGetListViewModel> data, int totalCount)> GetList(UserGetListFilterViewModel filterModel)
    {
        using var db = new DbEntityObject().GetConnectionString();

        #region Set Where Param
        var prams = new DynamicParameters();

        var whereQuery = @"WHERE wal.EntityType = 1 AND wal.WalletType = 1 ";

        if (filterModel.UserId.HasValue)
        {
            whereQuery += @"AND us.Id = @Id ";
            prams.Add("Id", filterModel.UserId.Value);
        }
        else
        {
            if (!string.IsNullOrEmpty(filterModel.UserName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.UserName) LIKE @UserName ";
                prams.Add("UserName", $"%{filterModel.UserName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.DisplayName?.Trim()))
            {
                whereQuery += @"AND ((us.DisplayName IS NOt NULL AND LOWER(us.DisplayName) LIKE @DisplayName) OR (us.FirstName IS NOT NULL AND us.LastName IS NOT NULL AND LOWER(us.FirstName + ' ' + us.LastName) LIKE @DisplayName) OR (us.UserName IS NOT NULL AND LOWER(us.UserName) LIKE @DisplayName))";
                prams.Add("DisplayName", $"%{filterModel.DisplayName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.FirstName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.FirstName) LIKE @FirstName ";
                prams.Add("FirstName", $"%{filterModel.FirstName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.LastName?.Trim()))
            {
                whereQuery += @"AND LOWER(us.LastName) LIKE @LastName ";
                prams.Add("LastName", $"%{filterModel.LastName.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.Email?.Trim()))
            {
                whereQuery += @"AND LOWER(us.Email) LIKE @Email ";
                prams.Add("Email", $"%{filterModel.Email.Trim().ToLower()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.MobileNumber?.Trim()))
            {
                whereQuery += @"AND us.MobileNumber LIKE @MobileNumber ";
                prams.Add("MobileNumber", $"%{filterModel.MobileNumber.Trim()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.NationalCode?.Trim()))
            {
                whereQuery += @"AND us.NationalCode LIKE @NationalCode ";
                prams.Add("NationalCode", $"%{filterModel.NationalCode.Trim()}%");
            }

            if (!string.IsNullOrEmpty(filterModel.RegistrationIp?.Trim()))
            {
                whereQuery += @"AND us.RegistrationIp LIKE @RegistrationIp ";
                prams.Add("RegistrationIp", $"%{filterModel.RegistrationIp.Trim()}%");
            }

            if (filterModel.IsActive.HasValue)
            {
                whereQuery += @"AND us.IsActive = @IsActive ";
                prams.Add("IsActive", filterModel.IsActive.Value);
            }

            if (filterModel.IsBane.HasValue)
            {
                whereQuery += @"AND us.IsBane = @IsBane ";
                prams.Add("IsBane", filterModel.IsBane.Value);
            }

            if (filterModel.Balance.HasValue)
            {
                whereQuery += @"AND wal.LastBalance >= @Balance ";
                prams.Add("Balance", filterModel.Balance.Value);
            }

            if (filterModel.RegisterDate.HasValue)
            {
                whereQuery += @"AND DATE_TRUNC('day', us.CreateOn) = @CreateOn ";
                prams.Add("CreateOn", filterModel.RegisterDate.Value);
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

        var sqlQuery = $@"SELECT 
                                       us.Id
                                      ,us.UserName
                                      ,us.FirstName
                                      ,us.LastName
                                      ,us.DisplayName
                                      ,us.GenderType
                                      ,us.MobileNumber
                                      ,us.CountryCode
                                      ,us.CountryIso
                                      ,us.Email
                                      ,us.RegistrationIp
                                      ,us.NationalCode
                                      ,us.ProfilePictureName
                                      ,us.IsActive
                                      ,us.IsBane
                                      ,us.LastLoggedIn
                                      ,us.CreateOn AS RegisterDate
                                   ,wal.LastBalance AS LastBalance
                                  FROM {TableName} AS us
                                  INNER JOIN [Transaction].[Wallet] AS wal ON us.Id = wal.EntityId
                                  {whereQuery}
                                  ORDER BY us.Id DESC OFFSET @Skip ROWS FETCH NEXT {filterModel.PageSize} ROWS ONLY;

                                  Select COUNT(1) 
                                  FROM {TableName} AS us
                                  INNER JOIN [Transaction].[Wallet] AS wal ON us.Id = wal.EntityId
                                  {whereQuery}";


        #endregion

        #region Get Data From Db
        using var multiData = await db.QueryMultipleAsync(sqlQuery, prams);
        //using var multiData =  db.QueryMultiple(sqlQuery, prams);

        var data = (await multiData.ReadAsync<UserGetListViewModel>()).ToList();
        var totalCount = (await multiData.ReadAsync<int>()).FirstOrDefault();

        return (data, totalCount);
        #endregion
    }

    public async Task<User?> GetById(long id)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE Id=@id ", new { id });
        return result.SingleOrDefault();
    }

    public async Task<List<User>> GetByIds(long[] ids)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE Id = Any (@ids)", new { ids });
        return result.ToList();
    }

    public async Task<User?> GetByMobileNumber(string mobileNumber)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE MobileNumber = @mobileNumber", new { mobileNumber });
        return result.SingleOrDefault();
    }

    public async Task<User?> GetByEmail(string email)
    {
        using var db = new DbEntityObject().GetConnectionString();
        email = email.ToLower();
        var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE lower(Email) = lower(@email)", new { email });
        return result.SingleOrDefault();
    }

    public async Task<User?> GetByUniqCode(string uniqCode)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE UniqCode = @uniqCode", new { uniqCode });
        return result.FirstOrDefault();
    }

    public async Task<User?> GetByUserName(string userName)
    {
        using var db = new DbEntityObject().GetConnectionString();
        userName = userName.ToLower();
        var query = $@"Select * From {TableName} WHERE lower(UserName) = lower(@userName)";
        var result = await db.QueryAsync<User>(query, new { userName = userName.Trim() });
        return result.SingleOrDefault();
    }

    public async Task<User?> GetByNationalCode(string nationalCode)
    {
        using var db = new DbEntityObject().GetConnectionString();
        nationalCode = nationalCode.ToLower();
        var result = await db.QueryAsync<User>($@"Select * From {TableName} WHERE NationalCode = @nationalCode", new { nationalCode });
        return result.SingleOrDefault();
    }

    public async Task<int> GetRegistrationIpCount(string registrationIp)
    {
        using var db = new DbEntityObject().GetConnectionString();

        registrationIp = registrationIp.ToLower();
        var result = await db.QueryAsync<int>($@"Select Count(1) 
                                                            From {TableName} 
                                                            WHERE RegistrationIp = @registrationIp",
            new { registrationIp });

        return result.SingleOrDefault();
    }

    public async Task<User?> GetByForgetPasswordToken(string forgetPasswordToken)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"Select * From {TableName} WHERE ForgetPasswordToken = @forgetPasswordToken";
        var result = await db.QueryAsync<User>(query, new { forgetPasswordToken });
        return result.SingleOrDefault();
    }

    public async Task<User?> GetByProfilePictureName(string fileName)
    {
        using var db = new DbEntityObject().GetConnectionString();
        var query = $@"Select * From {TableName} WHERE ProfilePictureName = @fileName";
        var result = await db.QueryAsync<User>(query, new { fileName });
        return result.SingleOrDefault();
    }

    public async Task<User?> GetByUserNameAndPassword(string userName, string password)
    {
        using var db = new DbEntityObject().GetConnectionString();
        userName = userName.ToLower();
        var result = await db.QueryAsync<User>(
            $@"Select * From {TableName} WHERE lower(UserName) = lower(@userName) AND Password = @password",
            new { userName = userName.Trim(), password });
        return result.SingleOrDefault();
    }

    public async Task<User?> GetByEmailAndPassword(string email, string password)
    {
        using var db = new DbEntityObject().GetConnectionString();
        email = email.Trim().ToLower();
        var result = await db.QueryAsync<User>(
            $@"Select * From {TableName} WHERE lower(Email) = lower(@email) AND Password = @password",
            new { email, password });
        return result.SingleOrDefault();
    }

    public async Task<List<string>> GetUserEmails(EmailSendTypeEnum sendType, string? filterValue)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var emails = new List<string>();

        switch (sendType)
        {
            #region All
            case EmailSendTypeEnum.All:
                emails = (await db.QueryAsync<string>($@"Select Email From {TableName} WHERE IsActive = 'True' AND IsBane = 'False'")).ToList();
                break;
            #endregion

            #region LastLoginDateLower
            case EmailSendTypeEnum.LastLoginDateLower:
                if (DateTime.TryParse(filterValue, out var filterDate))
                {
                    emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND
                                                                               DATE_TRUNC('day', LastLoggedIn) <= @filterDate",
                        new { filterDate.Date })).ToList();
                }
                break;
            #endregion

            #region LastLoginDateMoreThan
            case EmailSendTypeEnum.LastLoginDateMoreThan:
                if (DateTime.TryParse(filterValue, out var filterLMtDate))
                {
                    emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND
                                                                               DATE_TRUNC('day', LastLoggedIn) >= @filterLMtDate",
                        new { filterLMtDate.Date })).ToList();
                }
                break;
            #endregion

            #region RegisterDateMoreThan
            case EmailSendTypeEnum.RegisterDateMoreThan:
                if (DateTime.TryParse(filterValue, out var filterRegDate))
                {
                    emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND
                                                                               DATE_TRUNC('day', CreateOn) >= @filterRegDate",
                        new { filterRegDate.Date })).ToList();
                }
                break;
            #endregion

            #region RegisterDateLower
            case EmailSendTypeEnum.RegisterDateLower:
                if (DateTime.TryParse(filterValue, out var filterRegLoDate))
                {
                    emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND
                                                                               DATE_TRUNC('day', CreateOn) <= @filterRegLoDate",
                        new { filterRegLoDate.Date })).ToList();
                }
                break;
            #endregion

            #region Withdraw
            case EmailSendTypeEnum.Withdraw:
                if (bool.TryParse(filterValue, out var filterWithdraw))
                {
                    if (filterWithdraw)
                    {
                        emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND 
                                                                               Id In (Select UserId FROM Transaction.Withdrawal WHERE StatusType IN (2, 5)) ")).ToList();
                    }
                    else
                    {
                        emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND 
                                                                               Id NOT In (Select UserId FROM Transaction.Withdrawal WHERE StatusType IN (2, 5)) ")).ToList();
                    }
                }
                break;
            #endregion

            #region EmailVerified
            case EmailSendTypeEnum.EmailVerified:
                if (bool.TryParse(filterValue, out var filterEmailVerify))
                {
                    emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND 
                                                                               EmailVerified = @filterEmailVerify",
                        new { filterEmailVerify })).ToList();
                }
                break;
            #endregion

            #region MobileVerified
            case EmailSendTypeEnum.MobileVerified:
                if (bool.TryParse(filterValue, out var filterMobileVerify))
                {
                    emails = (await db.QueryAsync<string>($@"Select Email From {TableName} 
                                                                         WHERE IsActive = true AND IsBane = false AND
                                                                               MobileVerified = @filterMobileVerify",
                        new { filterMobileVerify })).ToList();
                }
                break;
            #endregion

            #region LasBalanceLower
            case EmailSendTypeEnum.LasBalanceLower:
                if (long.TryParse(filterValue, out var filterLasBalanceLower))
                {
                    emails = (await db.QueryAsync<string>($@"SELECT Us.Email
                                                                         FROM Security.User AS Us
                                                                         INNER JOIN Transaction.Wallet AS wal ON wal.EntityId = Us.Id 
                                                                         WHERE Us.IsActive = true AND Us.IsBane = false AND
                                                                               wal.EntityType = 1 AND 
                                                                               wal.WalletType = 1 AND 
                                                                               wal.LastBalance <= @filterLasBalanceLower",
                        new { filterLasBalanceLower })).ToList();
                }
                break;
            #endregion

            #region LastBalanceMoreThan
            case EmailSendTypeEnum.LastBalanceMoreThan:
                if (long.TryParse(filterValue, out var filterLastBalanceMoreThan))
                {
                    emails = (await db.QueryAsync<string>($@"SELECT Us.Email
                                                                         FROM Security.User AS Us
                                                                         INNER JOIN Transaction.Wallet AS wal ON wal.EntityId = Us.Id 
                                                                         WHERE Us.IsActive = true AND Us.IsBane = false AND
                                                                               wal.EntityType = 1 AND 
                                                                               wal.WalletType = 1 AND 
                                                                               wal.LastBalance >= @filterLastBalanceMoreThan",
                        new { filterLastBalanceMoreThan })).ToList();
                }
                break;
            #endregion
        }

        return emails;
    }


    #endregion

    #region Insert
    public async Task<long> Insert(User entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var prams = new DynamicParameters();
        prams.Add("@ParentId", entity.ParentId);
        prams.Add("@UserName", entity.UserName);
        prams.Add("@Password", entity.Password);
        prams.Add("@FirstName", entity.FirstName);
        prams.Add("@LastName", entity.LastName);
        prams.Add("@DisplayName", entity.DisplayName);
        prams.Add("@GenderType", entity.GenderType);
        prams.Add("@MobileNumber", entity.MobileNumber);
        prams.Add("@CountryCode", entity.CountryCode);
        prams.Add("@CountryIso", entity.CountryIso);
        prams.Add("@UniqCode", entity.UniqCode);
        prams.Add("@Email", entity.Email);
        prams.Add("@NationalCode", entity.NationalCode);
        prams.Add("@BirthDay", entity.BirthDay);
        prams.Add("@IsActive", entity.IsActive);
        prams.Add("@ProfilePictureId", entity.ProfilePictureId);
        prams.Add("@ProfilePictureName", entity.ProfilePictureName);
        prams.Add("@IsPanelUser", entity.IsPanelUser);
        prams.Add("@IsBane", entity.IsBane);
        prams.Add("@ChatStatus", entity.ChatStatus);
        prams.Add("@ForgetPasswordToken", entity.ForgetPasswordToken);
        prams.Add("@ForgetPasswordTokenExpiration", entity.ForgetPasswordTokenExpiration);
        prams.Add("@RegistrationIp", entity.RegistrationIp);
        prams.Add("@Comment", entity.Comment);
        prams.Add("@LastLoggedIn", entity.LastLoggedIn);
        prams.Add("@CreatorId", entity.CreatorId);
        prams.Add("@CreateOn", entity.CreateOn);
        prams.Add("@EmailVerified", entity.EmailVerified);
        prams.Add("@EmailVerifyCode", entity.EmailVerifyCode);
        prams.Add("@EmailVerifiedDate", entity.EmailVerifiedDate);
        prams.Add("@MobileVerified", entity.MobileVerified);
        prams.Add("@MobileVerifyCode", entity.MobileVerifyCode);
        prams.Add("@MobileVerifiedDate", entity.MobileVerifiedDate);

        var entityId = (await db.QueryAsync<long>(
            $@"INSERT INTO {TableName} 
                               (
                                       ParentId
                                      ,UserName
                                      ,Password
                                      ,FirstName
                                      ,LastName
                                      ,DisplayName
                                      ,GenderType
                                      ,MobileNumber                         
                                      ,CountryCode                         
                                      ,CountryIso                                                                      
                                      ,UniqCode
                                      ,Email
                                      ,NationalCode
                                      ,BirthDay
                                      ,IsActive
                                      ,ProfilePictureId
                                      ,ProfilePictureName                                
                                      ,IsPanelUser
                                      ,IsBane
                                      ,ChatStatus
                                      ,ForgetPasswordToken
                                      ,ForgetPasswordTokenExpiration
                                      ,RegistrationIp
                                      ,Comment
                                      ,LastLoggedIn
                                      ,CreatorId
                                      ,CreateOn
                                      ,EmailVerified
                                      ,EmailVerifyCode
                                      ,EmailVerifiedDate
                                      ,MobileVerified
                                      ,MobileVerifyCode
                                      ,MobileVerifiedDate
                               )
                               VALUES
                               (
                                       @ParentId
                                      ,@UserName
                                      ,@Password
                                      ,@FirstName
                                      ,@LastName
                                      ,@DisplayName
                                      ,@GenderType
                                      ,@MobileNumber                                 
                                      ,@CountryCode                                 
                                      ,@CountryIso                                                
                                      ,@UniqCode
                                      ,@Email
                                      ,@NationalCode                                  
                                      ,@BirthDay
                                      ,@IsActive
                                      ,@ProfilePictureId
                                      ,@ProfilePictureName
                                      ,@IsPanelUser
                                      ,@IsBane
                                      ,@ChatStatus
                                      ,@ForgetPasswordToken
                                      ,@ForgetPasswordTokenExpiration
                                      ,@RegistrationIp
                                      ,@Comment
                                      ,@LastLoggedIn
                                      ,@CreatorId
                                      ,@CreateOn
                                      ,@EmailVerified
                                      ,@EmailVerifyCode
                                      ,@EmailVerifiedDate
                                      ,@MobileVerified
                                      ,@MobileVerifyCode
                                      ,@MobileVerifiedDate
                               );
                               SELECT CAST(SCOPE_IDENTITY() as BIGINT);", prams)).SingleOrDefault();

        return entityId;
    }
    #endregion

    #region Update
    public async Task<int> Update(User entity)
    {
        using var db = new DbEntityObject().GetConnectionString();

        var sqlQuery = $@"UPDATE {TableName} 
                                   SET 
                                        ParentId = @ParentId
                                       ,UserName = @UserName
                                       ,Password = @Password
                                       ,FirstName = @FirstName
                                       ,LastName = @LastName
                                       ,DisplayName = @DisplayName
                                       ,GenderType = @GenderType
                                       ,MobileNumber = @MobileNumber
                                       ,CountryCode = @CountryCode
                                       ,CountryIso = @CountryIso
                                       ,UniqCode = @UniqCode
                                       ,Email = @Email
                                       ,NationalCode = @NationalCode
                                       ,BirthDay = @BirthDay
                                       ,IsActive = @IsActive
                                       ,ProfilePictureId = @ProfilePictureId
                                       ,ProfilePictureName = @ProfilePictureName
                                       ,IsPanelUser = @IsPanelUser
                                       ,IsBane = @IsBane
                                       ,ChatStatus = @ChatStatus
                                       ,ForgetPasswordToken = @ForgetPasswordToken
                                       ,ForgetPasswordTokenExpiration = @ForgetPasswordTokenExpiration
                                       ,Comment = @Comment
                                       ,RegistrationIp = @RegistrationIp
                                       ,LastLoggedIn = @LastLoggedIn
                                       ,CreatorId = @CreatorId
                                       ,CreateOn = @CreateOn
                                       ,EmailVerified = @EmailVerified
                                       ,EmailVerifyCode = @EmailVerifyCode
                                       ,EmailVerifiedDate = @EmailVerifiedDate
                                       ,MobileVerified = @MobileVerified
                                       ,MobileVerifyCode = @MobileVerifyCode
                                       ,MobileVerifiedDate = @MobileVerifiedDate
                                   WHERE Id = @Id";

        var rowsAffected = await db.ExecuteAsync(sqlQuery, new
        {
            entity.ParentId,
            entity.UserName,
            entity.Password,
            entity.FirstName,
            entity.LastName,
            entity.DisplayName,
            entity.GenderType,
            entity.MobileNumber,
            entity.CountryCode,
            entity.CountryIso,
            entity.UniqCode,
            entity.Email,
            entity.NationalCode,
            entity.BirthDay,
            entity.IsActive,
            entity.ProfilePictureId,
            entity.ProfilePictureName,
            entity.IsPanelUser,
            entity.IsBane,
            entity.ChatStatus,
            entity.ForgetPasswordToken,
            entity.ForgetPasswordTokenExpiration,
            entity.RegistrationIp,
            entity.Comment,
            entity.LastLoggedIn,
            entity.CreatorId,
            entity.CreateOn,
            entity.EmailVerified,
            entity.EmailVerifyCode,
            entity.EmailVerifiedDate,
            entity.MobileVerified,
            entity.MobileVerifyCode,
            entity.MobileVerifiedDate,
            entity.Id
        });

        return rowsAffected;
    }
    #endregion
}