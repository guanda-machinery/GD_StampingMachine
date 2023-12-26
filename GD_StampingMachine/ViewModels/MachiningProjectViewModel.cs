using GD_StampingMachine.GD_Model;

namespace GD_StampingMachine.ViewModels
{
    public class MachiningProjectViewModel : GD_CommonLibrary.BaseViewModel
    {
        public MachiningProjectViewModel(MachiningProjectModel _machiningProject)
        {
            MachiningProject = _machiningProject;
        }
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_MachiningProjectViewModel");

        private readonly MachiningProjectModel MachiningProject = new();
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectName { get => MachiningProject.ProjectName; set { MachiningProject.ProjectName = value; OnPropertyChanged(); } }
        /// <summary>
        /// 製作數量
        /// </summary>
        public double WorkPieceCurrent { get => MachiningProject.WorkPieceCurrent; set { MachiningProject.WorkPieceCurrent = value; OnPropertyChanged(); } }

        /// <summary>
        /// 目標製作數量
        /// </summary>
        public double WorkPieceTarget { get => MachiningProject.WorkPieceTarget; set { MachiningProject.WorkPieceTarget = value; OnPropertyChanged(); } }













    }
}
