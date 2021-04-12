using CCWFM.Helpers.Enums;
using CCWFM.UserControls;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class JobChildWindow
    {
        public JobChildWindow()
        {
            InitializeComponent();
            LayoutRoot.Children
               .Add(
                       new GenericForm
                                               ("TblAuthJob"
                                               , PermissionItemName.UserJobsForm
                                               )
                   );
        }
    }
}