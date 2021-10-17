using System;
using System.Collections.Generic;
using SystemsRx.Executor;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;
using SystemsRx.Infrastructure.Extensions;

namespace SystemsRx.Plugins.Runtime {
    /// <summary>
    /// A Plugin that provides a base notion for implementing a plugin to control when a system is started and stopped with ease.
    /// </summary>
    public sealed class SystemRuntimePlugin : ISystemsRxPlugin {
        public string Name => "System Runtime Plugin";
        public Version Version => this.GetType().Assembly.GetName().Version;

        public void SetupDependencies(IDependencyContainer container) {
            container.Unbind<ISystemExecutor>();
            container.Bind<ISystemExecutor>(new BindingConfiguration { ToMethod = CreateSystemExecutor });
            container.Bind<ManageSystemRuntimeHandler>();
            container.Bind<ISystemRuntime, SystemRuntimeProxy>();
        }

        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();

        private object CreateSystemExecutor(IDependencyContainer container) => new SystemExecutor(new[] { container.Resolve<ManageSystemRuntimeHandler>() });

        private class SystemRuntimeProxy : ISystemRuntime, IDisposable {
            private readonly SystemRuntime _runtime;

            public SystemRuntimeProxy(IEnumerable<IConventionalSystemHandler> conventionalSystemHandlers) {
                _runtime = new SystemRuntime(new SystemExecutor(conventionalSystemHandlers));
            }

            public SystemState GetSystemState(ISystem system) => _runtime.GetSystemState(system);
            public void StartSystem(ISystem system) => _runtime.StartSystem(system);
            public void DestroySystem(ISystem system) => _runtime.DestroySystem(system);
            public void Dispose() => _runtime.Dispose();
        }
    }
}
