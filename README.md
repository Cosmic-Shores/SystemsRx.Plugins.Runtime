# About
A [SystemsRx](https://github.com/EcsRx/SystemsRx) Plugin that provides a base notion for implementing a plugin to control when a system is started and stopped with ease.

![Build Status](https://github.com/Cosmic-Shores/SystemsRx.Plugins.Runtime/actions/workflows/publish.yml/badge.svg)
[![NuGet version](https://badgen.net/nuget/v/SystemsRx.Plugins.Runtime/latest)](https://nuget.org/packages/SystemsRx.Plugins.Runtime)
[![License](https://badgen.net/github/license/Naereen/Strapdown.js)](https://github.com/Cosmic-Shores/SystemsRx.Plugins.Runtime/blob/main/LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

## Building a plugin using this
- Create a class implementing IConventionalSystemRuntimeHandler
- Create a class implementing ISystemsRxPlugin and call `container.Bind<IConventionalSystemRuntimeHandler, MyHandler>()` in SetupDependencies()
- To use your new plugin make sure to load both the SystemsRxPlugin class you created above as well as SystemRuntimePlugin
