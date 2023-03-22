using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public interface NumberSettingInterface
    {

        public ObservableCollection<int> SequenceCountCollection{get;}
        /// <summary>
        /// 單排數量
        /// </summary>
        public int? SequenceCountComboBoxSelectValue { get; set; }

        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum? SpecialSequenceComboBoxSelectValue { get; set; }

        /// <summary>
        /// 這是鐵牌上要打的位置
        /// </summary>
        public ObservableCollection<int> PlateNumberList { get;  }

        public HorizontalAlignEnum? HorizontalAlignEnumComboBoxSelectValue { get; set; }
        public VerticalAlignEnum? VerticalAlignEnumComboBoxSelectValue { get; set; }


        public Array HorizontalAlignmentCollection { get;  }

        public Array VerticalAlignmentCollection { get;  }

        public ICommand LoadModeCommand { get; }
        public ICommand RecoverSettingCommand { get; }



        public ICommand SaveSettingCommand { get; }
        public ICommand DeleteSettingCommand { get; }

    }
}
