//using DevExpress.Mvvm.CodeGenerators;
using DevExpress.Data.Extensions;
using DevExpress.Utils.CommonDialogs;
using DevExpress.Utils.CommonDialogs.Internal;
using GD_StampingMachine.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                 while(true)
                 {
                     DateTimeNow = DateTime.Now;
                     Thread.Sleep(100);
                 }
             });
            MechanicalSpecificationVM = new MechanicalSpecificationViewModel();

            StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeModel>()
            {
                    new Model.StampingTypeModel(){ StampingTypeNumber =1 , StampingTypeString = "1" , StampingTypeUseCount=25} ,
                    new Model.StampingTypeModel(){ StampingTypeNumber =2 , StampingTypeString = "2" , StampingTypeUseCount=180},
                    new Model.StampingTypeModel(){ StampingTypeNumber =3, StampingTypeString = "3" , StampingTypeUseCount=25},
                    new Model.StampingTypeModel(){ StampingTypeNumber =4, StampingTypeString = "4" , StampingTypeUseCount=25},
                    new Model.StampingTypeModel(){ StampingTypeNumber =5, StampingTypeString = "5" , StampingTypeUseCount=25},
                    new Model.StampingTypeModel(){ StampingTypeNumber =6, StampingTypeString = "6" , StampingTypeUseCount=25},
                    new Model.StampingTypeModel(){ StampingTypeNumber =7, StampingTypeString = "7" , StampingTypeUseCount=25},
                    new Model.StampingTypeModel(){ StampingTypeNumber =8, StampingTypeString = "8" , StampingTypeUseCount=25},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =9, StampingTypeString = "9" , StampingTypeUseCount=25},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =10, StampingTypeString = "0" , StampingTypeUseCount=25},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =11, StampingTypeString = "A" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =12, StampingTypeString = "B" , StampingTypeUseCount=8677},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =13, StampingTypeString = "C" , StampingTypeUseCount=7025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =14, StampingTypeString = "D" , StampingTypeUseCount=3015},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =15, StampingTypeString = "E" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =16, StampingTypeString = "F" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =17, StampingTypeString = "G" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =18, StampingTypeString = "H" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =19, StampingTypeString = "I" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =20, StampingTypeString = "J" , StampingTypeUseCount=2025},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =21, StampingTypeString = "K" , StampingTypeUseCount=5071 },
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =22, StampingTypeString = "L" , StampingTypeUseCount=1562},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =23, StampingTypeString = "M" , StampingTypeUseCount=71},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =24, StampingTypeString = "N" , StampingTypeUseCount=9071},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =25, StampingTypeString = "O" , StampingTypeUseCount=5071},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =26, StampingTypeString = "P" , StampingTypeUseCount=5071},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =27, StampingTypeString = "Q" , StampingTypeUseCount=5071},
                      
                    new Model.StampingTypeModel(){ StampingTypeNumber =28, StampingTypeString = "R" , StampingTypeUseCount=5071},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =29, StampingTypeString = "S" , StampingTypeUseCount=5071},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =30, StampingTypeString = "T" , StampingTypeUseCount=5071},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =31, StampingTypeString = "U" , StampingTypeUseCount=50},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =32, StampingTypeString = "V" , StampingTypeUseCount=110},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =33, StampingTypeString = "W" , StampingTypeUseCount=550},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =34, StampingTypeString = "X" , StampingTypeUseCount=24},
                        
                    new Model.StampingTypeModel(){ StampingTypeNumber =35, StampingTypeString = "Y" , StampingTypeUseCount=5}, 
                    new Model.StampingTypeModel(){ StampingTypeNumber =36, StampingTypeString = "Z" , StampingTypeUseCount=5},                    
                    new Model.StampingTypeModel(){ StampingTypeNumber =37, StampingTypeString = "a" , StampingTypeUseCount=450},               
                    new Model.StampingTypeModel(){ StampingTypeNumber =38, StampingTypeString = "b" , StampingTypeUseCount=677},  
                    new Model.StampingTypeModel(){ StampingTypeNumber =39, StampingTypeString = "g" , StampingTypeUseCount=150},
                    new Model.StampingTypeModel(){ StampingTypeNumber =40, StampingTypeString = "-" , StampingTypeUseCount=2550}
             };

            UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeModel>()
            {
                    new Model.StampingTypeModel(){ StampingTypeNumber =1 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0} ,
                    new Model.StampingTypeModel(){ StampingTypeNumber =2 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0},
                    new Model.StampingTypeModel(){ StampingTypeNumber =3, StampingTypeString = "ㄇ" , StampingTypeUseCount=0},
                    new Model.StampingTypeModel(){ StampingTypeNumber =1 , StampingTypeString = "ㄈ" , StampingTypeUseCount=0} ,
                    new Model.StampingTypeModel(){ StampingTypeNumber =2 , StampingTypeString = "ㄉ" , StampingTypeUseCount=0},
                    new Model.StampingTypeModel(){ StampingTypeNumber =3, StampingTypeString = "ㄊ" , StampingTypeUseCount=0},
                    new Model.StampingTypeModel(){ StampingTypeNumber =1 , StampingTypeString = "ㄋ" , StampingTypeUseCount=0} ,
                    new Model.StampingTypeModel(){ StampingTypeNumber =2 , StampingTypeString = "ㄌ" , StampingTypeUseCount=0},
                    new Model.StampingTypeModel(){ StampingTypeNumber =3, StampingTypeString = "ㄍ" , StampingTypeUseCount=0},
            };
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

        public MechanicalSpecificationViewModel MechanicalSpecificationVM { get; set; } = new MechanicalSpecificationViewModel();


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
                if(StampingFontSelected != null && UnusedStampingFontSelected != null)
                {
                    var FontString = StampingFontSelected.StampingTypeString;
                    var FontStringNumber =  StampingFontSelected.StampingTypeNumber;
                    var FontStringUseCount =  StampingFontSelected.StampingTypeUseCount;
                  
                    var UnusedFontString =  UnusedStampingFontSelected.StampingTypeString;
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
