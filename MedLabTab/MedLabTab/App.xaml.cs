using System.Configuration;
using System.Data;
using System.Windows;

namespace MedLabTab
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.StyleProperty.OverrideMetadata(
                typeof(Window),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = CreateDefaultWindowStyle()
                });
        }

        private Style CreateDefaultWindowStyle()
        {
            var style = new Style(typeof(Window));

            style.Setters.Add(new Setter(Window.WindowStateProperty, WindowState.Maximized));
            style.Setters.Add(new Setter(Window.WindowStyleProperty, WindowStyle.SingleBorderWindow));
            style.Setters.Add(new Setter(Window.ResizeModeProperty, ResizeMode.CanResize));

            return style;
        }
    }
}
