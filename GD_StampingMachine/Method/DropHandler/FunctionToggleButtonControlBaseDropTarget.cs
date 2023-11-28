using DevExpress.Xpf.Core.Native;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.UserControls;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public class FunctionToggleButtonControlBaseDropTarget : BaseDropTarget
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo == null)
            {
                dropInfo.NotHandled = false;
                dropInfo.Effects = System.Windows.DragDropEffects.None;
                return;
            }

            if (dropInfo.DragInfo is FunctionToggleUserControl UserControlSourceData)
            {
                if (!UserControlSourceData.IsDragable)
                {
                    dropInfo.NotHandled = false;
                    dropInfo.Effects = System.Windows.DragDropEffects.None;
                    return;
                }
            }

            if (dropInfo.TargetItem is FunctionToggleUserControl UserControlTargetData)
            {
                if (UserControlTargetData.IsDropable)
                {
                    dropInfo.DropTargetAdorner = typeof(DropTargetHighlightAdorner);
                    dropInfo.NotHandled = true;
                    dropInfo.Effects = System.Windows.DragDropEffects.Copy;
                    return;
                }
            }


            dropInfo.NotHandled = false;
            dropInfo.Effects = System.Windows.DragDropEffects.None;
        }
        public override void Drop(IDropInfo dropInfo)
        {
           // if(dropInfo.VisualTarget is FunctionToggleButton DropTarget && dropInfo.Data is FunctionToggleButton DropData)
            //    DropTarget.MainStackPanel.DataContext = DropData.MainStackPanel.DataContext ;

            if (dropInfo.Data is FunctionToggleUserControl UserControlDropData)
            {
                //if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
                if (dropInfo.TargetItem is FunctionToggleUserControl UserControlTargetData)
                {

                    if (UserControlTargetData.IsDropable)
                    {
                        //將原按鈕的文字顏色保留
                        var tempForeground = UserControlTargetData.Toggle.Foreground;
                        UserControlTargetData.Toggle.DataContext = UserControlDropData.Toggle.DataContext;
                        UserControlTargetData.Toggle.Foreground = tempForeground;
                    }
                }

            }



        }


    }



}
