using GD_StampingMachine.Extensions;
using GD_StampingMachine.UserControls.CustomControls;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GD_StampingMachine.Method
{
    public class FunctionToggleButtonControlBaseDropTarget : BaseDropTarget
    {
        public override void DragEnter(IDropInfo dropInfo)
        {
            base.DragEnter(dropInfo);
        }
        public override void DragLeave(IDropInfo dropInfo)
        {
            base.DragEnter(dropInfo);
        }
        public override void DragOver(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
            dropInfo.Effects = System.Windows.DragDropEffects.Copy;
        }
        public override void Drop(IDropInfo dropInfo)
        {
            var SourceData = dropInfo.Data;
            var TargetData = dropInfo.DropTargetAdorner;
            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
            {
            }

            var a = (dropInfo.VisualTarget as FunctionToggleButton);
            (dropInfo.VisualTarget as FunctionToggleButton).MainStackPanel.DataContext = (SourceData as FunctionToggleButton).MainStackPanel.DataContext ;
            //(dropInfo.VisualTarget as FunctionToggleButton).Content = (dropInfo.DragInfo.VisualSource as FunctionToggleButton).Content;
        }
    }



}
