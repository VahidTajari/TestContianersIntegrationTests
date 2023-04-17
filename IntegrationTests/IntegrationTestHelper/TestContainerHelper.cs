using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace IntegrationTests.IntegrationTestHelper;

public static class TestContainerHelper
{
    private static bool _isDbSetup;
    private static bool _isSeeded;
    private static readonly object Obj = new object();
    private static readonly object ObjSeed = new object();
    private static MsSqlContainer _dbContainer = DbSetup();
    public static MsSqlContainer DbContainer => _dbContainer;


    public static MsSqlContainer DbSetup(string password = "P@ssw0rd", string imageName = "mcr.microsoft.com/mssql/server:2022-latest")
    {
        
        if (_isDbSetup)
            return _dbContainer;

        lock (Obj)
        {
            if (!_isDbSetup)
            {
                _isDbSetup = true; 
                _dbContainer
                    = new MsSqlBuilder()
                    .WithImage(imageName)
                    .WithCleanUp(true)
                    .Build();
                _isDbSetup = true;
            }
            return _dbContainer;
        }
    }



#pragma warning  disable CA2100
    public static void SeedDatabase(string scriptFileName = "script.sql")
    {
        if (_isSeeded)
            return;

        lock (ObjSeed)
        {
            if (!_isSeeded)
            {
                using var connection = new SqlConnection(_dbContainer.GetConnectionString() + ";Encrypt=false");

                var script = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), scriptFileName) ?? string.Empty);

                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30));

                connection.Open();

                foreach (var commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString.Trim()))
                    {
                        using var command = new SqlCommand(commandString, connection);

                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
                _isSeeded = true;
            }
        }

    }

    public static string GetConnectionString => _dbContainer.GetConnectionString() + ";Encrypt=false";
}
