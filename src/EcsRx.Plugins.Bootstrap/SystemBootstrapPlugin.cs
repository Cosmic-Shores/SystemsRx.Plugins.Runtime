using System;
using System.Collections.Generic;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Plugins.Runtime;
using SystemsRx.Systems;

namespace EcsRx.Plugins.Bootstrap {
    /// <summary>
    /// Requires the SystemRuntimePlugin to function.
    /// About: A SystemRx Plugin to delay the start of a system until it is ready.
    /// </summary>
    public sealed class SystemBootstrapPlugin : ISystemsRxPlugin {
        public string Name => "System Bootstrap Plugin";
        public Version Version => this.GetType().Assembly.GetName().Version;

        public void SetupDependencies(IDependencyContainer container) {
            container.Bind<IConventionalSystemRuntimeHandler, ManualStartupSystemHandler>();
        }

        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();
    }
}
