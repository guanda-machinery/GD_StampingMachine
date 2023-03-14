using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Enum
{
    public enum CharactersFormEnum
    {
        [Description("A~Z")]
        UpperCaseAlphabet, 
        [Description("0~9")]
        CardinalNumber, 
        [Description("a~z")]
        LowerCaseAlphabet
    }
}
