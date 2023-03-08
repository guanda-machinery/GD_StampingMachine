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
    public class NumberSettingViewModel : ViewModelBase
    {
        /// <summary>
        /// 預覽圖VM
        /// </summary>
        //public NumberSettingSchematicDiagramViewModel NumberSettingSchematicDiagramVM = new NumberSettingSchematicDiagramViewModel();


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
                for (int i = 1; i <= 8; i++)
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


        private int? _sequenceCountComboBoxSelectValue = null;
        /// <summary>
        /// 單排數量
        /// </summary>
        public int? SequenceCountComboBoxSelectValue
        {
            get
            {
                if (_sequenceCountComboBoxSelectValue.HasValue)
                {
                    //NumberSettingSchematicDiagramVM.SequenceCount = _sequenceCountComboBoxSelectValue.Value;
                    NumberSetting.SequenceCount = _sequenceCountComboBoxSelectValue.Value;
                }
                return _sequenceCountComboBoxSelectValue;
            }
            set
            {
                _sequenceCountComboBoxSelectValue = value;
                OnPropertyChanged(nameof(SequenceCountComboBoxSelectValue));
            }
        }

        private SpecialSequenceEnum? _specialSequenceComboBoxSelectValue = null;
        /// <summary>
        /// 特殊排序
        /// </summary>
        public SpecialSequenceEnum? SpecialSequenceComboBoxSelectValue
        {
            get
            {
                if (_sequenceCountComboBoxSelectValue.HasValue)
                {
                    NumberSetting.SpecialSequence = _specialSequenceComboBoxSelectValue.Value;
                }
                return _specialSequenceComboBoxSelectValue;

            }
            set
            {
                _specialSequenceComboBoxSelectValue = value;
                OnPropertyChanged(nameof(SpecialSequenceComboBoxSelectValue));
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
