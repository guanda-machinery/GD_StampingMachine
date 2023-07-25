using GD_CommonLibrary.Method;
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

        public override void DragOver(IDropInfo dropInfo)
        {
            if(dropInfo.DragInfo == null)
            {
                dropInfo.NotHandled = false;
                dropInfo.Effects = System.Windows.DragDropEffects.None;
                return;
            }


            if (dropInfo.DragInfo.SourceItem.GetType() == dropInfo.VisualTarget.GetType())
            {
                dropInfo.DropTargetAdorner = typeof(DropTargetHighlightAdorner);
                dropInfo.NotHandled = true;
                dropInfo.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                dropInfo.NotHandled = false;
                dropInfo.Effects = System.Windows.DragDropEffects.None;
            }
        }
        public override void Drop(IDropInfo dropInfo)
        {
            if(dropInfo.VisualTarget is FunctionToggleButton DropTarget && dropInfo.Data is FunctionToggleButton DropData)
                DropTarget.MainStackPanel.DataContext = DropData.MainStackPanel.DataContext ;
            
            if(dropInfo.VisualTarget is FunctionToggleUserControl UserControlDropTarget && dropInfo.Data is FunctionToggleUserControl UserControlDropData)
            {
                UserControlDropTarget.Toggle.DataContext = UserControlDropData.Toggle.DataContext;
            }

        }


    }



}
