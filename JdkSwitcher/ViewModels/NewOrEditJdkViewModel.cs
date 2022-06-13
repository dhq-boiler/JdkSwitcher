using JdkSwitcher.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace JdkSwitcher.ViewModels
{
    public class NewOrEditJdkViewModel : BindableBase, IDialogAware, IDisposable
    {
        private CompositeDisposable compositeDisposable = new CompositeDisposable();
        private string _title;
        private bool disposedValue;

        public string Title => "";

        public event Action<IDialogResult> RequestClose;

        public ReactiveCommand OkCommand { get; } = new ReactiveCommand();
        public ReactiveCommand CancelCommand { get; } = new ReactiveCommand();

        private string mode;
        public ReactivePropertySlim<Guid> ID { get; set; } = new ReactivePropertySlim<Guid>();
        public ReactivePropertySlim<string> Name { get; set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> JavaHome { get; set; } = new ReactivePropertySlim<string>();

        public NewOrEditJdkViewModel()
        {
            OkCommand.Subscribe(() =>
            {
                switch (mode)
                {
                    case "NEW":
                        var jdk = new Jdk();
                        jdk.ID = Guid.NewGuid();
                        jdk.Name = Name.Value;
                        jdk.JavaHome = JavaHome.Value;
                        RequestClose.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters() { { "Jdk", jdk } }));
                        break;
                    case "EDIT":
                        jdk = new Jdk();
                        jdk.ID = ID.Value;
                        jdk.Name = Name.Value;
                        jdk.JavaHome = JavaHome.Value;
                        RequestClose.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters() { { "Jdk", jdk } }));
                        break;
                }
            })
            .AddTo(compositeDisposable);
            CancelCommand.Subscribe(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
            })
            .AddTo(compositeDisposable);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            mode = parameters.GetValue<string>("Mode");
            switch (mode)
            {
                case "NEW":
                    _title = "JDKの新規インポート";
                    break;
                case "EDIT":
                    _title = "JDKの編集";
                    var jdk = parameters.GetValue<Jdk>("Jdk");
                    ID.Value = jdk.ID;
                    Name.Value = jdk.Name;
                    JavaHome.Value = jdk.JavaHome;
                    break;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    compositeDisposable.Dispose();
                }

                compositeDisposable = null;
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
