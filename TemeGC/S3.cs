namespace TemeGC
{
    internal class S3
    {
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);
            Graphics g = pb.CreateGraphics();

            return pb;
        }

        public static PictureBox P2(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);

            return pb;
        }
    }
}