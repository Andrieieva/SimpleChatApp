using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private static readonly List<Message> chatHistory = new List<Message>();

    public async Task SendMessage(string user, string message)
    {
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(message))
        {
            return;
        }

        var newMessage = new Message { User = user, Text = message };
        chatHistory.Add(newMessage);

        if (string.IsNullOrEmpty(newMessage.User) || string.IsNullOrEmpty(newMessage.Text))
        {
            return;
        }

        await Clients.All.SendAsync("ReceiveMessage", newMessage.User, newMessage.Text);
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.User.Identity.Name;

        await Clients.Caller.SendAsync("LoadChatHistory", chatHistory);

        await LoadChatHistory();

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var user = Context.User.Identity.Name;
        await Clients.All.SendAsync("UserDisconnected", user);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task LoadChatHistory()
    {
        var user = Context.User.Identity.Name;
        var userChatHistory = chatHistory.Where(msg => msg.User == user).ToList();
        await Clients.Caller.SendAsync("LoadChatHistory", userChatHistory);
    }

    private class Message
    {
        public string User { get; set; }
        public string Text { get; set; }
    }
}

