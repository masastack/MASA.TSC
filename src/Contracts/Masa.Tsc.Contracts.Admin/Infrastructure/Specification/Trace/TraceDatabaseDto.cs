// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TraceDatabaseDto
{         
    public string Kind { get; set; }

    [JsonPropertyName("db.system")]
    public string System { get; set; }

    [JsonPropertyName("db.connection_string")]
    public string ConnectionString { get; set; }

    [JsonPropertyName("db.user")]
    public string User { get; set; }

    [JsonPropertyName("net.peer.ip")]
    public string PeerIp { get; set; }

    [JsonPropertyName("net.peer.name")]
    public string PeerName { get; set; }

    [JsonPropertyName("net.peer.port")]
    public int PeerPort { get; set; }

    [JsonPropertyName("net.transport")]
    public string Transport { get; set; }

    [JsonPropertyName("db.jdbc.driver_classname")]
    public string JdbcDriverClassName { get; set; }

    [JsonPropertyName("db.mssql.instance_name")]
    public string MssqlInstanceName { get; set; }

    [JsonPropertyName("db.name")]
    public string Name { get; set; }

    [JsonPropertyName("db.statement")]
    public string Statement { get; set; }

    [JsonPropertyName("db.operation")]
    public string Operation { get; set; }

    [JsonPropertyName("db.redis.database_index")]
    public int RedisDatabaseIndex { get; set; }

    [JsonPropertyName("db.mongodb.collection")]
    public string MongodbCollection { get; set; }

    [JsonPropertyName("db.sql.table")]
    public string SqlTable { get; set; }

    #region Cassandra
    [JsonPropertyName("db.cassandra.page_size")]
    public int CassandraPageSize { get; set; }

    [JsonPropertyName("db.cassandra.consistency_level")]
    public string CassandraConsistencyLevel { get; set; }

    [JsonPropertyName("db.cassandra.table")]
    public string CassandraTable { get; set; }

    [JsonPropertyName("db.cassandra.idempotence")]
    public bool CassandraIdempotence { get; set; }

    [JsonPropertyName("db.cassandra.speculative_execution_count")]
    public bool CassandraSpeculativeExecutionCount { get; set; }

    [JsonPropertyName("db.cassandra.coordinator.id")]
    public string CassandraCoordinatorId { get; set; }

    [JsonPropertyName("db.cassandra.coordinator.dc")]
    public string CassandraCoordinatorDc { get; set; }
    #endregion
}