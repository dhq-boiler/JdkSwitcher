using Homura.Core;
using Homura.ORM;
using Homura.ORM.Setup;
using JdkSwitcher.Models;
using JdkSwitcher.Models.Migration;
using JdkSwitcher.Views;
using NLog;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JdkSwitcher.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private bool disposedValue;
        private CompositeDisposable compositeDisposable = new CompositeDisposable();

        public ReactiveCollection<Jdk> Jdks { get; } = new ReactiveCollection<Jdk>();

        public ReactiveCommand ImportJdkCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<System.Windows.Input.MouseButtonEventArgs> ListViewDoubleClicked { get; } = new ReactiveProperty<System.Windows.Input.MouseButtonEventArgs>();


        public ReactiveCommand<Jdk> SwitchCommand { get; } = new ReactiveCommand<Jdk>();
        public ReactiveCommand<Jdk> EditCommand { get; } = new ReactiveCommand<Jdk>();

        public MainWindowViewModel(IDialogService dialogService)
        {
            ConnectionManager.SetDefaultConnection($"DataSource=jdkswitcher.db", typeof(SQLiteConnection));

            var dvManager = new DataVersionManager();
            dvManager.CurrentConnection = ConnectionManager.DefaultConnection;
            dvManager.Mode = VersioningStrategy.ByTick;
            dvManager.RegisterChangePlan(new ChangePlan_VersionOrigin());
            dvManager.RegisterChangePlan(new ChangePlan_Version1());
            dvManager.FinishedToUpgradeTo += DvManager_FinishedToUpgradeTo;
            dvManager.UpgradeToTargetVersion();

            ImportJdkCommand.Subscribe(() =>
            {
                IDialogResult dialogResult = null;
                dialogService.ShowDialog(nameof(NewOrEditJdk), new DialogParameters { { "Mode", "NEW" } }, (result) => dialogResult = result);
                if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                {
                    var inserting = dialogResult.Parameters.GetValue<Jdk>("Jdk");
                    var dao = new JdkDao();
                    dao.Insert(inserting);
                    Jdks.Add(inserting);
                }
            })
            .AddTo(compositeDisposable);
            ListViewDoubleClicked
                .Select(p => new { EventArgs = p, ViewModel = (p?.Source as System.Windows.FrameworkElement)?.DataContext as Jdk })
                .Where(p => p.ViewModel != null)
                .Do(p => p.EventArgs.Handled = true)    //HandledをtrueにするためにDoを挟む
                .Select(p => p.ViewModel)
                .Subscribe(p =>
                {
                    IDialogResult dialogResult = null;
                    dialogService.ShowDialog(nameof(NewOrEditJdk), new DialogParameters { { "Mode", "EDIT" }, { "Jdk", p } }, (result) => dialogResult = result);
                    if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                    {
                        var updated = dialogResult.Parameters.GetValue<Jdk>("Jdk");
                        p.Name = updated.Name;
                        p.JavaHome = updated.JavaHome;
                        p.EnvironmentVariableTarget = updated.EnvironmentVariableTarget;
                        var dao = new JdkDao();
                        dao.Update(p);
                    }
                })
                .AddTo(compositeDisposable);
            SwitchCommand.Subscribe(jdk =>
            {
                jdk.A();
            })
            .AddTo(compositeDisposable);
            EditCommand.Subscribe(jdk =>
            {
                IDialogResult dialogResult = null;
                dialogService.ShowDialog(nameof(NewOrEditJdk), new DialogParameters { { "Mode", "EDIT" }, { "Jdk", jdk } }, (result) => dialogResult = result);
                if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                {
                    var updated = dialogResult.Parameters.GetValue<Jdk>("Jdk");
                    jdk.Name = updated.Name;
                    jdk.JavaHome = updated.JavaHome;
                    jdk.EnvironmentVariableTarget = updated.EnvironmentVariableTarget;
                    var dao = new JdkDao();
                    dao.Update(jdk);
                }
            })
            .AddTo(compositeDisposable);

            var dao = new JdkDao();
            var jdks = dao.FindAll();
            Jdks.AddRange(jdks);
        }

        //private void A()
        //{
        //    System.Environment.SetEnvironmentVariable("JAVA_HOME", JavaHome);
        //}

        private void DvManager_FinishedToUpgradeTo(object sender, ModifiedEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Info($"Heavy Modifying AppDB Count : {e.ModifiedCount}");

            if (e.ModifiedCount > 0)
            {
                SQLiteBaseDao<Dummy>.Vacuum(ConnectionManager.DefaultConnection);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~MainWindowViewModel()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
