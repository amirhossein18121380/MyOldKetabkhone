using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace DataAccess.Tool;

public class Entity
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    string connectionId = "Default";


    public Entity(IConfiguration configuration)
    {
        _configuration = configuration;
        //connectionId = "Data Source = DESKTOP - GGN4ULV; Initial Catalog = University; Integrated Security = True;";
        _connectionString = _configuration.GetConnectionString(connectionId);
    }
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}

