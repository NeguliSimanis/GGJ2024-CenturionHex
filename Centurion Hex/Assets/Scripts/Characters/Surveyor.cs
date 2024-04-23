using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Characters
{
    public class Surveyor : Character
    {
        public Surveyor()
        {
            Name = "Surveyor";
            Price = 1;
            AttackDamage = 0;
            StepsPerTurn = 3;
            InitialHealth = 1;
            Health = InitialHealth;
            isWarUnit = false;
            type = CharacterType.ctSurveyor;
        }
    }
}
