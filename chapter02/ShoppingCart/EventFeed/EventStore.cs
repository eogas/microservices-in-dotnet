namespace ShoppingCart.EventFeed;

public interface IEventStore
{
    IEnumerable<Event> GetEvents(
        long firstEventSequenceNumber,
        long lastEventSequenceNumber
    );
    void Raise(string eventName, object content);
}

// In memory EventStore for now
public class EventStore : IEventStore
{
    private static readonly HashSet<Event> database = new();

    public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber) =>
        database.Where(e =>
            e.SequenceNumber >= firstEventSequenceNumber &&
            e.SequenceNumber <= lastEventSequenceNumber)
        .OrderBy(e => e.SequenceNumber);

    public void Raise(string eventName, object content)
    {
        //var seqNumber = database.NextSequenceNumber();
        long seqNumber = 0;

        if (database.Any())
        {
            seqNumber = database.Max(e => e.SequenceNumber) + 1;
        }

        database.Add(
            new Event(
                seqNumber,
                DateTimeOffset.UtcNow,
                eventName,
                content
            )
        );
    }
}
