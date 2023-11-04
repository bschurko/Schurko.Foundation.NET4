
using Dapper;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;
using Schurko.Foundation.Identity.Impersonation;
using Schurko.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


#nullable enable
namespace Schurko.Foundation.Messaging.DbMsgQueue
{
    public class MessageQueuePoolService : IMessageQueuePoolService
  {
    private ILoggerFactory loggerFactory = (ILoggerFactory) new LoggerFactory();
    private ILogger? _logger;
    private string _connectionString;

    private ILogger Logger => this._logger ?? (this._logger = this.loggerFactory.CreateLogger("MsgQueueService"));

    public MessageQueuePoolService(string connectionString) => this._connectionString = connectionString;

    public async Task<string?> InMessageQueueAsync(string identifier, string message, int lifeInQueueSeconds = 60, int maxLength = 7500)
    {
      try
      {
        const string StoredProcName = MessageQueuePoolSprocs.InMessageQueue;
        identifier = identifier?.Trim();

        var p = new DynamicParameters();
        p.Add("identifier", identifier);
        p.Add("message", message);
        p.Add("lifeInQueueSeconds", lifeInQueueSeconds);
        p.Add("maxLength", maxLength);
                 
        var result = await StoredProcWithParamsDynamicAsync(StoredProcName, p).ConfigureAwait(false);

        var id = result != null ? (dynamic) result.FirstOrDefault() : null;

        if (id == null) throw new Exception("InMessageQueueAsync SPROC response failed.");

        var val = id.MessageId;

        return val.ToString();
      }
      catch (Exception ex)
      {
        Logger.LogError("GetProjectResourcesAsync Exception", ex);
        throw;
      }
    }

    public async Task<MessageQueueBase> DeMessageQueueAsync(string identifier, int numberOfMessage = 1)
    {
            try
            {
                const string StoredProcName = MessageQueuePoolSprocs.DeMessageQueue;

                identifier = identifier.Trim();

                var p = new DynamicParameters();
                p.Add("identifier", identifier);
                p.Add("numberOfMessage", numberOfMessage);
                p.Add("OUTPUT", 0, DbType.Int32, ParameterDirection.Output);

                var results = await StoredProcWithParamsAsync<MessageQueueBase>(StoredProcName, p).ConfigureAwait(false);

                if (p.Get<int>("OUTPUT") > 0)
                    throw new Exception("Stored Procedure failed to DeMessageQueueAsync.");

                var record = results != null ? results.FirstOrDefault() : null;

                if (record == null) this.Logger.LogError("DeMessageQueue SPROC response failed.");

                return record;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetProjectResourcesAsync Exception", ex);
                return null;
            }
        }

    public async void PurgeMessageQueueAsync(int deQueueTimeoutSeconds = 60)
    {
      try
      {
        List<object> objectList = await this.StoredProcWithParamsDynamicAsync(MessageQueuePoolSprocs.PurgeMessageQueue, 
            (object) new DynamicParameters()).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this.Logger.LogError("GetProjectResourcesAsync Exception", (object) ex);
        throw;
      }
    }

    public async Task<bool> DeleteQueueMessageAsync(Guid messageId)
    {
      try
      {
        var p = new DynamicParameters();
        p.Add("messageId", messageId);
        p.Add("OUTPUT", 0, DbType.Int32, ParameterDirection.Output);

        List<object?> source = await this.StoredProcWithParamsDynamicAsync(MessageQueuePoolSprocs.DeleteMessage, p).ConfigureAwait(false);

        if (p.Get<int>("OUTPUT") > 0)
            throw new Exception("Stored Procedure failed to delete queued messages by messageId.");

        var result = source?.FirstOrDefault<object>();

        if (result == null)
            Logger.LogError("DeleteQueueMessage SPROC response failed.");

        return true;
      }
      catch (Exception ex)
      {
        this.Logger.LogError("DeleteQueueMessageAsync Exception", (object) ex);
        return false;
      }
    }

