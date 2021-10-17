using System;
using System.Collections.Generic;
using System.Reflection;
using SystemsRx.Attributes;
using SystemsRx.Extensions;
using SystemsRx.Plugins.Runtime;
using SystemsRx.Systems;

namespace EcsRx.Plugins.Bootstrap {
    [Priority(10)]
    public sealed class ManualStartupSystemHandler : IConventionalSystemRuntimeHandler {
        private readonly IDictionary<ISystem, IDisposable> _systems = new Dictionary<ISystem, IDisposable>();
        private readonly MethodInfo _createSystemInstanceMethod;

        public ManualStartupSystemHandler() {
            _createSystemInstanceMethod = GetType().GetMethod(nameof(CreateSystemInstance), BindingFlags.Public | BindingFlags.Static);
        }

        public bool CanHandleSystem(ISystem system) => system.MatchesSystemTypeWithGeneric(typeof(IManualStartupSystem<>));

        public void SetupSystem(ISystemRuntime runtime, ISystem system) {
            var genericDataType = system.GetGenericDataType(typeof(IManualStartupSystem<>));
            var genericMethod = _createSystemInstanceMethod.MakeGenericMethod(genericDataType);
            _systems.Add(system, (IDisposable)genericMethod.Invoke(null, new object[] { runtime, system }));
        }

        public void DestroySystem(ISystemRuntime runtime, ISystem system) {
            if (!_systems.TryGetValue(system, out var disposable)) {
                throw new ArgumentException($"The system '{system.GetType()}' was not setup. It cannot be destroyed.", nameof(system));
            }
            disposable.Dispose();
            _systems.Remove(system);
        }

        public void Dispose() {
            _systems.Values.DisposeAll();
            _systems.Clear();
        }

        public static IDisposable CreateSystemInstance<T>(ISystemRuntime runtime, IManualStartupSystem<T> system) {
            var instance = new SystemInstance<T>(runtime, system);
            var isReady = system.Ready();
            if (isReady == null)
                throw new InvalidOperationException($"The {nameof(system.Ready)}() method of the system '{system.GetType()}' returned null.");
            instance.Init(isReady.Subscribe(instance));
            return instance;
        }

        private class SystemInstance<T> : IObserver<T>, IDisposable {
            private readonly ISystemRuntime _runtime;
            private readonly ISystem _system;
            private IDisposable _isReadySub;
            private bool _completedReadySub;
            private bool _isRunning;

            public SystemInstance(ISystemRuntime runtime, ISystem system) {
                _runtime = runtime;
                _system = system;
            }

            public void Init(IDisposable isReadySub) {
                if (_completedReadySub)
                    isReadySub.Dispose();
                else
                    _isReadySub = isReadySub;
            }

            public void OnCompleted() {
                if (_completedReadySub)
                    return;
                _completedReadySub = true;
                DisposeSub();
            }

            public void OnError(Exception error) {
                OnCompleted();
                throw error;
            }

            public void OnNext(T _) {
                if (_completedReadySub)
                    return;
                _completedReadySub = true;
                _isRunning = true;
                _runtime.StartSystem(_system);
                DisposeSub();
            }

            public void Dispose() {
                OnCompleted();
                if (_isRunning) {
                    _runtime.DestroySystem(_system);
                    _isRunning = false;
                }
            }

            private void DisposeSub() {
                if (_isReadySub == null)
                    return;
                _isReadySub.Dispose();
                _isReadySub = null;
            }
        }
    }
}
