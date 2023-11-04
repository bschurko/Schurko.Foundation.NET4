
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Schurko.Foundation.Messaging.DbMsgQueue
{
  public interface IMessageQueuePoolService
  {
    Task<string> InMessageQueueAsync(
      string identifier,
      string message,
      int lifeInQueueSeconds = 60,
      int maxLength = 7500);

    Task<MessageQueueBase> DeMessageQueueAsync(string identifier, int numberOfMessage = 1);

    void PurgeMessageQueueAsync(int deQueueTimeoutSeconds = 60);

    Task<bool> DeleteQueueMessageAsync(Guid messageId);

    Task<bool> DeleteQueueMessagesByIdentifierAsync(string identifier);

    Task<IEnumerable<MessageQueueModel>> GetIdentifierByMessageIdAsync(string messageId);

    Task<IEnumerable<MessageQueueModel>> GetMessageIdByIdentifierAsync(string identifier);
  }
}
