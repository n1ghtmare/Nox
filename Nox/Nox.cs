using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Nox
{
    public class Nox
    {
        private readonly INoxProvider _provider;

        public Nox(INoxProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<T> Execute<T>(string query, object parameters = null) where T : new()
        {
            using (IDbConnection connection = _provider.CreateConnection())
            using (IDbCommand command = _provider.CreateCommand(query, connection))
            {
                AppendParameters(command, parameters);

                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        // TODO -> need to optimize this bit here
                        if (typeof(T) == typeof(Object))
                            yield return (T)ComposeExpandoObject(reader);
                        else
                            yield return ComposeType<T>(reader);
                    }
                }
            }
        }

        public IEnumerable<dynamic> Execute(string query, object parameters = null)
        {
            return Execute<dynamic>(query, parameters);
        }

        public T ExecuteScalar<T>(string query, object parameters = null)
        {
            using (IDbConnection connection = _provider.CreateConnection())
            using (IDbCommand command = _provider.CreateCommand(query, connection))
            {
                AppendParameters(command, parameters);

                connection.Open();
                return (T)command.ExecuteScalar();
            }
        }

        private void AppendParameters(IDbCommand command, object parameters)
        {
            IEnumerable<IDataParameter> generatedParameters = _provider.CreateParameters(parameters);

            foreach (IDataParameter parameter in generatedParameters)
                command.Parameters.Add(parameter);
        }

        private static T ComposeType<T>(IDataReader reader) where T : new()
        {
            try
            {
                dynamic entity = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                    (typeof (T)).GetProperty(reader.GetName(i)).SetValue(entity, reader[i]);

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format(
                        "Can't map the results to the provided type, you can try to use dynamic as return type. - Inner Exception::{0}",
                        ex.InnerException));
            }
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