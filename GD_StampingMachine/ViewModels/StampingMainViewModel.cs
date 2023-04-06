//using DevExpress.Mvvm.CodeGenerators;
using DevExpress.Data.Extensions;
using DevExpress.Utils.CommonDialogs;
using DevExpress.Utils.CommonDialogs.Internal;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels.ProductSetting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    //[GenerateViewModel]
    public partial class StampingMainViewModel : ViewModelBase
    {
        public StampingMainViewModel()
        {
            Task.Run(() =>
             {
                 while (true)
                 {
                     DateTimeNow = DateTime.Now;
                     Thread.Sleep(100);
                 }
             });

            Task.Run(() =>
            {
                MechanicalSpecificationVM = new MechanicalSpecificationViewModel();
                StampingFontChangedVM.StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
            {
                    new StampingTypeViewModel(){ StampingTypeNumber =1 , StampingTypeString = "1" , StampingTypeUseCount=25} ,
                    new StampingTypeViewModel(){ StampingTypeNumber =2 , StampingTypeString = "2" , StampingTypeUseCount=180},
                    new StampingTypeViewModel(){ StampingTypeNumber =3, StampingTypeString = "3" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =4, StampingTypeString = "4" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =5, StampingTypeString = "5" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =6, StampingTypeString = "6" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =7, StampingTypeString = "7" , StampingTypeUseCount=25},
                    new StampingTypeViewModel(){ StampingTypeNumber =8, StampingTypeString = "8" , StampingTypeUseCount=25},

                    new StampingTypeViewModel(){ StampingTypeNumber =9, StampingTypeString = "9" , StampingTypeUseCount=25},

                    new StampingTypeViewModel(){ StampingTypeNumber =10, StampingTypeString = "0" , StampingTypeUseCount=25},

                    new StampingTypeViewModel(){ StampingTypeNumber =11, StampingTypeString = "A" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =12, StampingTypeString = "B" , StampingTypeUseCount=8677},

                    new StampingTypeViewModel(){ StampingTypeNumber =13, StampingTypeString = "C" , StampingTypeUseCount=7025},

                    new StampingTypeViewModel(){ StampingTypeNumber =14, StampingTypeString = "D" , StampingTypeUseCount=3015},

                    new StampingTypeViewModel(){ StampingTypeNumber =15, StampingTypeString = "E" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =16, StampingTypeString = "F" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =17, StampingTypeString = "G" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =18, StampingTypeString = "H" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =19, StampingTypeString = "I" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =20, StampingTypeString = "J" , StampingTypeUseCount=2025},

                    new StampingTypeViewModel(){ StampingTypeNumber =21, StampingTypeString = "K" , StampingTypeUseCount=5071 },

                    new StampingTypeViewModel(){ StampingTypeNumber =22, StampingTypeString = "L" , StampingTypeUseCount=1562},

                    new StampingTypeViewModel(){ StampingTypeNumber =23, StampingTypeString = "M" , StampingTypeUseCount=71},

                    new StampingTypeViewModel(){ StampingTypeNumber =24, StampingTypeString = "N" , StampingTypeUseCount=9071},

                    new StampingTypeViewModel(){ StampingTypeNumber =25, StampingTypeString = "O" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =26, StampingTypeString = "P" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =27, StampingTypeString = "Q" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =28, StampingTypeString = "R" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =29, StampingTypeString = "S" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =30, StampingTypeString = "T" , StampingTypeUseCount=5071},

                    new StampingTypeViewModel(){ StampingTypeNumber =31, StampingTypeString = "U" , StampingTypeUseCount=50},

                    new StampingTypeViewModel(){ StampingTypeNumber =32, StampingTypeString = "V" , StampingTypeUseCount=110},

                    new StampingTypeViewModel(){ StampingTypeNumber =33, StampingTypeString = "W" , StampingTypeUseCount=550},

                    new StampingTypeViewModel(){ StampingTypeNumber =34, StampingTypeString = "X" , StampingTypeUseCount=24},

                    new StampingTypeViewModel(){ StampingTypeNumber =35, StampingTypeString = "Y" , StampingTypeUseCount=5},
                    new StampingTypeViewModel(){ StampingTypeNumber =36, StampingTypeString = "Z" , StampingTypeUseCount=5},
                    new StampingTypeViewModel(){ StampingTypeNumber =37, StampingTypeString = "a" , StampingTypeUseCount=450},
                    new StampingTypeViewModel(){ StampingTypeNumber =38, StampingTypeString = "b" , StampingTypeUseCount=677},
                    new StampingTypeViewModel(){ StampingTypeNumber =39, StampingTypeString = "g" , StampingTypeUseCount=150},
                    new StampingTypeViewModel(){ StampingTypeNumber =40, StampingTypeString = "-" , StampingTypeUseCount=2550}
             };
                StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
            {
                    new StampingTypeViewModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0} ,
                    new StampingTypeViewModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0},
                    new StampingTypeViewModel(){ StampingTypeNumber =0, StampingTypeString = "ㄇ" , StampingTypeUseCount=0},
            };
                ProductSettingVM.ProductProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>()
            {
                new ProductProjectViewModel(new ProductProjectModel()
                {
                    Name="創典科技總公司基地",
                    Number = "AS001",
                    SheetStampingTypeForm = GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                    CreateTime= new DateTime(2022,10,27, 14,02,00),
                    EditTime = DateTime.Now,
                    FinishProgress = 10
                })
                ,
                new ProductProjectViewModel(new ProductProjectModel()
                {
                        Name="創典科技總公司基地-1",
                        Number = "AS002",
                        SheetStampingTypeForm =GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime= new DateTime(2022,10,27, 14,02,00),
                        FinishProgress = 26
                })
                ,
                new ProductProjectViewModel(new ProductProjectModel()
                {
                        Name="創典科技總公司基地-2",
                        Number = "AS003",
                        SheetStampingTypeForm =GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime= new DateTime(2022,10,27, 14,02,00),
                        FinishProgress = 51
                })
                ,
                new ProductProjectViewModel( new ProductProjectModel() {
                        Name="創典科技總公司基地-2",
                        Number = "AS003",
                        SheetStampingTypeForm =GD_Enum.SheetStampingTypeFormEnum.QRSheetStamping,
                        CreateTime= new DateTime(2022,10,27, 14,02,00),
                        FinishProgress = 76
                })


            };

            }
             );



            TypeSettingSettingVM.ProductProjectVMObservableCollection= ProductSettingVM.ProductProjectVMObservableCollection;

        }

        private DateTime _dateTimeNow = new DateTime();
        public DateTime DateTimeNow 
        { 
            get 
            {
                return _dateTimeNow;
            }
            set
            {
                _dateTimeNow = value;
                OnPropertyChanged(nameof(DateTimeNow));
            }
        }



        public RelayCommand OpenProjectFileCommand
        {
            get
            {
                return new RelayCommand(()=> 
                {
                    var fileContent = string.Empty;
                    var filePath = string.Empty;
                   
                    using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = "c:\\";
                        openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 2;
                        openFileDialog.RestoreDirectory = true;
                        
                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            //Get the path of specified file
                            filePath = openFileDialog.FileName;

                            //Read the contents of the file into a stream
                            var fileStream = openFileDialog.OpenFile();

                            using (StreamReader reader = new StreamReader(fileStream))
                            {
                                fileContent = reader.ReadToEnd();
                            }
                        }
                    }

                    

                });

            }
        }
        #region VM

        public MechanicalSpecificationViewModel MechanicalSpecificationVM { get; set; } = new MechanicalSpecificationViewModel();
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; } = new StampingFontChangedViewModel();
        public ParameterSettingViewModel ParameterSettingVM { get; set; } = new ParameterSettingViewModel();
        public ProductSettingViewModel ProductSettingVM { get; set; } = new ProductSettingViewModel();

        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; } = new TypeSettingSettingViewModel();


        #endregion










    }
}
