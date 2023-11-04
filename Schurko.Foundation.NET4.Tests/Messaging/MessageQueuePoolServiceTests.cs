using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schurko.Foundation.Messaging.DbMsgQueue;
using Schurko.Foundation.Utilities;

namespace Schurko.Foundation.Tests.Messaging
{
    [TestClass]
    public class MessageQueuePoolServiceTests
    {
        [TestMethod]
        public async Task MessageQueuePoolService_InMessageQueue_TESTS()
        {
            string messageId = InMessageQueueProcess();
            Assert.IsNotNull(messageId);
        }

        [TestMethod]
        public async Task MessageQueuePoolService_DeMessageQueue_TESTS()
        {
            string messageId = InMessageQueueProcess();
            var service = GetMessageQueuePoolService();
            var results = await service.GetIdentifierByMessageIdAsync(messageId).ConfigureAwait(false);
            var item = results.FirstOrDefault();

            MessageQueueBase mqb = await service.DeMessageQueueAsync(item.Identifier).ConfigureAwait(false);

            Assert.IsNotNull(mqb.Identifier);
        }

        [TestMethod]
        public async Task MessageQueuePoolService_DeleteQueueMessageAsync_TESTS()
        {
            string messageId = InMessageQueueProcess();
            var service = GetMessageQueuePoolService();

            bool isDeleted = await service.DeleteQueueMessageAsync(Guid.Parse(messageId)).ConfigureAwait(false);

            Assert.IsTrue(isDeleted);
        }

        

        [TestMethod]
        public async Task MessageQueuePoolService_GetMessageIdByIdenrifierAsync_TESTS()
        {
            string messageId = InMessageQueueProcess();
            var service = GetMessageQueuePoolService();

            var messageQueueModelList = await service.GetIdentifierByMessageIdAsync(messageId).ConfigureAwait(false);
            var result = messageQueueModelList.FirstOrDefault();

            Assert.IsNotNull(result.Identifier);
        }

        [TestMethod]
        public async Task MessageQueuePoolService_DeleteQueueMessagesByIdentifierAsync_TESTS()
        {
            string messageId = InMessageQueueProcess();
            var service = GetMessageQueuePoolService();
            var messageQueueModelList = await service.GetIdentifierByMessageIdAsync(messageId).ConfigureAwait(false);
            var result = messageQueueModelList.FirstOrDefault();

            bool isDeleted = await service.DeleteQueueMessagesByIdentifierAsync(result.Identifier).ConfigureAwait(false);

            Assert.IsTrue(isDeleted);
        }

        [TestMethod]
        public async Task MessageQueuePoolService_DeleteQueueMessagesAsync_TESTS()
        {
            string messageId = InMessageQueueProcess();
            var service = GetMessageQueuePoolService();

            bool isDeleted = await service.DeleteQueueMessageAsync(Guid.Parse(messageId)).ConfigureAwait(false);

            Assert.IsTrue(isDeleted);
        }

        #region Private

        /// <summary>
        /// Processes a job into the message queue and returns a MessageId.
        /// </summary>
        /// <returns></returns>
        public string InMessageQueueProcess()
        {
            const int MaxMessageLength = 7499;
            string message = new string('-', MaxMessageLength);
            var service = GetMessageQueuePoolService();
            var id = GetIdentifier();

            var task = service.InMessageQueueAsync(id, message);
            task.Wait();
            var result = task.Result;
            return result;
        }

        public string GetIdentifier()
        {
            var str = Guid.NewGuid().ToString().Replace("-", "").Trim();
            return str;
        }

        public MessageQueuePoolService GetMessageQueuePoolService()
        {
            var config = StaticConfigurationManager.GetConfiguration();
            var connStr = config.GetConnectionString("InMemDb");
            var service = new MessageQueuePoolService(connStr);
            return service;
        }

        #endregion
    }
}
