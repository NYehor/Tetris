using Avalonia.Controls;
using System.Threading.Tasks;

namespace TetrisView
{
    public partial class MessageBoxWindow : Window
    {
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        public enum MessageBoxResult
        {
            Ok,
            Cancel,
            Yes,
            No
        }

        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
        {
            var msgBox = new MessageBoxWindow()
            { 
                Title = title
            };

            msgBox.FindControl<TextBlock>("Text").Text = text;
            var buttonPanel = msgBox.FindControl<StackPanel>("Buttons");
            var res = MessageBoxResult.Ok;

            void AddButton(string text, MessageBoxResult result, bool def = false)
            {
                var button = new Button { Content = text};
                button.Click += (a, b) => {
                    res = result;
                    msgBox.Close();
                };

                buttonPanel.Children.Add(button);
                if (def)
                    res = result;
            }

            if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
            {
                AddButton("Ok", MessageBoxResult.Yes);
            }

            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton("Yes", MessageBoxResult.Yes);
                AddButton("No", MessageBoxResult.No, true);
            }

            msgBox.ShowDialog(parent);

            var task = new TaskCompletionSource<MessageBoxResult>();
            msgBox.Closed += (s, e) =>
            {
                task.TrySetResult(res);
            };


            if (!msgBox.ShowActivated)
            {
                msgBox.ShowDialog(parent);
            }

            return task.Task;
        }
    }
}
