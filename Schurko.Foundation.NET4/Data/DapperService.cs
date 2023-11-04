using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Schurko.Foundation.Identity.Impersonation;
using Schurko.Foundation.Logging;
using Schurko.Foundation.Utilities;

namespace Schurko.Foundation.Data
{
    public class DapperService
    {
        #region Members

        private readonly ILogger _logger;
        private readonly IConnectionString _dbConnectionStringContext;

        private readonly string _connectionStringName;
        private readonly string _connectionStringValue;
        #endregion Members

        #region Constructor
        public DapperService(


            IConnectionString dbConnectionStringContext)
        {
            _logger = Log.Logger;

            _dbConnectionStringContext = dbConnectionStringContext;
            _connectionStringName = _dbConnectionStringContext.Name;
            _connectionStringValue = _dbConnectionStringContext.Value;
        }
        #endregion Constructor

        #region Public
        /// <summary>
        /// Gets an open SQL Connection using the supplied [name] argument for matching a connection string in the
        /// Web.config. If the [name] argument is not supplied, then a connection string will be resolved using the
        /// configuration data in the ISettingManager, IConnectionManager and IConnectionString classes.
        /// </summary>
        /// <param name="name">The name of the connection string (optional).</param>
        /// <returns></returns>
        public SqlConnection GetOpenConnection(string name = null)
        {
            string connectionString = null;

            if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(_connectionStringValue))
            {
                connectionString = _connectionStringValue;
            }
            else
            {

                connectionString = StaticConfigurationManager.AppSetting.GetConnectionString(name ?? _connectionStringName) ?? _connectionStringValue;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new SettingsPropertyNotFoundException(
                    "DapperService failed to find a connection string to the Database.");
            }

            var connection = new SqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        /// <summary>
        /// Gets a SecurityImpersonation object, whose permissions are dictated by the configuration in ISettingManager.
        /// Additionally, an specific impersonation mode must be set in the ISettingManager class.
        /// </summary>
        /// <returns></returns>
        public SecurityImpersonation GetImpersonation()
        {
            ICredentialProvider provider = null;

            return provider != null ? new SecurityImpersonation(provider) : null;
        }

        /// <summary>
        /// The InsertMultiple method accepts an IEnumerable of your type, iterates over it, and performs an insert on every one, all on the same connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="entities"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public int InsertMultiple<T>(string sql, IEnumerable<T> entities, string connectionName = null) where T : class, new()
        {
            using (GetImpersonation())
            using (SqlConnection cnn = GetOpenConnection(connectionName))
            {
                int records = 0;

                foreach (T entity in entities)
                {
                    records += cnn.Execute(sql, entity);
                }
                return records;
            }
        }

        /// <summary>
        /// The GetParametersFromObject method accepts your type and returns a set of Dapper DynamicParameters, optionally
        /// excluding a list of property names to ignore. This is useful since many of the underlying Dapper methods want
        /// DynamicParameters. Of course you can create your own DynamicParameters, optionally specifying properties such
        /// as a parameter being an output parameter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyNamesToIgnore"></param>
        /// <returns></returns>
        public DynamicParameters GetParametersFromObject(object obj, string[] propertyNamesToIgnore)
        {
            if (propertyNamesToIgnore == null) propertyNamesToIgnore = new string[] { string.Empty };
            DynamicParameters p = new DynamicParameters();
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                if (!propertyNamesToIgnore.Contains(prop.Name))
                    p.Add("@" + prop.Name, prop.GetValue(obj, null));
            }
            return p;
        }

        /// <summary>
        ///  The SetIdentity method is useful when you are doing an insert and want to get back the @@IDENTITY
        /// value of the new row. You would call this method immediately after the line of code that performs
        /// your insert, and before the connection is closed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="setId"></param>
        public T SetIdentity<T>(IDbConnection connection, IDbTransaction transaction)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id", null, transaction).FirstOrDefault();

            T newId = default;

            if (identity != null)
            {
                newId = (T)identity.Id;
            }

