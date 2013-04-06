using System.Collections.Generic;
using System.Data;

namespace Nox
{
    public interface INoxProvider
    {
        IDbConnection CreateConnection();
        IDbCommand CreateCommand(string query, IDbConnection dbConnection);
        IEnumerable<IDataParameter> CreateParameters(object parameters);
    }
}