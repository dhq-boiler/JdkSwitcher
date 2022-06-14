using Homura.ORM;
using JdkSwitcher.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdkSwitcher.Models
{
    internal class JdkDao : Dao<Jdk>
    {
        public JdkDao()
            : base()
        { }

        public JdkDao(Type entityVersionType)
            : base(entityVersionType)
        { }

        protected override Jdk ToEntity(IDataRecord reader)
        {
            return new Jdk()
            {
                ID = reader.SafeGetGuid(nameof(Jdk.ID), Table),
                Name = reader.SafeGetString(nameof(Jdk.Name), Table),
                JavaHome = reader.SafeGetString(nameof(Jdk.JavaHome), Table),
                EnvironmentVariableTarget = reader.SafeGetEnum<EnvironmentVariableTarget>(nameof(Jdk.EnvironmentVariableTarget), Table),
            };
        }
    }
}
