using GD_CommonLibrary.Extensions;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public abstract class PlateSettingPageBaseViewModel : ParameterSettingBaseViewModel
    {

        public abstract SheetStampingTypeFormEnum SheetStampingTypeForm{get;}

        [JsonIgnore]
        public Array HorizontalAlignmentCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(HorizontalAlignEnum));
            }
        }
        [JsonIgnore]
        public Array VerticalAlignmentCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(VerticalAlignEnum));
            }
        }


        [JsonIgnore]
        public int PlateNumberListMax
        { 
            get 
            {
                if (SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                    return 6;
                if (SheetStampingTypeForm == SheetStampingTypeFormEnum.NormalSheetStamping)
                    return 8;

                return 0;
            }
        }
        [JsonIgnore]
        public ObservableCollection<int> SequenceCountCollection
        {
            get
            {
                var CountCollection = new ObservableCollection<int>();
                for (int i = 1; i <= PlateNumberListMax; i++)
                {
                    CountCollection.Add(i);
                }
                return CountCollection;
            }
        }

        [JsonIgnore]
        public ObservableCollection<SpecialSequenceEnum> SpecialSequenceCollection
        {
            get
            {
                var EnumList = new ObservableCollection<SpecialSequenceEnum>();
                foreach (SpecialSequenceEnum EachEnum in System.Enum.GetValues(typeof(SpecialSequenceEnum)))
                {
                    EnumList.Add(EachEnum);
                }

                return EnumList;
            }
        }




    }


    
}
