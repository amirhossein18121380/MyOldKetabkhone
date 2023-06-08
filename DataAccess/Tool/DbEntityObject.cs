using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Tool;

public class DbEntityObject
{
    public SqlConnection GetConnectionString() => new SqlConnection(ConfigurationHelper.Current.GetConnectionString("Default"));
}

public class ConfigurationHelper
{
    private static IConfiguration? _configuration;
    public static void Configure(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static IConfiguration Current => _configuration;
}
