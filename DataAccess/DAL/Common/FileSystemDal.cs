using Dapper;
using DataAccess.Tool;
using DataModel.Common;

namespace DataAccess.DAL.Common
{
    public class FileSystemDal : IFileSystem
    {
        #region DataMember
        private const string TableName = @"dbo.FileSystem";
        #endregion

        #region Fetch
        public async Task<FileSystem?> GetById(long id)
        {
            using var db = new DbEntityObject().GetConnectionString();
            return (await db.QueryAsync<FileSystem>($@"Select * From {TableName} WHERE Id = @Id",
                                                  new { id })).SingleOrDefault();
        }

        public async Task<FileSystem?> GetByFileName(string fileName)
        {
            using var db = new DbEntityObject().GetConnectionString();

            return (await db.QueryAsync<FileSystem>($@"Select * From {TableName} WHERE FileName = @FileName",
                                                  new { FileName = fileName })).SingleOrDefault();
        }

        public async Task<FileSystem?> GetByThumbnailFileName(string thumbnailFileName)
        {
            using var db = new DbEntityObject().GetConnectionString();

            return (await db.QueryAsync<FileSystem>($@"Select* From {TableName} WHERE ThumbnailFileName = @ThumbnailFileName",
                                                  new { ThumbnailFileName = thumbnailFileName })).SingleOrDefault();
        }
        #endregion

        #region Insert
        public async Task<long> Insert(FileSystem entity)
        {
            using var db = new DbEntityObject().GetConnectionString();

            var sqlQuery = $@"INSERT INTO {TableName} 
                                       (FileName
                                       ,FileSize
                                       ,FileType
                                       ,FileData
                                       ,ThumbnailFileName
                                       ,ThumbnailFileSize
                                       ,ThumbnailFileData
                                       ,IsCompress
                                       ,CreatorId
                                       ,CreateOn)
                                     VALUES
                                      ( @FileName
                                       ,@FileSize
                                       ,@FileType
                                       ,@FileData
                                       ,@ThumbnailFileName
                                       ,@ThumbnailFileSize
                                       ,@ThumbnailFileData
                                       ,@IsCompress
                                       ,@CreatorId
                                       ,@CreateOn);
                                    SELECT CAST(SCOPE_IDENTITY() as BIGINT)";

            var rowsAffected = (await db.QueryAsync<long>(sqlQuery, new
            {
                entity.FileName,
                entity.FileSize,
                entity.FileType,
                entity.FileData,
                entity.ThumbnailFileName,
                entity.ThumbnailFileSize,
                entity.ThumbnailFileData,
                entity.IsCompress,
                entity.CreatorId,
                entity.CreateOn
            })).SingleOrDefault();

            return rowsAffected;
        }
        #endregion

        #region Update
        public async Task<long> Update(FileSystem entity)
        {
            using var db = new DbEntityObject().GetConnectionString();

            var sqlQuery = $@"UPDATE {TableName} 
                                       SET FileName = @FileName
                                          ,FileSize = @FileSize
                                          ,FileType = @FileType
                                          ,FileData = @FileData
                                          ,ThumbnailFileName = @ThumbnailFileName
                                          ,ThumbnailFileSize = @ThumbnailFileSize
                                          ,ThumbnailFileData = @ThumbnailFileData
                                          ,IsCompress = @IsCompress
                                          ,CreatorId = @CreatorId
                                          ,CreateOn = @CreateOn
                                       WHERE Id = @Id";
            var rowsAffected = await db.ExecuteAsync(sqlQuery, new
            {
                entity.FileName,
                entity.FileSize,
                entity.FileType,
                entity.FileData,
                entity.ThumbnailFileName,
                entity.ThumbnailFileSize,
                entity.ThumbnailFileData,
                entity.IsCompress,
                entity.CreatorId,
                entity.CreateOn,
                entity.Id
            });

            return rowsAffected;
        }
        #endregion

        #region Delete
        public async Task<bool> Delete(long id)
        {
            using var db = new DbEntityObject().GetConnectionString();

            var sqlQuery = $@"DELETE FROM {TableName} WHERE Id = @Id";
            var rowsCount = await db.ExecuteAsync(sqlQuery, new { id });

            return rowsCount > 0;
        }
        #endregion
    }
}
