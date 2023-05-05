using GD_CommonLibrary;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Commands
{
    public static partial class GD_Command
    {
        /*
        public static RelayParameterizedCommand Box_OnDropRecordCommand
        {
            get
            {
                return new RelayParameterizedCommand(obj =>
                {
                    if (obj is DevExpress.Xpf.Core.DropRecordEventArgs e)
                    {
                        if (e.Data.GetData(typeof(DevExpress.Xpf.Core.RecordDragDropData)) is DevExpress.Xpf.Core.RecordDragDropData DragDropData)
                        {
                            foreach (var _record in DragDropData.Records)
                            {
                                if (_record is PartsParameterViewModel PartsParameterVM)
                                {
                                    //看目前選擇哪一個盒子
                                    if (StampingBoxPartsVM.SelectedSeparateBoxVM != null)
                                    {
                                        PartsParameterVM.DistributeName = ProjectDistributeName;// ProjectDistribute.ProjectDistributeName;
                                        PartsParameterVM.BoxIndex = StampingBoxPartsVM.SelectedSeparateBoxVM.BoxIndex;
                                        e.Effects = System.Windows.DragDropEffects.Move;
                                    }
                                }
                            }
                        }
                    }

                });
            }
        }
        */
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
    }
}
