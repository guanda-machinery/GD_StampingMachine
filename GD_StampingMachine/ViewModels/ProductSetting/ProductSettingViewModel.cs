
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.WindowsUI;

using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_CommonLibrary.SplashScreenWindows;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GD_CommonLibrary;
using GD_CommonLibrary.Method;
using Newtonsoft.Json;
using DevExpress.Data.Extensions;
using DevExpress.XtraScheduler;
using GD_StampingMachine.Properties;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductSettingViewModel : BaseViewModelWithLog
    {
        public override string ViewModelName => (string)Application.Current.TryFindResource("Name_ProductSettingViewModel");

        private bool _addProjectDarggableIsPopup;
        public bool AddProjectDarggableIsPopup
        {
            get
            {
                return _addProjectDarggableIsPopup;
            }
            set
            {
                _addProjectDarggableIsPopup = value;
                OnPropertyChanged();
            }
        }



       // private string _projectPathText;
        public string ProjectPathText
        {
            get => CreatedProjectVM.ProductProjectPath;
            set
            {
                if (value != null)
                {
                    value = value.Replace(@"/", @"\");
                    while (value.Contains(@"\\"))
                    {
                        value = value.Replace(@"\\", @"\");
                    }

                    while (value.Contains(@".."))
                    {
                        value = value.Replace(@".", @".");
                    }
                }


                CreatedProjectVM.ProductProjectPath = value;
                OnPropertyChanged(nameof(ProjectPathText));
            }
        }

       



        private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                if (_productProjectVMObservableCollection == null)
                    _productProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
                return _productProjectVMObservableCollection;
            }
            set
            {
                _productProjectVMObservableCollection = value;
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
            }
        }

        [JsonIgnore]
        public ICommand SetProjectFolder
        {
            get => new RelayParameterizedCommand(obj =>
            {
                var sfd = new System.Windows.Forms.SaveFileDialog()
                {
                    Filter = "Json files (*.json)|*.json"
                };

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CreatedProjectVM.ProductProjectName = Path.GetFileNameWithoutExtension(sfd.FileName);
                    CreatedProjectVM.ProductProjectPath = Path.GetDirectoryName(sfd.FileName);
                    /*   if (obj is System.Windows.Controls.TextBox ObjTB)
                       {
                           sfd.FileName
                           ObjTB.Text = dialog.SelectedPath;
                       }*/
                }

                /*using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        if (obj is System.Windows.Controls.TextBox ObjTB)
                        {
                            ObjTB.Text = dialog.SelectedPath;
                        }
                    }
                }*/
            });
        }
        [JsonIgnore]
        public ICommand CreateProjectCommand
        {
            get => new RelayCommand(() =>
            {
                AddLogData((string)Application.Current.TryFindResource("btnAddProject"));
                var ExistedIndex = ProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectName == CreatedProjectVM.ProductProjectName);
                //檔案已存在 詢問是否要覆蓋
                if (ExistedIndex != -1)
                {
                    if (!MethodWinUIMessageBox.AskOverwriteOrNot())
                        return;
                }

                if (CreatedProjectVM.SaveProductProject())
                {
                    //若不clone會導致資料互相繫結
                    if (ExistedIndex != -1)
                    {
                        //ProductProjectVMObservableCollection.RemoveAt(ExistedIndex);
                        ProductProjectVMObservableCollection[ExistedIndex] = CreatedProjectVM.DeepCloneByJson();
                    }
                    else
                    {
                        ProductProjectVMObservableCollection.Add(CreatedProjectVM.DeepCloneByJson());
                    }

                    SaveProductListSetting();

                }
            });

        }

        public ProductProjectViewModel CreatedProjectVM { get; set; } = new ProductProjectViewModel(new ProductProjectModel()
        {
            Name = "NewProject",
            Number = "newAS001",
            CreateTime = DateTime.Now
        });

        [JsonIgnore]
        public Array SheetStampingTypeEnumCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(SheetStampingTypeFormEnum));
            }
        }

        [JsonIgnore]
        public ICommand RowDoubleClickCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                {
                    if(SelectProductProjectVM != ProjectItem)
                        SelectProductProjectVM = ProjectItem;
                    SelectProductProjectVM.IsInParameterPage = true;

                }
            });
        }


        [JsonIgnore]
        public ICommand LoadProductSettingCommand
        {
            get => new RelayCommand(() =>
            {
           
                if (JsonHM.ManualReadProductProject(out ProductProjectModel ReadProductProject))
                {
                    var ExisedIndex = ProductProjectVMObservableCollection.FindIndex(x => x.ProductProjectName == ReadProductProject.Name);
                    if(ExisedIndex != -1)
                    {
                        if(ProductProjectVMObservableCollection[ExisedIndex].ProductProjectPath == ReadProductProject.ProjectPath)
                            MethodWinUIMessageBox.ProjectIsLoaded();
                        else
                            MethodWinUIMessageBox.ProjectIsExisted_CantOpenProject();
                        return;
                    }

                    ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(ReadProductProject));
                    SaveProductListSetting();
                }
            });
        }
        [JsonIgnore]
        public ICommand SaveProductSettingCommand
        {
            get => new RelayCommand(() =>
            {
                ProductProjectVMObservableCollection.ForEach(obj =>
                {
                    obj.SaveProductProject();
                });
                SaveProductListSetting();
            });
        }

        private bool SaveProductListSetting()
        {
            //儲存一份路徑清單
            List<ProjectModel> PathList = new List<ProjectModel>();
            //將所有檔案儲存
            ProductProjectVMObservableCollection.ForEach(obj =>
            {
                PathList.Add(new ProjectModel
                {
                    Name = obj.ProductProjectName,
                    Number = obj.ProductProjectNumber,
                    ProjectPath = obj.ProductProjectPath
                });
            });

            return JsonHM.WriteProjectSettingJson(PathList);
        }





        private ProductProjectViewModel _selectProductProjectVM = new ProductProjectViewModel(new ProductProjectModel());
        /// <summary>
        /// 新增零件的vm
        /// </summary>
        
        public ProductProjectViewModel SelectProductProjectVM 
        {
            get => _selectProductProjectVM;
            set 
            { 
                _selectProductProjectVM = value;
                OnPropertyChanged(nameof(SelectProductProjectVM)); 
            }
        }

    }
}
