using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopTool.SignalR
{
    public class SignalRClient : IDisposable
    {

        private HubConnection _connection;

        public async Task StartAsync()
        {
            _connection = new HubConnectionBuilder()
     .WithUrl("http://localhost:7071/api") // this URL must match your Azure Function endpoint
     .WithAutomaticReconnect()
     .Build();

            _connection.On<string>("NotifyUpload", message =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    File.AppendAllText("signalr.log", $"[{DateTime.Now}] Received: {message}{Environment.NewLine}");
                    ToastService.ShowToast(message); // or MessageBox.Show(message) for quick debug
                });
            });

            await _connection.StartAsync();
            //File.AppendAllText("signalr.log", $"[{DateTime.Now}] ✅ SignalR Connected.{Environment.NewLine}");
        }

        public async Task StopAsync()
        {
            if (_connection != null)
                await _connection.StopAsync();
        }

        public void Dispose()
        {
            _connection?.DisposeAsync().AsTask().Wait(); // Ensure cleanup
        }
    }
}
