using GD_StampingMachine.GD_Enum;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public abstract class PlateSettingPageBaseViewModel : ParameterSettingBaseViewModel
    {

        public abstract SheetStampingTypeFormEnum SheetStampingTypeForm { get; }

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
        public abstract int PlateNumberListMax { get; }

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
