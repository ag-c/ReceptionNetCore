﻿using Microsoft.AspNetCore.SignalR.Client;
using Reception.App.Network.Chat.Constants;
using Reception.Model.Network;
using System;
using System.Threading.Tasks;

namespace Reception.App.Network.Chat
{
    public class ClientService : IClientService, IDisposable
    {
        #region Fields
        private readonly HubConnection _client;

        private readonly int _userId;
        #endregion

        #region ctor
        public ClientService(int userId, string serverPath, bool withReconnect = true)
        {
            _userId = userId;

            var hubBuilder = new HubConnectionBuilder().WithUrl(serverPath);
            if (withReconnect)
            {
                hubBuilder = hubBuilder.WithAutomaticReconnect();
            }
            _client = hubBuilder.Build();

            _client.Closed += Closed;
            _client.Reconnected += Reconnected;
            _client.Reconnecting += Reconnecting;
            
            _client.On(ChatMethodType.RECEIVER, OnReceive());
        }
        #endregion

        #region Events
        public event Func<Exception, Task> Closed;

        public event Func<bool, Task> Connected;

        public event Func<int, Type, object, Task> MessageReceived;

        public event Func<string, Task> Reconnected;

        public event Func<Exception, Task> Reconnecting;
        #endregion

        #region Methods
        private Func<int, QueryResult<object>, Task> OnReceive()
        {
            return (userId, message) => MessageReceived?.Invoke(userId, message.DataType, message.Data);
        }

        public async Task SendAsync<T>(T value)
        {
            var query = new QueryResult<T> { DataType = typeof(T), Data = value };
            await _client.SendAsync(ChatMethodNames.SEND_MESSAGE_BROADCAST, _userId, query);
        }

        public async Task StartClientAsync()
        {
            await _client.StartAsync();
            Connected?.Invoke(true);
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected async virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client.Closed -= Closed;
                    _client.Reconnected -= Reconnected;
                    _client.Reconnecting -= Reconnecting;
                    await _client.DisposeAsync();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}