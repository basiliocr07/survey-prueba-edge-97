
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;

namespace SurveyApp.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await QueryCustomersWithServices(db, "");
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByTypeAsync(string customerType)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await QueryCustomersWithServices(db, "WHERE c.CustomerType = @CustomerType", new { CustomerType = customerType });
            }
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var customers = await QueryCustomersWithServices(db, "WHERE c.Id = @Id", new { Id = id });
                return customers.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Service>("SELECT * FROM Services WHERE IsActive = 1");
            }
        }

        public async Task<int> AddCustomerAsync(Customer customer)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                const string sql = @"
                    INSERT INTO Customers (BrandName, ContactName, ContactEmail, ContactPhone, CustomerType, CreatedAt, UpdatedAt)
                    VALUES (@BrandName, @ContactName, @ContactEmail, @ContactPhone, @CustomerType, @CreatedAt, @UpdatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                customer.CreatedAt = DateTime.UtcNow;
                customer.UpdatedAt = DateTime.UtcNow;

                return await db.QuerySingleAsync<int>(sql, customer);
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                const string sql = @"
                    UPDATE Customers 
                    SET BrandName = @BrandName, 
                        ContactName = @ContactName, 
                        ContactEmail = @ContactEmail, 
                        ContactPhone = @ContactPhone, 
                        CustomerType = @CustomerType, 
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";

                customer.UpdatedAt = DateTime.UtcNow;

                await db.ExecuteAsync(sql, customer);
            }
        }

        public async Task DeleteCustomerAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Primero eliminamos las relaciones con servicios
                await db.ExecuteAsync("DELETE FROM CustomerServices WHERE CustomerId = @Id", new { Id = id });
                
                // Luego eliminamos el cliente
                await db.ExecuteAsync("DELETE FROM Customers WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task AddCustomerServiceAsync(int customerId, string serviceId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                const string sql = @"
                    INSERT INTO CustomerServices (CustomerId, ServiceId)
                    VALUES (@CustomerId, @ServiceId)";

                await db.ExecuteAsync(sql, new { CustomerId = customerId, ServiceId = serviceId });
            }
        }

        public async Task<string> GetServiceIdByNameAsync(string serviceName)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QuerySingleOrDefaultAsync<string>(
                    "SELECT Id FROM Services WHERE Name = @Name", 
                    new { Name = serviceName });
            }
        }

        public async Task<IEnumerable<string>> GetCustomerEmailsAsync(string customerType = null)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT ContactEmail FROM Customers";
                if (!string.IsNullOrEmpty(customerType))
                {
                    sql += " WHERE CustomerType = @CustomerType";
                    return await db.QueryAsync<string>(sql, new { CustomerType = customerType });
                }
                return await db.QueryAsync<string>(sql);
            }
        }

        // Método helper para consultar clientes con sus servicios
        private async Task<IEnumerable<Customer>> QueryCustomersWithServices(IDbConnection db, string whereClause, object param = null)
        {
            string sql = $@"
                SELECT c.Id, c.BrandName, c.ContactName, c.ContactEmail, c.ContactPhone, 
                       c.CreatedAt, c.UpdatedAt, c.CustomerType,
                       s.Name as ServiceName
                FROM Customers c
                LEFT JOIN CustomerServices cs ON c.Id = cs.CustomerId
                LEFT JOIN Services s ON cs.ServiceId = s.Id
                {whereClause}
                ORDER BY c.CreatedAt DESC";

            var customerDict = new Dictionary<int, Customer>();

            await db.QueryAsync<Customer, string, Customer>(
                sql,
                (customer, serviceName) => {
                    if (!customerDict.TryGetValue(customer.Id, out var customerEntry))
                    {
                        customerEntry = customer;
                        customerEntry.AcquiredServices = new List<string>();
                        customerDict.Add(customer.Id, customerEntry);
                    }

                    if (!string.IsNullOrEmpty(serviceName) && 
                        !customerEntry.AcquiredServices.Contains(serviceName))
                    {
                        customerEntry.AcquiredServices.Add(serviceName);
                    }

                    return customerEntry;
                },
                param,
                splitOn: "ServiceName");

            return customerDict.Values;
        }
    }
}
