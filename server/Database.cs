namespace server;

using Npgsql;

public class Database
{
  private readonly string _host = "217.76.56.135";
  private readonly string _port = "5437";
  private readonly string _username = "postgres";
  private readonly string _password = "GreedyMotherGrows";
  private readonly string _database = "crm_db";


  private readonly NpgsqlDataSource _connection;

  public NpgsqlDataSource Connection()
  {
    return _connection;
  }

  public Database()
  {
    string connectionString = $"Host={_host};Port={_port};Username={_username};Password={_password};Database={_database}";
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    dataSourceBuilder.MapEnum<Role>();
    _connection = dataSourceBuilder.Build();
  }
}