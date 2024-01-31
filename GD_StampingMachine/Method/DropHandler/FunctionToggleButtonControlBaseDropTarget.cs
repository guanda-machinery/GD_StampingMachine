using GD_CommonLibrary.Method;
using GD_StampingMachine.UserControls;
using GongSolutions.Wpf.DragDrop;

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

            //if(dropInfo.DropPosition)
            dropInfo.NotHandled = false;
            dropInfo.Effects = System.Windows.DragDropEffects.None;
        }
        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is FunctionToggleUserControl UserControlDropData)
            {
                if (dropInfo.TargetItem is FunctionToggleUserControl UserControlTargetData)
                {

                    if (UserControlTargetData.IsDropable)
                    {
                        //將原按鈕的文字顏色保留
                        UserControlTargetData.Toggle.DataContext = UserControlDropData.Toggle.DataContext;
                    }
                }

            }



        }


    }



}
