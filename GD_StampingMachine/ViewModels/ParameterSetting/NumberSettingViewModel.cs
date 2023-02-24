using GD_StampingMachine.Enum;
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
    public class NumberSettingViewModel: ViewModelBase
    {
        private NumberSettingModel _NumberSettingModel = new NumberSettingModel();

        public NumberSettingModel NumberSetting
        {
            get 
            {
                return _NumberSettingModel;
            } 
            set 
            {
                _NumberSettingModel = value;
                OnPropertyChanged(nameof(NumberSetting));
            }

        } 

        public ObservableCollection<NumberSettingModeEnum> NumberSettingModeCollection
        { 
            get
            {
                var EnumList = new ObservableCollection<NumberSettingModeEnum>();
                foreach (NumberSettingModeEnum EachEnum in System.Enum.GetValues(typeof(NumberSettingModeEnum)))
                {
                    EnumList.Add(EachEnum);
                }
                return EnumList;
            }
        }

        public ObservableCollection<int> SequenceCountCollection
        {
            get
            {
                var CountCollection = new ObservableCollection<int>();  
                for(int i=1; i<=8;i++)
                {
                    CountCollection.Add(i);
                }
                return CountCollection;
            }
        }

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






        public ICommand LoadModeCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
        public ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                NumberSetting = new NumberSettingModel();
            });
        }

        public ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
    }










}
