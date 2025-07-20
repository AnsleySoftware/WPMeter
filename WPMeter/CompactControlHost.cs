namespace WPMeter
{
    public class CompactControlHost : ToolStripControlHost
    {
        public CompactControlHost(Control control) : base(control) 
        {
            this.AutoSize = false;
            this.Padding = Padding.Empty;
            this.Margin = Padding.Empty;
            control.AutoSize = false;
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[CompactHost] returning {Control.Size} for {Control.GetType().Name}");
            return Control.Size;
        }
    }
}
