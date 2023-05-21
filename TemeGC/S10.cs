namespace TemeGC
{
    internal class S10
    {
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            Graphics g = pb.CreateGraphics();

            return pb;
        }
    }
}