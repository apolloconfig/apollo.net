namespace Com.Ctrip.Framework.Apollo.Core.Dto;

internal class ApolloNotificationMessages
{
    public IDictionary<string, long> Details { get; init; } = new Dictionary<string, long>();

    public void MergeFrom(ApolloNotificationMessages? source)
    {
        if (source == null) return;

        foreach (var entry in source.Details)
        {
            //to make sure the notification id always grows bigger
            if (!Details.TryGetValue(entry.Key, out var value) || value < entry.Value) Details[entry.Key] = entry.Value;
        }
    }

    public ApolloNotificationMessages Clone() => new() { Details = new Dictionary<string, long>(Details) };
}
