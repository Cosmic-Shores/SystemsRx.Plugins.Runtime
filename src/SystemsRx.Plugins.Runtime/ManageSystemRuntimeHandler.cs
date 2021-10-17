using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Attributes;
using SystemsRx.Exceptions;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.Systems;

namespace SystemsRx.Plugins.Runtime {
    [Priority(10)]
    sealed class ManageSystemRuntimeHandler : IConventionalSystemHandler {
        private readonly IList<ISystem> _systems = new List<ISystem>();
        private readonly ISystemRuntime _runtime;
        private readonly IList<IConventionalSystemRuntimeHandler> _conventionalSystemRuntimeHandlers;

        public IEnumerable<ISystem> Systems => _systems;

        public ManageSystemRuntimeHandler(ISystemRuntime runtime, IEnumerable<IConventionalSystemRuntimeHandler> conventionalSystemRuntimeHandlers) {
            _runtime = runtime;
            _conventionalSystemRuntimeHandlers = conventionalSystemRuntimeHandlers.ToArray();
        }

        public bool CanHandleSystem(ISystem system) => true;

        public void SetupSystem(ISystem system) {
            var systemRuntimeHandlersandlers = _conventionalSystemRuntimeHandlers.Where(x => x.CanHandleSystem(system)).ToArray();
            switch (systemRuntimeHandlersandlers.Length) {
                case 0:
                    _runtime.StartSystem(system);
                    break;

                case 1:
                    if (_systems.Contains(system))
                        throw new SystemAlreadyRegisteredException(system);
                    _systems.Add(system);
                    systemRuntimeHandlersandlers[0].SetupSystem(_runtime, system);
                    break;

                default:
                    throw CreateTooManyEnableSystemHandlerException(system, systemRuntimeHandlersandlers);
            }
        }

        public void DestroySystem(ISystem system) {
            var systemRuntimeHandlersandlers = _conventionalSystemRuntimeHandlers.Where(x => x.CanHandleSystem(system)).ToArray();
            switch (systemRuntimeHandlersandlers.Length) {
                case 0:
                    _runtime.DestroySystem(system);
                    break;

                case 1:
                    systemRuntimeHandlersandlers[0].DestroySystem(_runtime, system);
                    _systems.Remove(system);
                    break;

                default:
                    throw CreateTooManyEnableSystemHandlerException(system, systemRuntimeHandlersandlers);
            }
        }

        public void Dispose() {
            foreach (var system in _systems)
                _conventionalSystemRuntimeHandlers.Single(x => x.CanHandleSystem(system)).DestroySystem(_runtime, system);
            _systems.Clear();
            _conventionalSystemRuntimeHandlers.DisposeAll();
            if (_runtime is IDisposable disposable)
                disposable.Dispose();
        }

        private static InvalidOperationException CreateTooManyEnableSystemHandlerException(ISystem system, IConventionalSystemRuntimeHandler[] systemRuntimeHandlersandlers) {
            return new InvalidOperationException($"More than one {nameof(IConventionalSystemRuntimeHandler)} can handle the {nameof(SystemState)} for the System '{system.GetType()}'. Please ensure there never more than one way of specifying the enabled state for one system. Contenders are: {string.Join(Environment.NewLine, systemRuntimeHandlersandlers.Select(x => x.GetType().ToString()))}");
        }
    }
}
