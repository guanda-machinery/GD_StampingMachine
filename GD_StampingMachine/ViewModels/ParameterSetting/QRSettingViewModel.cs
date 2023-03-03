using GD_StampingMachine.Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;




namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class QRSettingViewModel : ViewModelBase
    {
        private QRSettingModel _QRSettingModel = new QRSettingModel();

        public QRSettingModel QRSetting
        {
            get 
            {
                return _QRSettingModel;
            } 
            set 
            {
                _QRSettingModel = value;
                OnPropertyChanged(nameof(QRSetting));
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


                return EnumList;
            }
        }


        public ObservableCollection<CharactersFormEnum> CharactersFormEnumCollection
        {
            get
            {
                var EnumList = new ObservableCollection<CharactersFormEnum>();
                foreach (CharactersFormEnum EachEnum in System.Enum.GetValues(typeof(CharactersFormEnum)))
                {
                    EnumList.Add(EachEnum);
                }

                return EnumList;
            }
        }




        public ObservableCollection<string> ModelSizeCollection
        {
            get
            {
                var EnumList = new ObservableCollection<string>();
                EnumList.Add("11x11");
                EnumList.Add("11x22");
                EnumList.Add("22x22");

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
                QRSetting = new QRSettingModel();
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
