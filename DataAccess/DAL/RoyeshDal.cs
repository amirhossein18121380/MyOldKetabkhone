using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Tool;
using DataModel.Models;

namespace DataAccess.DAL;

public class RoyeshDal
{
    private readonly Entity _context;
    #region DataMember
    private const string TbName = "[dbo].[Royesh]";
    #endregion

    public async Task<Royesh> GetLeaves(long userid)
    {
        var db = _context.CreateConnection();

        try
        {
            var query = $@"select * from {TbName} where Royesh.UserId = @Id";
            var result = await db.QuerySingleOrDefaultAsync<Royesh>(query, new {userid});
            return result;
        }
        finally
        {
            db.Close();
        }
    }

    public async Task<bool> AddLeaves(Royesh royesh)
    {
        var db = _context.CreateConnection();

        try
        {
            var quer = "";

            var result = await db.ExecuteAsync(quer);
            return result > 0;
        }
        finally 
        {
            db.Close();
        }
    }

}
