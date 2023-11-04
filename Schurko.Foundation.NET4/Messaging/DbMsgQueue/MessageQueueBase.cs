
using System;


#nullable enable
namespace Schurko.Foundation.Messaging.DbMsgQueue
{
  public class MessageQueueBase
  {
    private string _identifier;

    public Guid MessageId { get; set; }

    public string Identifier
    {
      get => this._identifier == null ? string.Empty : this._identifier.Trim();
      set => this._identifier = value;
    }

    public string SegmentMsg { get; set; }

    public MessageQueueBase()
    {
    }

    public MessageQueueBase(Guid messageId, string identifier, string segmentMsg)
    {
      this.MessageId = messageId;
      this.SegmentMsg = segmentMsg;
      this.Identifier = identifier;
    }

    public MessageQueueBase(string identifier, string segmentMsg)
    {
      this.MessageId = Guid.Empty;
      this.SegmentMsg = segmentMsg;
      this.Identifier = identifier;
    }
  }
}
