// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

internal class ExceptErrorService : IExceptErrorService
{
    private readonly MasaStackClickhouseConnection _connection;
    readonly ILogger _logger;
    public ExceptErrorService(MasaStackClickhouseConnection dbConnection, ILogger<ExceptErrorService> logger)
    {
        _connection = dbConnection;
        _logger = logger;
        if (_connection.State == ConnectionState.Closed)
            _connection.Open();
        else if (_connection.State == ConnectionState.Broken)
        {
            _connection.Close();
            _connection.Open();
        }

    }

    public async Task AddAsync(params ExceptErrorDto[] values)
    {
        var sql = new StringBuilder($"insert into {Constants.ExceptErrorTable}(Id,Environment,Project,Service,Type,Message,Comment,Creator,Modifier,CreationTime,ModificationTime,IsDeleted) values");
        var index = 1;
        var parameters = new List<ClickHouseParameter>();
        foreach (var entity in values)
        {
            sql.AppendLine(InsertSql(index));
            parameters.AddRange(CreateParamaters(index++, entity));
        }
        sql.Remove(sql.Length - 1, 1);
        _ = Execute<int>(_connection, _logger, cmd => cmd.ExecuteNonQuery(), sql.ToString(), parameters);
        await Task.CompletedTask;
    }

    private static string InsertSql(int index)
    {
        return $"(@Id_{index},@Environment_{index},@Project_{index},@Service_{index},@Type_{index},@Message_{index},@Comment_{index},@Creator_{index},@Modifier_{index},@CreationTime_{index},@ModificationTime_{index},@IsDeleted_{index}),";
    }

    private static IEnumerable<ClickHouseParameter> CreateParamaters(int index, ExceptErrorDto entity)
    {
        return new[] {
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Id)}_{index}",Value=entity.Id },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Environment)}_{index}",Value=entity.Environment },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Project)}_{index}",Value=entity.Project },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Service)}_{index}",Value=entity.Service },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Type)}_{index}",Value=entity.Type },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Message)}_{index}",Value=entity.Message },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Comment)}_{index}",Value=entity.Comment },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Creator)}_{index}",Value=entity.Creator },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.Modifier)}_{index}",Value=entity.Modifier },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.CreationTime)}_{index}",Value=entity.CreationTime,DbType= DbType.DateTime2 },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.ModificationTime)}_{index}",Value=entity.ModificationTime,DbType= DbType.DateTime2 },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptErrorDto.IsDeleted)}_{index}",Value=entity.IsDeleted,DbType= DbType.Boolean}
        };
    }

    private static TResult Execute<TResult>(IDbConnection connect, ILogger logger, Func<IDbCommand, TResult> func, string sql, List<ClickHouseParameter>? @parameters = null)
    {
        var start = DateTime.Now;
        using var cmd = connect.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Count > 0)
            foreach (var p in @parameters)
                cmd.Parameters.Add(p);
        try
        {
            return func.Invoke(cmd);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "execute sql error:{RawSql}, paramters:{Parameters}", sql, parameters);
            throw;
        }
        finally
        {
            var end = DateTime.Now;
            var duration = (end - start).TotalSeconds;
            if (duration - 1 > 0)
                logger.LogWarning("execute query slow {Duration}s, rawSql:{Rawsql}, parameters:{Paramters}", duration, sql, parameters);
        }
    }
}
