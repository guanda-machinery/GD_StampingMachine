using GD_StampingMachine.UserControls.CustomControls;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            dropInfo.Effects = System.Windows.DragDropEffects.Link;
        }
        public override void Drop(IDropInfo dropInfo)
        {
            var SourceData = dropInfo.Data;
            var TargetData = dropInfo.DropTargetAdorner;
            if (dropInfo.VisualTarget == dropInfo.DragInfo.VisualSource)
            {
            }
           (dropInfo.VisualTarget as FunctionToggleButton).DataContext = (dropInfo.DragInfo.VisualSource as FunctionToggleButton).DataContext;

        }
    }
}
