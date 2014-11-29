using System;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Core.Presentation.Factories;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Parallel = System.Activities.Statements.Parallel;

namespace DesignerRehosting
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new DesignerMetadata().Register();

            this.New_OnClick(null, null);

            this.AddToolboxItem();
        }

        public static readonly DependencyProperty DesignerProperty =
            DependencyProperty.Register("Designer", typeof(WorkflowDesigner), typeof(MainWindow), new PropertyMetadata(default(WorkflowDesigner)));

        public WorkflowDesigner Designer
        {
            get { return (WorkflowDesigner)GetValue(DesignerProperty); }
            set { SetValue(DesignerProperty, value); }
        }

        private void AddToolboxItem()
        {
            foreach (var category in InitToolboxItem())
            {
                ToolboxControl.Categories.Add(category);
            }
        }

        private static IEnumerable<ToolboxCategory> InitToolboxItem()
        {
            yield return new ToolboxCategory("フロー制御")
            {
                new ToolboxItemWrapper(typeof(Sequence)),
                new ToolboxItemWrapper(typeof(ForEachWithBodyFactory<>), "Foreach"),
                new ToolboxItemWrapper(typeof(If)),
                new ToolboxItemWrapper(typeof(Parallel)),
                new ToolboxItemWrapper(typeof(Switch<>), "Switch"),
                new ToolboxItemWrapper(typeof(While)),
                new ToolboxItemWrapper(typeof(DoWhile)),
            };

            yield return new ToolboxCategory("フローチャート")
            {
                new ToolboxItemWrapper(typeof(Flowchart)),
                new ToolboxItemWrapper(typeof(FlowDecision)),
                new ToolboxItemWrapper(typeof(FlowSwitch<>), "FlowSwitch")
            };

            yield return new ToolboxCategory("プリミティブ")
            {
                new ToolboxItemWrapper(typeof(Assign)),
                new ToolboxItemWrapper(typeof(Delay)),
                new ToolboxItemWrapper(typeof(InvokeMethod)),
                new ToolboxItemWrapper(typeof(WriteLine)),
            };

            yield return new ToolboxCategory("エラー処理")
            {
                new ToolboxItemWrapper(typeof(Rethrow)),
                new ToolboxItemWrapper(typeof(Throw)),
                new ToolboxItemWrapper(typeof(TryCatch)),
            };

            yield return new ToolboxCategory("カスタム")
            {
                new ToolboxItemWrapper(typeof(CodeActivity1), "Messagebox"),
            };

        }

        private const string TempFileName = "Temp.xaml";
        private void Run_OnClick(object sender, RoutedEventArgs e)
        {
            this.Designer.Flush();
            this.Designer.Save(TempFileName);

            var invoker = new WorkflowInvoker(ActivityXamlServices.Load(TempFileName));
            var ret = invoker.Invoke();

            MessageBox.Show("実行しました" + Environment.NewLine +
                            string.Join(Environment.NewLine, ret.Select(kvp => "{" + kvp.Key + "," + kvp.Value + "}")));

        }
        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "XAML File(.xaml)|*.xaml";
            if (dlg.ShowDialog() == true)
            {

                this.Designer.Flush();
                this.Designer.Save(dlg.FileName);
            }
        }

        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "XAML File(.xaml)|*.xaml";
            if (dlg.ShowDialog() == true)
            {
                var wd = CreateWorkflowDesigner();
                wd.Load(dlg.FileName);
                this.Designer = wd;
            }
        }
        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            var wd = CreateWorkflowDesigner();
            wd.Load(new ActivityBuilder { Name = "Workflow", Implementation = new Flowchart() });
            this.Designer = wd;
        }

        /// <summary>
        /// デザイナを作成します。
        /// </summary>
        /// <returns></returns>
        /// <see cref="http://blogs.msdn.com/b/tilovell/archive/2012/06/05/wf4-5-enabling-new-net-framework-4-5-features-in-your-rehosted-designer-application.aspx"/>
        private static WorkflowDesigner CreateWorkflowDesigner()
        {
            var workflow = new WorkflowDesigner();

            var configService =
                workflow.Context.Services.GetRequiredService<DesignerConfigurationService>();
            configService.AnnotationEnabled = true; /* maybe, see explanation of TargetFrameworkName*/
            configService.AutoConnectEnabled = true;
            configService.AutoSplitEnabled = true;
            configService.AutoSurroundWithSequenceEnabled = true;
            configService.BackgroundValidationEnabled = true;
            configService.MultipleItemsContextMenuEnabled = true;
            configService.MultipleItemsDragDropEnabled = true;
            configService.NamespaceConversionEnabled = true;
            configService.PanModeEnabled = true;
            configService.RubberBandSelectionEnabled = true;

            configService.LoadingFromUntrustedSourceEnabled = true;
            configService.TargetFrameworkName = new FrameworkName(".NETFramework,Version=v4.5");
            return workflow;
        }
    }
}
