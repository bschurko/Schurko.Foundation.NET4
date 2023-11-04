
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Messaging.RabbitMQ
{
  public class RabbitMQService
  {
    private string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public RabbitMQService(string hostName, string userName, string password)
    {
      this.HostName = hostName;
      this.UserName = userName;
      this.Password = password;
    }

    private IConnection GetConnection(string hostName, string userName, string password) => new ConnectionFactory()
    {
      HostName = hostName,
      UserName = userName,
      Password = password,
      Port = Protocols.DefaultProtocol.DefaultPort,
      VirtualHost = "/",
      ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
    }.CreateConnection();

    public void Publish(string queueName, string message, bool isDurable = true)
    {
      using (IConnection connection = this.GetConnection(this.HostName, this.UserName, this.Password))
      {
        using (IModel model = connection.CreateModel())
        {
          model.QueueDeclare(queueName, isDurable, false, false, (IDictionary<string, object>) null);
          IBasicProperties basicProperties = model.CreateBasicProperties();
          model.QueueDeclare(queueName, isDurable, false, false, (IDictionary<string, object>) null);
          IModelExensions.BasicPublish(model, string.Empty, queueName, basicProperties, (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(message));
          model.Close();
          Console.WriteLine(" [x] Sent " + message);
        }
      }
    }

    public string Consume(string queueName, bool isDurable = true)
    {
      string str = (string) null;
      using (IConnection connection = new ConnectionFactory().CreateConnection())
      {
        using (IModel model = connection.CreateModel())
        {
          model.QueueDeclare(queueName, isDurable, false, false, (IDictionary<string, object>) null);
          EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(model);
          BasicGetResult basicGetResult = model.BasicGet(queueName, true);
          if (basicGetResult != null)
            str = Encoding.UTF8.GetString(basicGetResult.Body.ToArray());
        }
      }
      return str;
    }
  }
}
