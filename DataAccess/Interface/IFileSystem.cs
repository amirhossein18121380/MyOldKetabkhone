using DataModel.Common;

namespace DataAccess.DAL.Common;

public interface IFileSystem
{
    Task<bool> Delete(long id);
    Task<FileSystem?> GetByFileName(string fileName);
    Task<FileSystem?> GetById(long id);
    Task<FileSystem?> GetByThumbnailFileName(string thumbnailFileName);
    Task<long> Insert(FileSystem entity);
    Task<long> Update(FileSystem entity);
}