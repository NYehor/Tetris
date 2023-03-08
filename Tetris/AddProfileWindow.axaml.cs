using Avalonia.Controls;
using System.Threading.Tasks;

namespace TetrisView
{
    public partial class AddProfileWindow : Window
    {
        public AddProfileWindow()
        {
            InitializeComponent();
        }

        public static Task<string> ShowAndTryGetName(Window parent)
        {
            string name = "";
            var window = new AddProfileWindow();

            var okBtn = window.FindControl<Button>("addBtn");
            var cancelBtn = window.FindControl<Button>("cancelBtn");

            okBtn.Click += (s, e) =>
            {
                name = window.FindControl<TextBox>("profileNameTextBox").Text;
                window.Close();
            };

            cancelBtn.Click += (s, e) =>
            {
                window.Close();
            };

            var task = new TaskCompletionSource<string>();
            window.Closed += (s, e) =>
            {
                task.TrySetResult(name);
            };
            window.ShowDialog(parent);

            return task.Task;
        }
    }
}
