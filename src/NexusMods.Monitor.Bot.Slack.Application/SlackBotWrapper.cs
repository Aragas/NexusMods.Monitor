using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Slack.Application.Options;

using SlackNet;
using SlackNet.Bot;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application
{
    public class SlackBotWrapper : ISlackBot
    {
        public string Id => _implementation.Id;
        public string Name => _implementation.Name;
        public IObservable<IMessage> Messages => _implementation.Messages;

        public event EventHandler<IMessage>? OnMessage
        {
            add => _implementation.OnMessage += value;
            remove => _implementation.OnMessage -= value;
        }

        private readonly ISlackBot _implementation;

        public SlackBotWrapper(IOptions<SlackOptions> options)
        {
            _implementation = new SlackBot(options.Value.BotToken);
        }

        public void OnCompleted() => _implementation.OnCompleted();
        public void OnError(Exception error) => _implementation.OnError(error);
        public void OnNext(BotMessage value) => _implementation.OnNext(value);
        public Task Connect(CancellationToken? cancellationToken = null) => _implementation.Connect(cancellationToken);
        public void AddIncomingMiddleware(Func<IObservable<IMessage>, IObservable<IMessage>> middleware) => _implementation.AddIncomingMiddleware(middleware);
        public void AddOutgoingMiddleware(Func<IObservable<BotMessage>, IObservable<BotMessage>> middleware) => _implementation.AddOutgoingMiddleware(middleware);
        public void AddHandler(IMessageHandler handler) => _implementation.AddHandler(handler);
        public Task<IReadOnlyCollection<Conversation>> GetConversations() => _implementation.GetConversations();
        public Task<Conversation> GetConversationById(string conversationId) => _implementation.GetConversationById(conversationId);
        public Task<Conversation> GetConversationByName(string conversationName) => _implementation.GetConversationByName(conversationName);
        public Task<Conversation> GetConversationByUserId(string userId) => _implementation.GetConversationByUserId(userId);
        public Task<User> GetUserById(string userId) => _implementation.GetUserById(userId);
        public Task<BotInfo> GetBotUserById(string botId) => _implementation.GetBotUserById(botId);
        public Task<User> GetUserByName(string username) => _implementation.GetUserByName(username);
        public Task<IReadOnlyList<User>> GetUsers() => _implementation.GetUsers();
        public Task Send(BotMessage message, CancellationToken? cancellationToken = null) => _implementation.Send(message, cancellationToken);
        public Task WhileTyping(string channelId, Func<Task> action) => _implementation.WhileTyping(channelId, action);
        public void ClearCache() => _implementation.ClearCache();
        public Task<Hub> GetHubById(string hubId) => _implementation.GetHubById(hubId);
        public Task<Hub> GetHubByName(string channel) => _implementation.GetHubByName(channel);
        public Task<Hub> GetChannelByName(string name) => _implementation.GetChannelByName(name);
        public Task<Hub> GetGroupByName(string name) => _implementation.GetGroupByName(name);
        public Task<Im> GetImByName(string username) => _implementation.GetImByName(username);
        public Task<Im> GetImByUserId(string userId) => _implementation.GetImByUserId(userId);
        public Task<IReadOnlyList<Channel>> GetChannels() => _implementation.GetChannels();
        public Task<IReadOnlyList<Channel>> GetGroups() => _implementation.GetGroups();
        public Task<IReadOnlyList<Channel>> GetMpIms() => _implementation.GetMpIms();
        public Task<IReadOnlyList<Im>> GetIms() => _implementation.GetIms();
    }
}