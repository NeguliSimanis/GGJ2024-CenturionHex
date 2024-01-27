using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Characters
{
    public class Scout : Character
    {
        public Scout()
        {
            Name = "Scout";
            InitialHealth = 2;
            Health = InitialHealth;
            Price = 2;
            AttackDamage = 1;
            StepsPerTurn = 3;
            isWarUnit = true;
        }

        public override void DoAttack()
        {
            Health = 0;//dies on attack
        }
    }
}
