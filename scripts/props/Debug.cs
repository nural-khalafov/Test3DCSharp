using Godot;

namespace Test3DCSharp.Utilities
{
    public partial class Debug : PanelContainer
    {
        private VBoxContainer _propertyContainer;

        private string _framesPerSecond;


        public override void _EnterTree()
        {
            ServiceLocator.RegisterService(this);
        }

        public override void _Ready()
        {
            _propertyContainer = GetNode<VBoxContainer>("%VBoxContainer");

            Visible = false;
        }

        public override void _Process(double delta)
        {
            if (Visible)
            {
                _framesPerSecond = (1.0 / delta).ToString("F2");
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("debug"))
            {
                Visible = !Visible;
            }
        }

        public void AddProperty(string title, Variant value, int order)
        {
            var target = _propertyContainer.FindChild(title, true, false) as Label;

            if (target == null)
            {
                target = new Label();
                target.Name = title;
                target.Text = $"{title}: {value}";
                _propertyContainer.AddChild(target);
            }
            else if (Visible)
            {
                target.Text = $"{title}: {value}";
                _propertyContainer.MoveChild(target, order);
            }
        }
    }
}
