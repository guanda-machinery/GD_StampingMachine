using DevExpress.Mvvm.Xpf;
using GD_CommonLibrary;
using GD_StampingMachine.ViewModels.ProductSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine
{
    public partial class GD_Command
    {
        public static RelayParameterizedCommand Box_OnDragRecordOverCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DragRecordOverEventArgs e)
                    {
                        e.Effects = System.Windows.DragDropEffects.None;
                        if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                        {
                            e.Effects = System.Windows.DragDropEffects.Move;
                        }
                    }
                });
            }
        }
        //篩選器
        /// <summary>
        /// 篩選無註冊的零件
        /// </summary>
        public static DevExpress.Mvvm.ICommand<RowFilterArgs> PartsParameterVMCollection_Unassigned_RowFilterCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<RowFilterArgs>(args =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProductSetting.PartsParameterViewModel PartsParameterVM)
                {
                    if (PartsParameterVM.BoxIndex == null && PartsParameterVM.DistributeName == null)
                        args.Visible = true;
                    else
                        args.Visible = false;
                }
            });
        }
    }
}
