using GD_CommonLibrary.Method;
using GongSolutions.Wpf.DragDrop;

namespace GD_StampingMachine.Method
{
    public class StampingFileDropTarget : BaseDropTarget
    {

        public override void DragEnter(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragEnter(dropInfo);
        }

        public override void DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public override void DragLeave(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragLeave(dropInfo);
        }

        public override void Drop(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragLeave(dropInfo);
        }

    }
}
