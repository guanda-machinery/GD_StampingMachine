using System.ComponentModel;

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
