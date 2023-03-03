using DevExpress.Data.Extensions;
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
    public class StampingFontChangedViewModel : ViewModelBase
    {


        public ObservableCollection<StampingTypeModel> StampingTypeVMObservableCollection { get; set; } = new ObservableCollection<StampingTypeModel>();

        public ObservableCollection<StampingTypeModel> UnusedStampingTypeVMObservableCollection { get; set; } = new ObservableCollection<StampingTypeModel>();
        /// <summary>
        /// 鋼印機上的字模
        /// </summary>
        public StampingTypeModel StampingFontSelected { get; set; }
        /// <summary>
        /// 被新建出來還沒放上去的字模/被換下來的字模
        /// </summary>
        public StampingTypeModel UnusedStampingFontSelected { get; set; }



        public RelayCommand StampingFontReplaceCommand
        {
            get => new RelayCommand(() =>
            {
                if (StampingFontSelected != null && UnusedStampingFontSelected != null)
                {
                    var FontString = StampingFontSelected.StampingTypeString;
                    var FontStringNumber = StampingFontSelected.StampingTypeNumber;
                    var FontStringUseCount = StampingFontSelected.StampingTypeUseCount;

                    var UnusedFontString = UnusedStampingFontSelected.StampingTypeString;
                    var UnusedFontStringNumber = UnusedStampingFontSelected.StampingTypeNumber;
                    var UnusedFontStringUseCount = UnusedStampingFontSelected.StampingTypeUseCount;

                    var ST_index = StampingTypeVMObservableCollection.FindIndex(x => x == StampingFontSelected);
                    var UST_index = UnusedStampingTypeVMObservableCollection.FindIndex(x => x == UnusedStampingFontSelected);

                    StampingTypeVMObservableCollection[ST_index] = new StampingTypeModel()
                    {
                        StampingTypeNumber = FontStringNumber,
                        StampingTypeString = UnusedFontString,
                        StampingTypeUseCount = UnusedFontStringUseCount
                    };

                    UnusedStampingTypeVMObservableCollection[UST_index] = new StampingTypeModel()
                    {
                        StampingTypeNumber = UnusedFontStringNumber,
                        StampingTypeString = FontString,
                        StampingTypeUseCount = FontStringUseCount
                    };

                    // UnusedStampingTypeVMObservableCollection[UST_index] = new StampingTypeModel() { StampingTypeNumber=  };
                    //
                }





            });
        }

        private string _newUnnsedStampingFontString;
        public string NewUnnsedStampingFontString
        {
            get { return _newUnnsedStampingFontString; }
            set { _newUnnsedStampingFontString = value; OnPropertyChanged(nameof(NewUnnsedStampingFontString)); }
        }
        public ICommand UnusedStampingFontAddCommand
        {
            get => new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(NewUnnsedStampingFontString))
                {
                    UnusedStampingTypeVMObservableCollection.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeString = NewUnnsedStampingFontString,
                        StampingTypeUseCount = 0
                    });
                }
            });
        }

        public RelayCommand UnusedStampingFontDelCommand
        {
            get => new RelayCommand(() =>
            {
                if (UnusedStampingFontSelected != null)
                    UnusedStampingTypeVMObservableCollection.Remove(UnusedStampingFontSelected);


            });
        }


    }
}
