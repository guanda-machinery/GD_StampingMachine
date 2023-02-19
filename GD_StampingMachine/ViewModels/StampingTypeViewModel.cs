using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
   public class StampingTypeViewModel:ViewModelBase
    {
        public StampingTypeViewModel()
        {

        }

        public StampingTypeViewModel(StampingTypeModel StampingType)
        {
            StampingTypeString = StampingType.StampingTypeString;
            StampingTypeNumber = StampingType.StampingTypeNumber.ToString();
            StampingTypeUseCount = StampingType.StampingTypeUseCount.ToString();
        }


        private string _stampingTypeString = null;
        /// <summary>
        /// 鋼印文字
        /// </summary>
        public string StampingTypeString 
        {
            get
            {
                if (string.IsNullOrEmpty(_stampingTypeString))
                    return "NaN";
                return _stampingTypeString;
            }
            set
            {
                _stampingTypeString = value;
            }
        }

        private string _stampingTypeNumber;

        /// <summary>
        /// No編號
        /// </summary>
        public string StampingTypeNumber
        {
            get
            {
                if (string.IsNullOrEmpty(_stampingTypeNumber))
                    return "NaN";
                return _stampingTypeNumber;
            }
            set
            {
                _stampingTypeNumber = value;
            }
        }


        private string _stampingTypeUseCount;
        /// <summary>
        /// 使用次數
        /// </summary>
        public string StampingTypeUseCount
        {
            get
            {
                if (_stampingTypeUseCount == null)
                    return "NaN";
                return _stampingTypeUseCount;
            }
            set
            {
                _stampingTypeUseCount = value;
            }
        }

    }
}
