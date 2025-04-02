using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace SurveyApp.Infrastructure.Data
{
    public static class AppDbContextExtensions
    {
        public static void EnsureCreated(this AppDbContext context, ILogger logger)
        {
            try
            {
                // Verificar si la base de datos existe
                if (context.Database.EnsureCreated())
                {
                    logger.LogInformation("Database created successfully");
                }
                else
                {
                    logger.LogInformation("Database already exists");
                }
                
                // Ejecutar scripts SQL personalizados
                ExecuteSqlScripts(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating/updating the database");
                throw;
            }
        }
        
        private static void ExecuteSqlScripts(AppDbContext context, ILogger logger)
        {
            try
            {
                // Obtener la ruta de los scripts SQL
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var scriptsDirectory = Path.Combine(assemblyPath, "Data", "Migrations");
                
                // Si el directorio existe, ejecutar los scripts
                if (Directory.Exists(scriptsDirectory))
                {
                    foreach (var scriptFile in Directory.GetFiles(scriptsDirectory, "*.sql"))
                    {
                        try
                        {
                            logger.LogInformation($"Executing SQL script: {Path.GetFileName(scriptFile)}");
                            
                            var sql = File.ReadAllText(scriptFile);
                            context.Database.ExecuteSqlRaw(sql);
                            
                            logger.LogInformation($"SQL script executed successfully: {Path.GetFileName(scriptFile)}");
                        }
                        catch (Exception scriptEx)
                        {
                            // Loggear el error pero continuar con los siguientes scripts
                            logger.LogError(scriptEx, $"Error executing SQL script {Path.GetFileName(scriptFile)}: {scriptEx.Message}");
                        }
                    }
                }
                else
                {
                    logger.LogWarning($"Migrations directory not found: {scriptsDirectory}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading or executing SQL scripts");
            }
        }
    }
}
