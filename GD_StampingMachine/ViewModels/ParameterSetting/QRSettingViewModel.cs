using GD_StampingMachine.GD_Enum;
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
    public class QRSettingViewModel : NumberSettingViewModel
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

        public override int PlateNumberListMax { get; set; } = 6;

        public override ICommand LoadModeCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                QRSetting = new QRSettingModel();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {

            });
        }
    }










}
