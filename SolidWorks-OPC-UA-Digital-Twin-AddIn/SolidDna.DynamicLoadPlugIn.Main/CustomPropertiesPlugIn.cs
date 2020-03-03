using AngelSix.SolidDna;
using System.IO;

namespace SolidDna.DynamicLoadPlugIn
{
    /// <summary>
    /// Register as SolidDna Plugin
    /// </summary>
    public class CustomPropertiesSolidDnaPlugin : SolidPlugIn
    {
        #region Private Members
        /// <summary>
        /// The Taskpane UI for our plug-in
        /// </summary>
        private TaskpaneIntegration<TaskpaneUserControlHost> mTaskpane;
        #endregion


        #region Public Properties
        public override string AddInTitle => "OPC UA Digital Twin";

        public override string AddInDescription => "Modify assembly in real time based by connecting to OPC UA variables from and industrial controller";
        #endregion


        #region Connect To SolidWorks
        public override void ConnectedToSolidWorks()
        {
            // Create our taskpane
            mTaskpane = new TaskpaneIntegration<TaskpaneUserControlHost>()
            {
                Icon = Path.Combine(this.AssemblyPath(), "logo-small.png"),
                WpfControl = new CustomPropertiesUI()
            };

            // Add to taskpane
            mTaskpane.AddToTaskpaneAsync();
        }

        public override void DisconnectedFromSolidWorks()
        {

        }
        #endregion
    }
}
