using Microsoft.Data.Sqlite;

public class Database
{
    private readonly string _connectionString;

    public Database(string databaseFile = "/home/jeovana/desafioBanco.db")
    {
        _connectionString = $"Data Source={databaseFile}";
    }

    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