            return newId;
        }

        /// <summary>
        /// Gets an integer based number that represents the number or rows/records that were
        /// returned by the previous SQL query.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public long? GetRowCount(IDbConnection connection, IDbTransaction transaction)
        {

            dynamic count = connection.Query("SELECT @@ROWCOUNT AS ROW_COUNT", null, transaction).FirstOrDefault();

            long? countResult = null;

            if (count != null && count.ROW_COUNT != null)
            {
                countResult = count.ROW_COUNT;
            }

            return countResult;
        }

        /// <summary>
        ///  The GetPropertyValue and SetPropertyValue methods are, again, convenience methods. I don't often
        /// use these, but as above, I keep the methods in the class so I don't need to go hunting around for
        /// code when I do.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(object target, string propertyName)
        {
            PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            object theValue = null;
            foreach (PropertyInfo prop in properties)
            {
                if (string.Compare(prop.Name, propertyName, true) == 0)
                {
                    theValue = prop.GetValue(target, null);
                }
            }
            return theValue;
        }

        /// <summary>
        ///   The GetPropertyValue and SetPropertyValue methods are, again, convenience methods. I don't often
        /// use these, but as above, I keep the methods in the class so I don't need to go hunting around for
        /// code when I do.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public void SetPropertyValue(object p, string propName, object value)
        {
            Type t = p.GetType();
            PropertyInfo info = t.GetProperty(propName);
            if (info == null)
                return;
            if (!info.CanWrite)
                return;
            info.SetValue(p, value, null);
        }

        /// <summary>
        ///  The StoredProcWithParams<T> method executes a named stored proc, accepting an instance of dynamic containing the parameters, and an optional connection name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public List<T> StoredProcWithParams<T>(string procname, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {

                return connection.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        /// <summary>
        /// [Async Version]
        /// The StoredProcWithParams<T> method executes a named stored proc, accepting an instance of dynamic containing the parameters, and an optional connection name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname"></param>
        /// <param name="parms"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public async Task<List<T>> StoredProcWithParamsAsync<T>(string procname, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {

                var task = await connection.QueryAsync<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task.ToList();
            }
        }

        /// <summary>
        /// Stored proc with params returning dynamic.
        /// </summary>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public List<dynamic> StoredProcWithParamsDynamic(string procname, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Query(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        /// <summary>
        /// [Async Version]
        /// Stored proc with params returning dynamic.
        /// </summary>
        /// <param name="procname"></param>
        /// <param name="parms"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> StoredProcWithParamsDynamicAsync(string procname, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task =
                    await connection.QueryAsync(procname, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task.ToList();
            }
        }

        /// <summary>
        /// Stored proc insert with ID.
        /// The   U StoredProcInsertWithID<T,U> method executes a stored proc and expects a defined output parameter to return the value to the caller.
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <typeparam name="U">The Type of the ID</typeparam>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="parms">instance of DynamicParameters class. This
        /// should include a defined output parameter</param>
        /// <returns>U - the @@Identity value from output parameter</returns>
        public async Task<U> StoredProcInsertWithIDAsync<T, U>(string procName, DynamicParameters parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task = await connection.ExecuteAsync(procName, parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return parms.Get<U>("@ID");
            }
        }

        /// <summary>
        /// SQL with params.
        /// The SqlWithParams<T> method executes a specified SQL string with dynamic parameters and returns a List<T> of the type specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public List<T> SqlWithParams<T>(string sql, dynamic parms, string connectionnName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionnName))
            {
                return connection.Query<T>(sql, (object)parms).ToList();
            }
        }

        /// <summary>
        /// [Async Version]
        /// SQL with params.
        /// The SqlWithParams<T> method executes a specified SQL string with dynamic parameters and returns a List<T> of the type specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="connectionnName"></param>
        /// <returns></returns>
        public async Task<List<T>> SqlWithParamsAsync<T>(string sql, dynamic parms, string connectionnName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionnName))
            {
                var task = await connection.QueryAsync<T>(sql, (object)parms).ConfigureAwait(false);

                return task.ToList();
            }
        }

        /// <summary>
        /// Insert update or delete SQL.
        /// The InsertUpdateOrDeleteSql method will perform an insert, Update, or Delete with the specified SQL
        /// with dynamic parameters and returns the integer result of the operation.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public int InsertUpdateOrDeleteSql(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Execute(sql, (object)parms);
            }
        }

        /// <summary>
        /// [Async Version]
        /// Insert update or delete SQL.
        /// The InsertUpdateOrDeleteSql method will perform an insert, Update, or Delete with the specified SQL
        /// with dynamic parameters and returns the integer result of the operation.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public async Task<int> InsertUpdateOrDeleteSqlAsync(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task = await connection.ExecuteAsync(sql, (object)parms).ConfigureAwait(false);

                return task;
            }
        }

        /// <summary>
        /// Insert update or delete stored proc.
        ///  The InsertUpdateOrDeleteStoredProc  method does the same thing but is used to execute a named stored procedure.
        /// </summary>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public int InsertUpdateOrDeleteStoredProc(string procName, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Execute(procName, (object)parms, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// [Async Version]
        /// Insert update or delete stored proc.
        ///  The InsertUpdateOrDeleteStoredProc  method does the same thing but is used to execute a named stored procedure.
        /// </summary>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        public async Task<int> InsertUpdateOrDeleteStoredProcAsync(string procName, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task = await connection.ExecuteAsync(procName, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task;
            }
        }

        /// <summary>
        /// SQLs the with params single.
        ///  The T SqlWithParamsSingle<T> executes the specified SQL with the supplied dynamic for parameters, and returns a single instance of the type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public T SqlWithParamsSingle<T>(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Query<T>(sql, (object)parms).FirstOrDefault();
            }
        }

        /// <summary>
        /// [Async Version]
        /// SQLs the with params single.
        ///  The T SqlWithParamsSingle<T> executes the specified SQL with the supplied dynamic for parameters, and returns a single instance of the type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public async Task<T> SqlWithParamsSingleAsync<T>(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task = await connection.QueryAsync<T>(sql, (object)parms).ConfigureAwait(false);

                return task.FirstOrDefault();
            }
        }

        /// <summary>
        ///  proc with params single returning Dynamic object.
        ///  The DynamicProcWithParamsSingle<T> executes a stored proc and returns type DynamicObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public System.Dynamic.DynamicObject DynamicProcWithParamsSingle<T>(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Query(sql, (object)parms, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        /// <summary>
        /// [Async Version]
        ///  proc with params single returning Dynamic object.
        ///  The DynamicProcWithParamsSingle<T> executes a stored proc and returns type DynamicObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public async Task<System.Dynamic.DynamicObject> DynamicProcWithParamsSingleAsync<T>(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task = await connection.QueryAsync(sql, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task.FirstOrDefault();
            }
        }

        /// <summary>
        /// proc with params returning Dynamic.
        /// The IEnumerable<dynamic> DynamicProcWithParams<T> does the same and returns an IEnumerable of type dynamic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public IEnumerable<dynamic> DynamicProcWithParams<T>(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Query(sql, (object)parms, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// [Async Version]
        /// proc with params returning Dynamic.
        /// The IEnumerable<dynamic> DynamicProcWithParams<T> does the same and returns an IEnumerable of type dynamic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> DynamicProcWithParamsAsync<T>(string sql, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task = await connection.QueryAsync(sql, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task;
            }
        }

        /// <summary>
        /// Stored proc with params returning single.
        /// The T StoredProcWithParamsSingle<T> executes a stored procedure with the specified dynamic for params and returns a single of type T that was specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public T StoredProcWithParamsSingle<T>(string procname, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                return connection.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        /// <summary>
        /// [Async Version]
        /// Stored proc with params returning single.
        /// The T StoredProcWithParamsSingle<T> executes a stored procedure with the specified dynamic for params and returns a single of type T that was specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procname">The procname.</param>
        /// <param name="parms">The parms.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        public async Task<T> StoredProcWithParamsSingleAsync<T>(string procname, dynamic parms, string connectionName = null)
        {
            using (GetImpersonation())
            using (SqlConnection connection = GetOpenConnection(connectionName))
            {
                var task =
                    await connection.QueryAsync<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task.FirstOrDefault();
            }
        }
        #endregion Public
    }

    public static class DapperExtensions
    {
        public async static Task<IEnumerable<TFirst>> Map<TFirst, TSecond, TKey>
        (
        this Task<SqlMapper.GridReader> reader,
        Func<TFirst, TKey> firstKey,
        Func<TSecond, TKey> secondKey,
        Action<TFirst, IEnumerable<TSecond>> addChildren
        )
        {
            var first = await reader.Result.ReadAsync<TFirst>();
            var childMapData = await reader.Result.ReadAsync<TSecond>();
            var childMap = childMapData.GroupBy(secondKey).ToDictionary(g => g.Key, g => g.AsEnumerable());

            foreach (var item in first)
            {
                IEnumerable<TSecond> children;
                if (childMap.TryGetValue(firstKey(item), out children))
                {
                    addChildren(item, children);
                }
            }

            return first;
        }

        /// <summary>
        /// Allows one-to-many SQL operations using the Dapper ORM Extension API, along with asynchronous behavior.
        /// Usage -
        /// conn.QueryParentChild<Contact, Phone, int>(
        /// "SELECT * FROM Contact LEFT OUTER JOIN Phone ON Contact.ContactID = Phone.ContactID",
        /// contact => contact.ContactID,
        /// contact => contact.Phones,
        /// splitOn: "PhoneId");
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TParentKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parentKeySelector"></param>
        /// <param name="childSelector"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TParent>> QueryMultipleParentChildAsync<TParent, TChild, TParentKey>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            DynamicParameters param = null, IDbTransaction transaction = null,
            bool buffered = true, string splitOn = "Id",
            int? commandTimeout = null, CommandType? commandType = null)
        {
            Dictionary<TParentKey, TParent> cache = new Dictionary<TParentKey, TParent>();

            var results = await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

            var result1 = results.Read<dynamic, dynamic, Tuple<dynamic, dynamic>>((a, b) => Tuple.Create((object)a, (object)b)).ToList();

            var result2 = results.Read<dynamic, dynamic, Tuple<dynamic, dynamic>>((a, b) => Tuple.Create((object)a, (object)b)).ToList();

            /*
            await connection.QueryAsync<TParent, TChild, TParent>(sql,
                (parent, child) =>
                {
                    if (!cache.ContainsKey(parentKeySelector(parent)))
                    {
                        cache.Add(parentKeySelector(parent), parent);
                    }

                    TParent cachedParent = cache[parentKeySelector(parent)];
                    IList<TChild> children = childSelector(cachedParent);

                    if (!EqualityComparer<TChild>.Default.Equals(child, default(TChild)))
                    {
                        children.Add(child);
                    }

                    return cachedParent;
                },
                param as object, transaction, buffered, splitOn, commandTimeout, commandType).ConfigureAwait(false);
            */
            return cache.Values;
        }

        /// <summary>
        /// Allows one-to-many SQL operations using the Dapper ORM Extension API, along with asynchronous behavior.
        /// Usage -
        /// conn.QueryParentChild<Contact, Phone, int>(
        /// "SELECT * FROM Contact LEFT OUTER JOIN Phone ON Contact.ContactID = Phone.ContactID",
        /// contact => contact.ContactID,
        /// contact => contact.Phones,
        /// splitOn: "PhoneId");
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TParentKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parentKeySelector"></param>
        /// <param name="childSelector"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TParent>> QueryParentChildAsync<TParent, TChild, TParentKey>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            dynamic param = null, IDbTransaction transaction = null,
            bool buffered = true, string splitOn = "Id",
            int? commandTimeout = null, CommandType? commandType = null)
        {
            Dictionary<TParentKey, TParent> cache = new Dictionary<TParentKey, TParent>();

            await connection.QueryAsync<TParent, TChild, TParent>(sql,
                (parent, child) =>
                {
                    if (!cache.ContainsKey(parentKeySelector(parent)))
                    {
                        cache.Add(parentKeySelector(parent), parent);
                    }

                    TParent cachedParent = cache[parentKeySelector(parent)];
                    IList<TChild> children = childSelector(cachedParent);

                    if (!EqualityComparer<TChild>.Default.Equals(child, default))
                    {
                        children.Add(child);
                    }

                    return cachedParent;
                },
                param as object, transaction, buffered, splitOn, commandTimeout, commandType).ConfigureAwait(false);

            return cache.Values;
        }

        /// <summary>
        /// Allows one-to-many SQL operations using the Dapper ORM Extension API.
        /// Usage -
        /// conn.QueryParentChild<Contact, Phone, int>(
        /// "SELECT * FROM Contact LEFT OUTER JOIN Phone ON Contact.ContactID = Phone.ContactID",
        /// contact => contact.ContactID,
        /// contact => contact.Phones,
        /// splitOn: "PhoneId");
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TParentKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parentKeySelector"></param>
        /// <param name="childSelector"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IEnumerable<TParent> QueryParentChild<TParent, TChild, TParentKey>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            Dictionary<TParentKey, TParent> cache = new Dictionary<TParentKey, TParent>();

            connection.Query<TParent, TChild, TParent>(
                sql,
                (parent, child) =>
                {
                    if (!cache.ContainsKey(parentKeySelector(parent)))
                    {
                        cache.Add(parentKeySelector(parent), parent);
                    }

                    TParent cachedParent = cache[parentKeySelector(parent)];
                    IList<TChild> children = childSelector(cachedParent);

                    if (!EqualityComparer<TChild>.Default.Equals(child, default))
                    {
                        children.Add(child);
                    }

                    return cachedParent;
                },
                param as object, transaction, buffered, splitOn, commandTimeout, commandType);

            return cache.Values;
        }


    }

}
