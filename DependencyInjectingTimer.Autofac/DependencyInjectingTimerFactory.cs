using System;
using System.Threading.Tasks;
using Autofac;
using log4net;

namespace DependencyInjectingTimer.Autofac {
    public class DependencyInjectingTimerFactory {
        private readonly Func<IContainer> _container;
        private readonly ILog _log;

        public DependencyInjectingTimerFactory(Func<IContainer> container, ILog log) {
            _container = container;
            _log = log;
        }

        public async void StartAndRepeatHours(int interval, Action<ILifetimeScope> action,
                                              Action<ContainerBuilder> configurationAction = null) {
            var milliseconds = interval.FromHoursToMilliseconds().AssertGreaterThanZero();
            await ExecuteEvery(milliseconds, action, configurationAction);
        }

        public async void StartAndRepeatSeconds(int interval, Action<ILifetimeScope> action,
                                                Action<ContainerBuilder> configurationAction = null) {
            var milliseconds = interval.FromSecondsToMilliseconds().AssertGreaterThanZero();
            await ExecuteEvery(milliseconds, action, configurationAction);
        }

        private async Task ExecuteEvery(int milliseconds, Action<ILifetimeScope> action,
                                        Action<ContainerBuilder> configurationAction) {
            var taskNotToAwait = Task.Run(() => ExecuteInChildScope(action, configurationAction));

            await Task.Delay(TimeSpan.FromMilliseconds(milliseconds))
                      .ContinueWith(t => ExecuteEvery(milliseconds, action, configurationAction));
        }

        private void ExecuteInChildScope(Action<ILifetimeScope> action, Action<ContainerBuilder> configurationAction) {
            _log.Debug($"Timer action started @ {DateTime.Now.ToString("T")}");
            try {
                using (var lifetime = _container().BeginLifetimeScope(configurationAction ?? (c => { }))
                    )
                    action(lifetime);
                _log.Debug($"Timer action finished @ {DateTime.Now.ToString("T")}");
            }
            catch (Exception e) {
                _log.Error($"Timer action faulted, exception: {e}");
            }
        }
    }
}