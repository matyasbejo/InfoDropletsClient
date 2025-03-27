using System.ComponentModel;
using System.Windows.Threading;

namespace InfoDroplets.ResultExporterApp
{
    public class GlobalLabelHelper : INotifyPropertyChanged
    {
        private static GlobalLabelHelper _instance = new GlobalLabelHelper();
        public static GlobalLabelHelper Instance => _instance;

        private string labelText;
        public string LabelText
        {
            get => labelText;
            set
            {
                if (labelText != value)
                {
                    labelText = value;
                    OnPropertyChanged(nameof(LabelText));
                    ForceUIToUpdate();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.Render);
        }

        void ForceUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background,
                                          new Action(delegate { }));
        }
    }

}
