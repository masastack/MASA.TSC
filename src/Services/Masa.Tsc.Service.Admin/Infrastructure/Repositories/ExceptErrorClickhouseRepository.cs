// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using ClickHouse.Ado;

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories;

internal class ExceptErrorClickhouseRepository
{
    private readonly MasaStackClickhouseConnection _clickhouseConnection;
    private readonly ILogger _logger;
    private const string TableName = "tsc_except_errors";

    public ExceptErrorClickhouseRepository(MasaStackClickhouseConnection clickhouseConnection, ILogger<ExceptErrorClickhouseRepository> logger)
    {
        _clickhouseConnection = clickhouseConnection;
        if (_clickhouseConnection.State == ConnectionState.Closed)
            _clickhouseConnection.Open();
        _logger = logger;
    }

    public async Task AddAsync(params ExceptError[] entities)
    {
        var sql = new StringBuilder($"insert into {TableName}(Id,Environment,Project,Service,Type,Message,Comment,Creator,Modifier,CreationTime,ModificationTime,IsDeleted) values");
        var index = 1;
        var parameters = new List<ClickHouseParameter>();
        foreach (var entity in entities)
        {
            sql.AppendLine(InsertSql(index));
            parameters.AddRange(CreateParamaters(index++, entity));
        }
        sql.Remove(sql.Length - 1, 1);
        _ = Execute<int>(_clickhouseConnection, _logger, cmd => cmd.ExecuteNonQuery(), sql.ToString(), parameters);
        await Task.CompletedTask;
    }

    private static string InsertSql(int index)
    {
        return $"(@Id_{index},@Environment_{index},@Project_{index},@Service_{index},@Type_{index},@Message_{index},@Comment_{index},@Creator_{index},@Modifier_{index},@CreationTime_{index},@ModificationTime_{index},@IsDeleted_{index}),";
    }

    private static IEnumerable<ClickHouseParameter> CreateParamaters(int index, ExceptError entity)
    {
        return new[] {
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Id)}_{index}",Value=entity.Id },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Environment)}_{index}",Value=entity.Environment },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Project)}_{index}",Value=entity.Project },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Service)}_{index}",Value=entity.Service },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Type)}_{index}",Value=entity.Type },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Message)}_{index}",Value=entity.Message },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Comment)}_{index}",Value=entity.Comment },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Creator)}_{index}",Value=entity.Creator },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.Modifier)}_{index}",Value=entity.Modifier },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.CreationTime)}_{index}",Value=entity.CreationTime,DbType= DbType.DateTime2 },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.ModificationTime)}_{index}",Value=entity.ModificationTime,DbType= DbType.DateTime2 },
            new ClickHouseParameter(){ ParameterName=$"{nameof(ExceptError.IsDeleted)}_{index}",Value=entity.IsDeleted,DbType= DbType.Boolean}
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
