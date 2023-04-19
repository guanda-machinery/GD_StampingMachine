using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class StampingTypeViewModel : BaseViewModelWithLog
    {
        private StampingTypeModel StampingType;

        public StampingTypeViewModel()
        {
            StampingType = new StampingTypeModel();
        }

        /// <summary>
        /// 鋼印文字
        /// </summary>
        public string StampingTypeString
        {
            get => StampingType.StampingTypeString;
            set
            {
                StampingType.StampingTypeString = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// No編號
        /// </summary>
        public int StampingTypeNumber
        {
            get => StampingType.StampingTypeNumber;
            set
            {
                StampingType.StampingTypeNumber = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 使用次數
        /// </summary>
        public int StampingTypeUseCount
        {
            get => StampingType.StampingTypeUseCount;
            set
            {
                StampingType.StampingTypeUseCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsNewAddStamping
        {
            get => StampingType.IsNewAddStamping;
            set
            {
                StampingType.IsNewAddStamping = value;
                OnPropertyChanged();
            }
        }


        private bool _stampingIsUsing = false;
        public bool StampingIsUsing
        {
            get => _stampingIsUsing;
            set
            {
                _stampingIsUsing = value;
                OnPropertyChanged(nameof(StampingIsUsing));
            }
        }




    }
}
