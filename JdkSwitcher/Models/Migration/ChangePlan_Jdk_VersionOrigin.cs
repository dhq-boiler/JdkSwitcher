using Homura.ORM;
using Homura.ORM.Mapping;
using Homura.ORM.Migration;

namespace JdkSwitcher.Models.Migration
{
    internal class ChangePlan_Jdk_VersionOrigin : ChangePlanByTable<Jdk, VersionOrigin>
    {
        public override void CreateTable(IConnection connection)
        {
            var dao = new JdkDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.CreateTableIfNotExists();
            ++ModifiedCount;
            dao.CreateIndexIfNotExists();
            ++ModifiedCount;
        }

        public override void DropTable(IConnection connection)
        {
            var dao = new JdkDao(typeof(VersionOrigin));
            dao.CurrentConnection = connection;
            dao.DropTable();
            ++ModifiedCount;
        }

        public override void UpgradeToTargetVersion(IConnection connection)
        {
            CreateTable(connection);
        }
    }
}