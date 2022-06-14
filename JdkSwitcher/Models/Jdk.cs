using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homura.ORM.Mapping;
using System.Diagnostics;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace JdkSwitcher.Models
{
    [DefaultVersion(typeof(Version1))]
    public class Jdk : PkIdEntity, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        private string _name;
        private string _JavaHome;
        private bool disposedValue;
        private EnvironmentVariableTarget _EnvironmentVariableTarget;

        [Column("Name", "TEXT", 1)]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [Column("JavaHome", "TEXT", 2)]
        public string JavaHome
        {
            get { return _JavaHome; }
            set { SetProperty(ref _JavaHome, value); }
        }

        [Since(typeof(Version1))]
        [Column("EnvironmentVariableTarget", "INTEGER", 3)]
        public EnvironmentVariableTarget EnvironmentVariableTarget
        {
            get { return _EnvironmentVariableTarget; }
            set { SetProperty(ref _EnvironmentVariableTarget, value); }
        }

        public ReactiveCommand SwitchCommand { get; } = new ReactiveCommand();

        public Jdk()
        {
            SwitchCommand.Subscribe(() =>
            {
                A();
            })
            .AddTo(disposables);
        }

        public void A()
        {
            System.Environment.SetEnvironmentVariable("JAVA_HOME", JavaHome, EnvironmentVariableTarget);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }

                disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
