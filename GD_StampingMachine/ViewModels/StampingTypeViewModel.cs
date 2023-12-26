using GD_StampingMachine.GD_Model;

namespace GD_StampingMachine.ViewModels
{
    public class StampingTypeViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override bool Equals(object obj)
        {
            if (obj is StampingTypeViewModel StampObj)
            {
                var e1 = this.StampingTypeString == StampObj.StampingTypeString;
                var e2 = this.StampingTypeNumber == StampObj.StampingTypeNumber;
                var e3 = this.StampingTypeUseCount == StampObj.StampingTypeUseCount;
                return e1 && e2 && e3;
            }
            else
                return false;
        }

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingTypeViewModel");


        public readonly StampingTypeModel StampingType;

        public StampingTypeViewModel(StampingTypeModel _StampingType)
        {
            StampingType = _StampingType;
        }

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
                OnPropertyChanged();
            }
        }






    }
}
