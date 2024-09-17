using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExclusiveLockApi.Services;

public class ForUpdateInterceptor : DbCommandInterceptor
{
    public const string HintForUpdate = "Hint: FOR UPDATE";

    private static string MutateCommand(string commandText) =>
        commandText.Contains(HintForUpdate, StringComparison.InvariantCulture)
            ? commandText + Environment.NewLine + "FOR UPDATE"
            : commandText;

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        command.CommandText = MutateCommand(command.CommandText);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        command.CommandText = MutateCommand(command.CommandText);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        command.CommandText = MutateCommand(command.CommandText);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = new())
    {
        command.CommandText = MutateCommand(command.CommandText);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
        CancellationToken cancellationToken = new())
    {
        command.CommandText = MutateCommand(command.CommandText);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        command.CommandText = MutateCommand(command.CommandText);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }
}