    public async Task<bool> DeleteQueueMessagesByIdentifierAsync(string identifier)
    {
        try
        {
            identifier = identifier.Trim();
            var p = new DynamicParameters();
            p.Add("identifier", identifier);
            p.Add("OUTPUT", 0, DbType.Int32, ParameterDirection.Output);
            List<object?> source = await this.StoredProcWithParamsDynamicAsync(MessageQueuePoolSprocs.DeleteMessageByIdentifer, p).ConfigureAwait(false);

            if (p.Get<int>("OUTPUT") > 0)
                throw new Exception("Stored Procedure failed to delete queued messages by identifier.");

            var result = source?.FirstOrDefault<object>();

            if (result == null)
                this.Logger.LogError("DeleteMessageByIdentifer SPROC response failed.");

            return true;
        } 
        catch(Exception ex)
        {
            Logger.LogError("DeleteMessageByIdentifier Exception", ex);
            return false;
        }
    }

    public async Task<IEnumerable<MessageQueueModel>> GetIdentifierByMessageIdAsync(string messageId)
    {
      string str = "SELECT MessageId, SegmentSeq, Identifier, SegmentMsg, InQueueTime , ExpiryTime, DeQueueTime, State FROM dbo.MsgQueue WHERE messageId = @messageId ORDER BY Identifier, SegmentSeq";
      IEnumerable<MessageQueueModel> byMessageIdAsync;
      var p = new DynamicParameters();
      p.Add("messageId", messageId);

      using (SqlConnection con = new SqlConnection(this._connectionString))
      {
        byMessageIdAsync = await con.QueryAsync<MessageQueueModel>(str, p).ConfigureAwait(false);
      }
             
      return byMessageIdAsync;
    }

    public async Task<IEnumerable<MessageQueueModel>> GetMessageIdByIdentifierAsync(string identifier)
    {
        string str = "SELECT MessageId, SegmentSeq, Identifier,SegmentMsg, InQueueTime , ExpiryTime, DeQueueTime, State FROM dbo.MsgQueue WHERE Identifier = '@Identifier' ORDER BY Identifier, SegmentSeq";      
      
        IEnumerable<MessageQueueModel> byIdenrifierAsync;
        var p = new DynamicParameters();
        p.Add("identifier", identifier);

        using (SqlConnection con = new SqlConnection(this._connectionString))
        {
                byIdenrifierAsync = await con.QueryAsync<MessageQueueModel>(str, p).ConfigureAwait(false);

        }
       
      return byIdenrifierAsync;
    }

    private List<object> StoredProcWithParamsDynamic(
      string procname,
      object parms,
      string connectionName = null)
    {
        using (GetImpersonation())
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.Query(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();
        }
    }

    private async Task<List<object>> StoredProcWithParamsDynamicAsync(
      string procname,
      object parms,
      string connectionName = null)
    {
        using (GetImpersonation())
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var task = await connection.QueryAsync(procname, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return task.ToList();
        }
    }

    private List<T> StoredProcWithParams<T>(string procname, object parms)
    {
            using (GetImpersonation())
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ToList();
            }
        }

    private async Task<List<T>> StoredProcWithParamsAsync<T>(string procname, object parms)
    {
        using (GetImpersonation())
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var task = await connection.QueryAsync<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return task.ToList();
        }
    }

    private SecurityImpersonation GetImpersonation()
    {
      ICredentialProvider provider = (ICredentialProvider) null;
      return provider == null ? (SecurityImpersonation) null : new SecurityImpersonation(provider);
    }

    public sealed class MessageQueuePoolSprocs
    {
      public const string DeleteMessage = "[dbo].[DeleteMessage]";
      public const string DeMessageQueue = "[dbo].[DeMessageQueue]";
      public const string InMessageQueue = "[dbo].[InMessageQueue]";
      public const string PurgeMessageQueue = "[dbo].[PurgeMessageQueue]";
      public const string DeleteMessageByIdentifer = "[dbo].[DeleteMessageByIdentifer]";
    }
  }
}
