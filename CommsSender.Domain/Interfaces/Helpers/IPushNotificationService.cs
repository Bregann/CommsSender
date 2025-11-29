namespace CommsSender.Domain.Interfaces.Helpers
{
    public interface IPushNotificationService
    {
        Task RegisterPushToken(string token);
    }
}
