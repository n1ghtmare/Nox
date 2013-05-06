using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using Nox.Interfaces;

namespace Nox
{
    public class Conductor : IConductor
    {
        private readonly IProvider _provider;

        public Conductor(IProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<T> Execute<T>(string query) where T : new()
        {
            return Execute<T>(query, null, (CommandType)0);
        }

        public IEnumerable<T> Execute<T>(string query, object parameters) where T : new()
        {
            return Execute<T>(query, parameters, (CommandType) 0);
        }

        public IEnumerable<T> Execute<T>(string query, CommandType commandType) where T : new()
        {
            return Execute<T>(query, null, commandType);
        }
        
        public IEnumerable<T> Execute<T>(string query, object parameters, CommandType commandType) where T : new()
        {
            using (IDbConnection connection = _provider.CreateConnection())
            using (IDbCommand command = _provider.CreateCommand(query, connection, commandType))
            {
                AppendParameters(command, parameters);
                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        if (typeof(T) == typeof(Object))
                            yield return (T)ComposeExpandoObject(reader);
                        else
                            yield return ComposeType<T>(reader);
                    }
                }
            }
        }

        public IEnumerable<dynamic> Execute(string query)
        {
            return Execute(query, null, (CommandType) 0);
        }

        public IEnumerable<dynamic> Execute(string query, object parameters)
        {
            return Execute(query, parameters, (CommandType) 0);
        }

        public IEnumerable<dynamic> Execute(string query, CommandType commandType)
        {
            return Execute(query, null, commandType);
        }

        public IEnumerable<dynamic> Execute(string query, object parameters, CommandType commandType)
        {
            return Execute<dynamic>(query, parameters, commandType).ToList();
        }

        public T ExecuteScalar<T>(string query)
        {
            return ExecuteScalar<T>(query, null, (CommandType) 0);
        }

        public T ExecuteScalar<T>(string query, object parameters)
        {
            return ExecuteScalar<T>(query, parameters, (CommandType) 0);
        }

        public T ExecuteScalar<T>(string query, CommandType commandType)
        {
            return ExecuteScalar<T>(query, null, commandType);
        }

        public T ExecuteScalar<T>(string query, object parameters, CommandType commandType)
        {
            using (IDbConnection connection = _provider.CreateConnection())
            using (IDbCommand command = _provider.CreateCommand(query, connection, commandType))
            {
                AppendParameters(command, parameters);

                connection.Open();
                return (T)command.ExecuteScalar();
            }
        }

        private void AppendParameters(IDbCommand command, object parameters)
        {
            if (parameters != null)
            {
                IDictionary<string, object> parameterDictionary = GenerateParameterDictionary(parameters);
                IEnumerable<IDataParameter> generatedParameters = _provider.CreateParameters(parameterDictionary);

                foreach (IDataParameter parameter in generatedParameters)
                    command.Parameters.Add(parameter);
            }
        }

        private static IDictionary<string, object> GenerateParameterDictionary(object parameters)
        {
            Type parametersType = parameters.GetType();

            return parametersType != typeof (Dictionary<string, object>)
                       ? parametersType.GetProperties().ToDictionary(p => p.Name, p => p.GetValue(parameters, null))
                       : parameters as Dictionary<string, object>;
        }

        private static T ComposeType<T>(IDataReader reader) where T : new()
        {
            Type currentType = typeof (T);

            if (currentType.GetProperties().Length == 0)
                throw new Exception("Can't map the results to the provided type, you can try to use dynamic as return type.");

            var entity = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                PropertyInfo property = currentType.GetProperty(reader.GetName(i));
                if (property != null)
                    property.SetValue(entity, reader[i]);
            }
            return entity;
        }

        private static IDictionary<string, object> ComposeExpandoObject(IDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (int i = 0; i < reader.FieldCount; i++)
                expandoObject.Add(reader.GetName(i), reader[i]);

            return expandoObject;
        }
    }
}