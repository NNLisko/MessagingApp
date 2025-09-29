using Npgsql;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

// Npgsql to manage a fixed amount of connections
// ConnectionPool is a singleton with a private builder

namespace MessageLiskoServer.Database;

class ConnectionPool
{

    private Queue<NpgsqlConnection> connections = new ();
    private static ConnectionPool instance;

    private int _poolSize;


    private ConnectionPool(string constr, int poolSize)
    {
        _poolSize = poolSize;
        connections = new Queue<NpgsqlConnection>();

        for (int i = 0; i < poolSize; i++)
        {
            await using var dataSource = NpgsqlDataSource.Create(constr);
        }
    }

    public static ConnectionPool makeConnPool(string constr, int poolSize)
    {
        if (instance == null)
        {
            instance = new ConnectionPool(constr, int poolSize);
        }
        return instance;
    

    

    

    //await using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
    //{
    //    cmd.Parameters.AddWithValue("p", "Hello world");
    //    await cmd.ExecuteNonQueryAsync();
    //}

    //await using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
    //await using (var reader = await cmd.ExecuteReaderAsync())
    //{
    //    while (await reader.ReadAsync())
    //        Console.WriteLine(reader.GetString(0));
    //}
}