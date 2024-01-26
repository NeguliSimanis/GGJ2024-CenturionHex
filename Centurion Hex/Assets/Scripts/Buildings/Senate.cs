using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Buildings
{
    public class Senate : Building
    {
        public Senate() {
            Type = BuildingType.btSenate;
            Name = "Senate";
        }
    }
}
