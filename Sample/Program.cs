using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SampleApi;
using SampleApi.Entities;

var connectionString = "Host=localhost;Port=7432;Database=locks;Username=postgres;Password=postgres;Timeout=180;Command Timeout=180;Include Error Detail=true;";

//CreateDatabase();

// RaceCondition();
// FixRaceCondition_RepeatableRead();
// NoRaceCondition_ReadCommited();
// NoRaceCondition_RepeatableRead();
//Timestamp_DbUpdateConcurrencyException();

//DbUpdateConcurrencyException: The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
void Timestamp_DbUpdateConcurrencyException()
{
    var dbContext1 = CreateDbContext();
    var order1 = dbContext1.Orders.First(x => x.Id == 1);
    
    var dbContext2 = CreateDbContext();
    var order2 = dbContext2.Orders.First(x => x.Id == 1);

    order1.Name = "Version1";
    dbContext1.SaveChanges();

    order2.Name = "Version2";
    dbContext2.SaveChanges();
}

void RaceCondition()
{
    var connection1 = CreateConnection();
    var transaction1 = connection1.BeginTransaction(IsolationLevel.ReadCommitted);
    var order1 = connection1.QueryFirst<Order>("SELECT id, name FROM orders");


    var connection2 = CreateConnection();
    var transaction2 = connection2.BeginTransaction(IsolationLevel.ReadCommitted);
    var order2 = connection2.QueryFirst<Order>("SELECT id, name FROM orders");

    connection1.Execute("UPDATE orders SET name='Order 1' WHERE id=1");
    transaction1.Commit();

    connection2.Execute("UPDATE orders SET name='Order 2' WHERE id=1");
    transaction2.Commit();
}

void FixRaceCondition_RepeatableRead()
{
    var connection1 = CreateConnection();
    var transaction1 = connection1.BeginTransaction(IsolationLevel.RepeatableRead);
    var order1 = connection1.QueryFirst<Order>("SELECT id, name FROM orders");

    var connection2 = CreateConnection();
    var transaction2 = connection2.BeginTransaction(IsolationLevel.RepeatableRead);
    var order2 = connection2.QueryFirst<Order>("SELECT id, name FROM orders");

    connection1.Execute("UPDATE orders SET name='Order 1' WHERE id=1");
    transaction1.Commit();

    connection2.Execute("UPDATE orders SET name='Order 2' WHERE id=1");
    transaction2.Commit();
}



void NoRaceCondition_ReadCommited()
{
    var connection1 = CreateConnection();
    var order1 = connection1.QueryFirst<Order>("SELECT id, name FROM orders");
    
    var connection2 = CreateConnection();
    var order2 = connection2.QueryFirst<Order>("SELECT id, name FROM orders");
    
    var transaction1 = connection1.BeginTransaction(IsolationLevel.ReadCommitted);
    connection1.Execute("UPDATE orders SET name='Order 1' WHERE id=1");
    transaction1.Commit();
    
    var transaction2 = connection2.BeginTransaction(IsolationLevel.ReadCommitted);
    connection2.Execute("UPDATE orders SET name='Order 2' WHERE id=1");
    transaction2.Commit();
}

void NoRaceCondition_RepeatableRead()
{
    var connection1 = CreateConnection();
    var order1 = connection1.QueryFirst<Order>("SELECT id, name FROM orders");
    
    var connection2 = CreateConnection();
    var order2 = connection2.QueryFirst<Order>("SELECT id, name FROM orders");
    
    var transaction1 = connection1.BeginTransaction(IsolationLevel.RepeatableRead);
    connection1.Execute("UPDATE orders SET name='Order 1' WHERE id=1");
    transaction1.Commit();
    
    var transaction2 = connection2.BeginTransaction(IsolationLevel.RepeatableRead);
    connection2.Execute("UPDATE orders SET name='Order 2' WHERE id=1");
    transaction2.Commit();
}


DbConnection CreateConnection()
{
    var result = new NpgsqlConnection(connectionString);
    result.Open();
    return result;
}


LocksDbContext CreateDbContext()
{
    var options = new DbContextOptionsBuilder<LocksDbContext>()
        .UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention()
        .Options;
    return new LocksDbContext(options);
}
