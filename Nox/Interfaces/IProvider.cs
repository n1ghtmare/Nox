﻿using System.Collections.Generic;
using System.Data;

namespace Nox.Interfaces
{
    public interface IProvider
    {
        IDbConnection CreateConnection();
        IDbCommand CreateCommand(string query, IDbConnection connection, CommandType commandType);
        IEnumerable<IDataParameter> CreateParameters(IDictionary<string, object> parameters);
    }
}