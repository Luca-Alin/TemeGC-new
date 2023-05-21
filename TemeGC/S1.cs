using System.Windows.Forms;

namespace TemeGC
{
    internal class S1
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

        public static PictureBox P3(PictureBox pb)
        {
            pb.Size = new Size(Form1.WIDTH, Form1.HEIGHT);

            return pb;
        }
    }
}