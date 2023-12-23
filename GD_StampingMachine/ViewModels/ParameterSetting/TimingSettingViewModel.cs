using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GD_CommonLibrary;
using GD_StampingMachine.Method;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json.Serialization;
using DevExpress.Mvvm.Native;
using System.Diagnostics;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    /// <summary>
    /// 計時設定
    /// </summary>
    public class TimingSettingViewModel : ParameterSettingBaseViewModel
    {
        StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TimingSettingViewModel");

  
        public TimingSettingViewModel(TimingSettingModel timingSetting)
        {
            this.TimingSetting = timingSetting;

            TimingControlVMCollection = new ObservableCollection<TimingControlViewModel>(timingSetting.TimingControlCollection?.Select(x => new TimingControlViewModel(x)));

            if (Debugger.IsAttached)
            {
                TimingControlVMCollection.Add(new TimingControlViewModel());
                TimingControlVMCollection.Add(new TimingControlViewModel());
                TimingControlVMCollection.Add(new TimingControlViewModel());

            }
        }

        //private TimingSettingModel _timingSetting;
        [JsonIgnore]
        public TimingSettingModel TimingSetting;



        //private List<TimingControlModel> _timingControlCollection = new();

        private ObservableCollection<TimingControlViewModel> _timingControlVMCollection;
        public ObservableCollection<TimingControlViewModel> TimingControlVMCollection
        {
            get 
            {
                if(_timingControlVMCollection == null)
                {
                    _timingControlVMCollection = new ObservableCollection<TimingControlViewModel>(TimingSetting.TimingControlCollection?.Select(x => new TimingControlViewModel(x)));
                }

                return _timingControlVMCollection; 
            }
            set
            {
                _timingControlVMCollection = value;
                TimingSetting.TimingControlCollection = value.Select(x => x.timingControlModel).ToList();
                OnPropertyChanged(); 
            } 
        }



        private TimingControlViewModel _timingControlVMSelected;
        public TimingControlViewModel TimingControlVMSelected
        {
            get => _timingControlVMSelected??=new();
            set
            { 
                _timingControlVMSelected = value;
                OnPropertyChanged(); 
            }
        }











        public ObservableCollection<int> CharactersCountCollection
        {
            get
            {
                var EnumList = new ObservableCollection<int>();
                EnumList.Add(1);
                EnumList.Add(2);
                EnumList.Add(3);
                EnumList.Add(4);
                EnumList.Add(5); 
                EnumList.Add(6);
                EnumList.Add(7);


                return EnumList;
            }
        }


        public override ICommand LoadSettingCommand 
        {
            get => new RelayCommand(() =>
            {
                if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel NewTiming ,true))
                {
                    TimingSetting = NewTiming;
                }
            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                TimingSetting = new TimingSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new AsyncRelayCommand(async () =>
            {
                if (await JsonHM.WriteParameterSettingJsonSettingAsync(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, TimingSetting, true))
                {

                }
            });
        }

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();
    }
      
    public  class TimingControlViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => "TimingControlViewModel";
        public TimingControlViewModel()
        {

            timingControlModel = new();
        }
        public TimingControlViewModel(TimingControlModel timingControl)
        {
            timingControlModel = timingControl;
        }

        [JsonIgnore]
        public readonly TimingControlModel timingControlModel;
        /// <summary>
        /// 休息時間
        /// </summary>
        public DateTime RestTime { get => timingControlModel.RestTime; set { timingControlModel.RestTime = value; OnPropertyChanged(); } }
        /// <summary>
        /// 開啟時間
        /// </summary>
        public DateTime OpenTime { get => timingControlModel.OpenTime; set { timingControlModel.OpenTime = value; OnPropertyChanged(); } }
        /// <summary>
        /// 已啟用
        /// </summary>
        public bool IsEnable { get=> timingControlModel.IsEnable; set { timingControlModel.IsEnable = value;OnPropertyChanged(); } }


        private bool _isDeleteButtonTrigger;
        public bool IsDeleteButtonTrigger { get=> _isDeleteButtonTrigger; set { _isDeleteButtonTrigger = value;OnPropertyChanged(); } }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand 
        {
            get => _deleteCommand ??= new AsyncRelayCommand<object>(async obj=> 
            { 
                if(obj is ObservableCollection<GD_StampingMachine.ViewModels.ParameterSetting.TimingControlViewModel> collection)
                {
                    if(collection.Contains(this))
                        collection.Remove(this);
                }


        });
        }






    }






}
