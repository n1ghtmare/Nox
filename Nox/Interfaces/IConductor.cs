using System.Collections.Generic;
using System.Data;

namespace Nox.Interfaces
{
    public interface IConductor
    {
        IEnumerable<T> Execute<T>(string query) where T : new();
        IEnumerable<T> Execute<T>(string query, object parameters) where T : new();
        IEnumerable<T> Execute<T>(string query, CommandType commandType) where T : new();
        IEnumerable<T> Execute<T>(string query, object parameters, CommandType commandType) where T : new();
        IEnumerable<object> Execute(string query);
        IEnumerable<object> Execute(string query, object parameters);
        IEnumerable<object> Execute(string query, CommandType commandType);
        IEnumerable<object> Execute(string query, object parameters, CommandType commandType);
        T ExecuteScalar<T>(string query);
        T ExecuteScalar<T>(string query, object parameters);
        T ExecuteScalar<T>(string query, CommandType commandType);
        T ExecuteScalar<T>(string query, object parameters, CommandType commandType);
    }
}