﻿using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Slack.Host.Options;

using SlackNet;
using SlackNet.Bot;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Host
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
        public Task Connect(CancellationToken? ct = null) => _implementation.Connect(ct);
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
        public Task Send(BotMessage message, CancellationToken? ct = null) => _implementation.Send(message, ct);
        public Task WhileTyping(string channelId, Func<Task> action) => _implementation.WhileTyping(channelId, action);
        public void ClearCache() => _implementation.ClearCache();
        [Obsolete("Use GetConversationById instead")]
        public Task<Hub> GetHubById(string hubId) => _implementation.GetHubById(hubId);
        [Obsolete("Use GetConversationByName instead")]
        public Task<Hub> GetHubByName(string channel) => _implementation.GetHubByName(channel);
        [Obsolete("Use GetConversationByName instead")]
        public Task<Hub> GetChannelByName(string name) => _implementation.GetChannelByName(name);
        [Obsolete("Use GetConversationByName instead")]
        public Task<Hub> GetGroupByName(string name) => _implementation.GetGroupByName(name);
        [Obsolete("Use GetConversationByName instead")]
        public Task<Im> GetImByName(string username) => _implementation.GetImByName(username);
        [Obsolete("Use GetConversationByUserId instead")]
        public Task<Im> GetImByUserId(string userId) => _implementation.GetImByUserId(userId);
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Channel>> GetChannels() => _implementation.GetChannels();
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Channel>> GetGroups() => _implementation.GetGroups();
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Channel>> GetMpIms() => _implementation.GetMpIms();
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Im>> GetIms() => _implementation.GetIms();
    }
}