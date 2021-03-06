﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Reception.App.Constants;
using Reception.App.Models;
using Reception.App.Network.Server;
using Splat;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Reception.App.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        #region Fields
        private readonly IPingService _pingService;
        #endregion

        #region ctor
        public MainWindowViewModel()
        {
            CenterMessage = "Loading..";

            ServerStatusMessage = ConnectionStatus.OFFLINE.ToLower();
            StatusMessage = ConnectionStatus.OFFLINE.ToLower();

            Router = new RoutingState();

            _pingService ??= Locator.Current.GetService<IPingService>();

            Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromSeconds(AppSettings.PingDelay), RxApp.MainThreadScheduler)
                .Subscribe(async x => await TryPing());

            LoadIsBossMode();

            CenterMessage = "";
        }
        #endregion

        #region Enums
        public enum ErrorType
        {
            No,
            Server,
            Connection,
            System,
            Request
        }
        #endregion

        #region Properties
        [Reactive]
        public string CenterMessage { get; set; }

        [Reactive]
        public string ErrorMessage { get; set; }

        [Reactive]
        public ErrorType LastErrorType { get; set; }

        public RoutingState Router { get; }

        [Reactive]
        public string ServerStatusMessage { get; set; }

        [Reactive]
        public string StatusMessage { get; set; }
        #endregion

        #region Methods
        private void LoadIsBossMode()
        {
            try
            {
                if (AppSettings.IsBoss)
                {
                    Router.Navigate.Execute(new BossViewModel(this));
                }
                else
                {
                    Router.Navigate.Execute(new SubordinateViewModel(this));
                }
            }
            catch (Exception)
            {
                LastErrorType = ErrorType.System;
                ErrorMessage = "Can't load IsBoss mode";
            }
        }

        private async Task TryPing()
        {
            try
            {
                await _pingService.PingAsync();
                ServerStatusMessage = ConnectionStatus.ONLINE.ToLower();
                if (LastErrorType == ErrorType.Server)
                {
                    LastErrorType = ErrorType.No;
                    ErrorMessage = null;
                }
            }
            catch (Exception ex)
            {
                ServerStatusMessage = ConnectionStatus.OFFLINE.ToLower();
                LastErrorType = ErrorType.Server;
                ErrorMessage = ex.Message;
            }
        }
        #endregion
    }
}