using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NLog;

namespace testMySql
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static async Task Main(string[] args)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Database = "db",
                UserID = "root",
                Password = "admin",
                SslMode = MySqlSslMode.Required
            };
            var stopwatch = new Stopwatch();
            while (true)
                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    Logger.Info("Opening connection");
                    stopwatch.Start();
                    try
                    {
                        await conn.OpenAsync();

                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = "SELECT field FROM table limit 1;";

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var test = reader.GetInt32(0);
                                    Logger.Info(test);
                                }
                            }
                        }

                        Logger.Info("Closing connection");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    stopwatch.Stop();
                    var ts = stopwatch.Elapsed;
                    var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
                    Logger.Info(elapsedTime);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
        }
    }
}