namespace NProject.Web.ViewModels
{
    public class ViewModelBase
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public ViewModelBase(int projectId, string projectName)
        {
            ProjectId = projectId;
            ProjectName = projectName;
        }

        public ViewModelBase()
        {
            
        }
    }
}