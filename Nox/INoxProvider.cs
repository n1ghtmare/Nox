﻿using System.Collections.Generic;
using System.Data;

namespace Nox
{
    public interface INoxProvider
    {
        IDbConnection CreateConnection();
        IDbCommand CreateCommand(string query, IDbConnection connection, bool isStoredProcedure);
        IEnumerable<IDataParameter> CreateParameters(object parameters);
    }
}