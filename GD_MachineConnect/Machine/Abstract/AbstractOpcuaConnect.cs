using GD_CommonLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GD_MachineConnect.Machine.Interfaces
{
    public abstract class AbstractOpcuaConnect : IOpcuaConnect
    {
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                IsConnectedChanged?.Invoke(this, new ValueChangedEventArgs<bool>(_isConnected, value));
                _isConnected = value;
            }
        }
        public event EventHandler<ValueChangedEventArgs<bool>> IsConnectedChanged;


        public abstract Task<bool> ConnectAsync(string hostPath, string? user = null, string? password = null);
        public abstract void Disconnect();

        public abstract Task DisconnectAsync();
        public abstract Task<T> ReadNodeAsync<T>(string NodeID);

        public abstract Task<IEnumerable<object>> ReadNodesAsync(IEnumerable<string> NodeTrees);

        public abstract Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval = 10, bool checkDuplicates = true);
        public abstract Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval, bool checkDuplicates)> nodeList);
        public abstract Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, EventHandler<ValueChangedEventArgs<T>> updateHandler, int samplingInterval = 10, bool checkDuplicates = true);
        public abstract Task<IEnumerable<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, EventHandler<ValueChangedEventArgs<T>> updateHandler, int samplingInterval, bool checkDuplicates)> nodeList);

        public abstract Task<bool> UnsubscribeNodeAsync(string NodeID, int samplingInterval);
        public abstract Task<bool> WriteNodeAsync<T>(string NodeTreeString, T WriteValue);


        public abstract Task<IEnumerable<bool>> WriteNodesAsync(Dictionary<string, object> NodeTrees);
    }
}
