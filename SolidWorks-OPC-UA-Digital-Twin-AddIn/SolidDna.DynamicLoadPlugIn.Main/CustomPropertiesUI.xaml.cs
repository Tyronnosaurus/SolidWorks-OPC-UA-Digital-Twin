using Dna;
using AngelSix.SolidDna;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SolidDna.DynamicLoadPlugIn
{
    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : UserControl
    {
        #region Private Members
        private const string CustomPropertyDescription = "Description";
        private const string CustomPropertyLength = "Length";
        private const string CustomPropertyNote = "Note";
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomPropertiesUI()
        {
            InitializeComponent();
        }
        #endregion


        #region Startup
        /// <summary>
        /// Fired when the control is fully loaded
        /// </summary>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // By default show the No Part open screen
            NoPartContent.Visibility = System.Windows.Visibility.Visible;
            MainContent.Visibility   = System.Windows.Visibility.Hidden;

            // Listen out for the active model changing
            Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
        }
        #endregion


        #region Model Events
        /// <summary>
        /// Fired when the active SolidWorks model is changed
        /// </summary>
        private void Application_ActiveModelInformationChanged(Model obj)
        {
            ReadDetails();
        }
        #endregion


        #region Read Details
        /// <summary>
        /// Reads all the details from the active SolidWorks model
        /// </summary>
        private void ReadDetails()
        {
            ThreadHelpers.RunOnUIThread(() =>
            {
                // Get the active model
                var model = Application.ActiveModel;

                // If we have no model, or the model is not a part
                // then show the No Part screen and return
                if (model == null || (!model.IsPart && !model.IsAssembly))
                {
                    // Show No Part screen
                    NoPartContent.Visibility = System.Windows.Visibility.Visible;
                    MainContent.Visibility   = System.Windows.Visibility.Hidden;
                    return;
                }

                // If we got here, we have a valid model

                // Listen out for selection changes
                model.SelectionChanged += Model_SelectionChanged;

                // Show the main sidepanel content
                NoPartContent.Visibility = System.Windows.Visibility.Hidden;
                MainContent.Visibility   = System.Windows.Visibility.Visible;

                // Description
                DescriptionText.Text = model.GetCustomProperty(CustomPropertyDescription);

                // Dimension
                SheetMetalLengthText.Text          = model.GetCustomProperty(CustomPropertyLength);
                SheetMetalLengthEvaluatedText.Text = model.GetCustomProperty(CustomPropertyLength, resolved: true);

                // Notes
                NoteText.Text = model.GetCustomProperty(CustomPropertyNote);
            });
        }

        /// <summary>
        /// When selecting something, check if it is a dimension and enable the Get Dimension button
        /// </summary>
        private void Model_SelectionChanged()
        {
            Application?.ActiveModel?.SelectedObjects((objects) =>
            {
                ThreadHelpers.RunOnUIThread(() =>
                {
                    LengthButton.IsEnabled = objects.Any(f => f.IsDimension);
                });
            });
        }
        #endregion


        #region Set Details
        /// <summary>
        /// Sets all the details to the active SolidWorks model
        /// </summary>
        public void SetDetails()
        {
            var model = Application.ActiveModel;

            // Check we have a part
            if (model == null || !model.IsPart) return;

            // Description
            model.SetCustomProperty(CustomPropertyDescription, DescriptionText.Text);

            // Note
            model.SetCustomProperty(CustomPropertyNote, NoteText.Text);

            // Re-read details to confirm they are correct
            ReadDetails();
        }
        #endregion


        #region Button Events
        /// <summary>
        /// Called when the read button is clicked
        /// </summary>
        private void ReadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReadDetails();
        }

        /// <summary>
        /// Called when the reset button is clicked
        /// </summary>
        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Clear all values
            DescriptionText.Text = string.Empty;

            SheetMetalLengthText.Text = string.Empty;
            SheetMetalLengthEvaluatedText.Text = string.Empty;

            NoteText.Text = string.Empty;
        }

        /// <summary>
        /// Called when the apply button is clicked
        /// </summary>
        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetDetails();
        }
        
        /// <summary>
        /// Called when 'Get' button is pressed
        /// </summary>
        private void LengthButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.ActiveModel?.SelectedObjects((objects) =>
            {
                var lastDimension = objects.LastOrDefault(f => f.IsDimension);  //Get newest dimension

                if (lastDimension == null) return;  //Double check we have one

                var dimensionSelectionName = string.Empty;            
                lastDimension.AsDimension((dimension) => dimensionSelectionName = dimension.SelectionName);    // Get the dimension name

                // Set the length button name
                ThreadHelpers.RunOnUIThread(() =>
                {
                    SheetMetalLengthText.Text = $"\"{dimensionSelectionName}\"";
                });
            });
        }

        private void SetLengthButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Get the active model
            var model = Application.ActiveModel;

            ThreadHelpers.RunOnUIThread(() =>
            {
                model.SetCustomProperty("D1@Sketch8", "105");
            });

        }
        #endregion



        public void SetDimension(string name, string value, string configuration = null)
        {
            public ModelExtension Extension = new ModelExtension(BaseObject.Extension, Application.ActiveModel);


            // Get the custom property editor
            using (var editor = Extension.CustomPropertyEditor(configuration))
            {
                // Set the property
                editor.SetCustomProperty(name, value);
            }
        }



    }
}
