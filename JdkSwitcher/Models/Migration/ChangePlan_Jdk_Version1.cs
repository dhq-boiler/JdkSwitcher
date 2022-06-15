using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;

namespace JdkSwitcher.Models.Migration
{
    internal class ChangePlan_Jdk_Version1 : ChangePlanByTable<Jdk, Version1>
    {
        public override void CreateTable(IConnection connection)
        {
            var dao = new JdkDao(typeof(Version1));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new JdkDao(typeof(Version1));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            var dao = new JdkDao(typeof(Version1));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
            dao.UpgradeTable(new VersionChangeUnit(typeof(VersionOrigin), TargetVersion.GetType()));
            ++ModifiedCount;
        }
    }
}