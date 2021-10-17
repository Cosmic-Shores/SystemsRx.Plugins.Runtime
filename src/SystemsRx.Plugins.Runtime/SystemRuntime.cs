using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Executor;
using SystemsRx.Systems;

namespace SystemsRx.Plugins.Runtime {
    sealed class SystemRuntime : ISystemRuntime, IDisposable {
        private readonly IDictionary<ISystem, SystemState> _systemStateDict = new Dictionary<ISystem, SystemState>();
        private readonly ISystemExecutor _systemExecutor;
        private bool _isDisposed;

        public IEnumerable<ISystem> Systems => _systemStateDict.Where(x => x.Value == SystemState.Running).Select(x => x.Key);

        public SystemRuntime(ISystemExecutor systemExecutor) {
            _systemExecutor = systemExecutor;
        }

        public SystemState GetSystemState(ISystem system) {
            return _systemStateDict.TryGetValue(system, out var systemState)
                ? systemState
                : SystemState.NotYetRun;
        }

        public void StartSystem(ISystem system) {
            lock (_systemStateDict) {
                if (_isDisposed)
                    throw new ObjectDisposedException(nameof(SystemRuntime));
                if (_systemStateDict.TryGetValue(system, out var systemState))
                    throw new ArgumentException($"The system '{system.GetType()}' can't be started as it already has the state {systemState}", nameof(system));
                _systemStateDict.Add(system, SystemState.Running);
                _systemExecutor.AddSystem(system);
            }
        }

        public void DestroySystem(ISystem system) {
            lock (_systemStateDict) {
                if (_isDisposed) { return; }
                if (!_systemStateDict.TryGetValue(system, out var systemState))
                    throw new ArgumentException($"The system '{system.GetType()}' can't be destroyed as it's hasn't even been started!", nameof(system));
                if (systemState == SystemState.Destroyed)
                    throw new ArgumentException($"The system '{system.GetType()}' was already destroyed!", nameof(system));
                DestroySystemInternal(system);
            }
        }

        public void Dispose() {
            lock (_systemStateDict) {
                if (_isDisposed)
                    return;
                _isDisposed = true;
                foreach (var pair in _systemStateDict)
                    if (pair.Value == SystemState.Running)
                        DestroySystemInternal(pair.Key);
                if (_systemExecutor is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        private void DestroySystemInternal(ISystem system) {
            _systemExecutor.RemoveSystem(system);
            _systemStateDict[system] = SystemState.Destroyed;
        }
    }
}
