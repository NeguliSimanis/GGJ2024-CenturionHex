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
            Class = BuildingClass.bcSenate;
            Type = BuildingType.btSenate;

            Name = "Senate";
            Description = "All allied players gain two gold per turn. Unit or building damaging this is killed";
            Health = InitialHealth = 3;
        }

        override public void onAttack()
        {
            //if shrine is not present
            Health--;
            //kill attacker
        }
    }
}
