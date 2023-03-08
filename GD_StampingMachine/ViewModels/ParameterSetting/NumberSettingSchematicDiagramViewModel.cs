using GD_StampingMachine.Enum;
using System.Windows;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class NumberSettingSchematicDiagramViewModel : ViewModelBase
    {

        /// <summary>
        /// 目前模式
        /// </summary>
        //public NumberSettingModeEnum NumberSettingMode { get; set; }


        private int _sequenceCount = 1;
        /// <summary>
        /// 單排數量
        /// </summary>
        public int SequenceCount
        {
            get
            {
                return _sequenceCount;
            }
            set
            {
                SheetColumn_1_DefinitonWidth = 0;
                _sequenceCount = value;
                OnPropertyChanged(nameof(SequenceCount));
            }
        }

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum SpecialSequence { get; set; }
















    }
}
