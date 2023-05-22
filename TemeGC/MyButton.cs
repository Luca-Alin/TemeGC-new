namespace TemeGC
{
    internal class MyButton : Button
    {
        public MyButton(string text, EventHandler eventE)
        {
            this.Size = new Size(70, 35);
            this.Text = text;
            this.Click += eventE;
        }
    }
}