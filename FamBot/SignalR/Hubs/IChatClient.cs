namespace FamBot.SignalR.Hubs
{
    public interface IChatClient
    {
        Task RecieveMessage(string userColor, string message);
    }
}