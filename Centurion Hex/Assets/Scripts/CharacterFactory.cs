using Assets.Scripts.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterFactory
{
    public static Character CreateCharacter( Character.CharacterType characterType )
    {
        switch( characterType )
        {
            case Character.CharacterType.ctScout:
                return new Scout();
            case Character.CharacterType.ctSurveyor:
                return new Surveyor();
        }
        return null;
    }
}
