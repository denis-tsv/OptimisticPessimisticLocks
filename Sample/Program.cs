using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sample;
using Sample.Model;

var connectionString = "Host=localhost;Port=7432;Database=locks;Username=postgres;Password=postgres;Timeout=180;Command Timeout=180;Include Error Detail=true;";

//CreateDatabase();

// RaceCondition();
// FixRaceCondition_RepeatableRead();
// NoRaceCondition_ReadCommited();
// NoRaceCondition_RepeatableRead();

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

void CreateDatabase()
{
    var options = new DbContextOptionsBuilder<LocksDbContext>()
        .UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention()
        .Options;
    var context = new LocksDbContext(options);
    var created = context.Database.EnsureCreated();
    if (created)
    {
        var order = new Order {Name = "Order"};
        context.Orders.Add(order);
        var item1 = new OrderItem
        {
            Price = 100,
            Quantity = 10,
            Order = order
        };
        var item2 = new OrderItem
        {
            Price = 200,
            Quantity = 20,
            Order = order
        };
        context.OrderItems.Add(item1);
        context.OrderItems.Add(item2);
        context.SaveChanges();
    }
}