
using System;

namespace SplendidCRM.Administration.ModuleBuilder
{
    /// <summary>
    ///     Summary description for Default.
    /// </summary>
    public class Default : SplendidAdminPage
    {
        private void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
              
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IsAdminPage = true;
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion
    }
}