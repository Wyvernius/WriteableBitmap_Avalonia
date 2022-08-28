using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Slidecrew_UI.CustomControls
{
    public partial class TimeBubble : UserControl
    {
        public TimeBubble()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly StyledProperty<IBrush> BackgroundColorProperty = new StyledProperty<IBrush>(nameof(BackgroundColor), typeof(TimeBubble), new StyledPropertyMetadata<IBrush>(new SolidColorBrush(Colors.White)));

        public static readonly StyledProperty<IBrush> ForegroundColorProperty = new StyledProperty<IBrush>(nameof(ForegroundColor), typeof(TimeBubble), new StyledPropertyMetadata<IBrush>(new SolidColorBrush(Colors.Black)));

        public static readonly DirectProperty<TimeBubble, string> TimeProperty = new DirectProperty<TimeBubble, string>(nameof(Time), (o) => { return o.time; }, (o, v) => o.Time = v, new DirectPropertyMetadata<string>("12:00"));

        public IBrush BackgroundColor
        {
            get { return GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public IBrush ForegroundColor
        {
            get { return GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}
