using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValheimTweaking
{
    class Helper
    {
        public static Character getPlayerCharacter() {
            foreach (Character character in Character.GetAllCharacters()) {
                if (character.IsPlayer()) {
                    return character;
                }
            }
            return null;
        }

        public static int max(int a, int b) {
            return a > b ? a : b;
        }
    }
}
