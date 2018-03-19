using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Autofac;
using LogConverter.Operations;
using LogConverterCore;
using Serilog.Events;

namespace LogConverter
{
    [ComVisible(true)]
    [Guid("6DE2F2EE-5DCF-422C-99A7-162F6E8D08E2")]
    [COMServerAssociation(AssociationType.Directory)]
    public class ContextMenuExtension : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var separator1 = new ToolStripSeparator();
            var separator2 = new ToolStripSeparator();

            var convertButton = new ToolStripMenuItem
            {
                Text = "Convert logs",
                //Image = Properties.Resources.ConvertLogFiles
            };

            convertButton.Click += (sender, args) => ConvertLogFiles();

            menu.Items.Add(separator1);
            menu.Items.Add(convertButton);
            menu.Items.Add(separator2);

            return menu;
        }

        private void ConvertLogFiles()
        {
            try
            {
                ConvertOptions configuration = GetProgramConfig();

                var container = AutofaConfigurationHelper.GetConfig().Build();
                var logsConverter = container.Resolve<LogsConverter>();

                foreach (var folder in SelectedItemPaths)
                {
                    logsConverter.ConvertLogs(folderPath: folder,
                        outputFolder: Path.Combine(folder, "Converter output"),
                        splitByComponents: false, //todo: change
                        componetsList: null,
                        minimumLevel: LogEventLevel.Verbose,
                        oneLevelSearch: false);
                }

                MessageBox.Show("Convertation complited.", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        public void ConvertLogFilesPerComponents()
        {

        }



        private ConvertOptions GetProgramConfig()
        {
            var defaultConfiguration = new ConvertOptions();



            return defaultConfiguration;

        }

        
    }
}
