
using System;


#nullable enable
namespace Schurko.Foundation.Messaging.DbMsgQueue
{
  public class MessageQueueModel : MessageQueueBase
  {
    public int? SegmentSeq { get; set; }

    public DateTime? InQueueTime { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public DateTime? DeQueueTime { get; set; }

    public int? State { get; set; }

    public MessageQueueModel()
    {
    }

    public MessageQueueModel(
      Guid messageId,
      int? segmentSeq,
      string identifier,
      string segmentMsg,
      DateTime? inQueueTime,
      DateTime? expiryTime,
      DateTime? deQueueTime,
      int? state)
      : base(messageId, segmentMsg, identifier)
    {
      this.SegmentSeq = segmentSeq;
      this.InQueueTime = inQueueTime;
      this.ExpiryTime = expiryTime;
      this.DeQueueTime = deQueueTime;
      this.State = state;
    }

    public MessageQueueModel(
      int? segmentSeq,
      string identifier,
      string segmentMsg,
      DateTime? inQueueTime,
      DateTime? expiryTime,
      DateTime? deQueueTime,
      int? state)
      : base(segmentMsg, identifier)
    {
      this.SegmentSeq = segmentSeq;
      this.InQueueTime = inQueueTime;
      this.ExpiryTime = expiryTime;
      this.DeQueueTime = deQueueTime;
      this.State = state;
    }
  }
}
