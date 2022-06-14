using Homura.ORM.Mapping;
using Homura.ORM.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdkSwitcher.Models.Migration
{
    internal class ChangePlan_Version1 : ChangePlanByVersion<Version1>
    {
        public override IEnumerable<IEntityVersionChangePlan> VersionChangePlanList
        {
            get
            {
                yield return new ChangePlan_Jdk_Version1();
            }
        }
    }
}
