namespace Com.Ctrip.Framework.Apollo.Core.Dto;

internal class ApolloConfigNotification
{
    private ApolloNotificationMessages? _messages;

    public string NamespaceName { get; set; } = default!;

    public long NotificationId { get; set; }

    public ApolloNotificationMessages? Messages
    {
        get => _messages;
        set => _messages = value;
    }

    public override string ToString() => $"ApolloConfigNotification{{namespaceName='{NamespaceName}{'\''}, notificationId={NotificationId}{'}'}";
}
