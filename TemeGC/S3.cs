namespace TemeGC
{
    internal static class S3
    {
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            Graphics g = pb.CreateGraphics();

            return pb;
        }

        public static PictureBox P2(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);

            return pb;
        }
    }
}