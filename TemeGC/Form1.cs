using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TemeGC
{
    public partial class Form1 : Form
    {
        public static int WIDTH;
        public static int HEIGHT;
        
        public static PictureBox pictureBox1 = new PictureBox();
        static void swap(object sender, EventArgs e)
        {
            Button b = sender as Button;
            String s = b.Text;
            pictureBox1.Image = null;
            pictureBox1.Update();

            Type pictureBoxType = typeof(PictureBox);

            // Get the event information for the MouseClick event
            EventInfo mouseClickEvent = pictureBoxType.GetEvent("MouseClick");

            // Get the field storing the event handlers
            FieldInfo eventHandlersField = pictureBoxType.GetField("EventMouseClick", BindingFlags.NonPublic | BindingFlags.Instance);

            // Get the current event handlers
            if (eventHandlersField != null)
            {
                Delegate eventHandlers = (Delegate)eventHandlersField.GetValue(pictureBox1);
                if (eventHandlers != null)
                {
                    foreach (Delegate handler in eventHandlers.GetInvocationList())
                    {
                        pictureBoxType.GetEvent("MouseClick").RemoveEventHandler(pictureBox1, handler);
                    }
                }
            }

            // Remove all event handlers



            switch (s)
            {
                case ("S1P1"):
                    pictureBox1 = S1.P1(pictureBox1);
                    break;
                case ("S1P2"):
                    pictureBox1 = S1.P2(pictureBox1);
                    break;
                case ("S1P3"):
                    pictureBox1 = S1.P3(pictureBox1);
                    break;
                case ("S2P1"):
                    pictureBox1 = S2.P1(pictureBox1);
                    break;
                case ("S2P2"):
                    pictureBox1 = S2.P2(pictureBox1);
                    break;
                case ("S2P3"):
                    pictureBox1 = S2.P3(pictureBox1);
                    break;
                case ("S3P1"):
                    pictureBox1 = S3.P1(pictureBox1);
                    break;
                case ("S3P2"):
                    pictureBox1 = S3.P2(pictureBox1);
                    break;
                case ("S4P1"):
                    pictureBox1 = S4.P1(pictureBox1);
                    break;
                case ("S5P1"):
                    pictureBox1 = S5.P1(pictureBox1);
                    break;
                case ("S6P1"):
                    pictureBox1 = S6.P1(pictureBox1);
                    break;
                case ("S6P2"):
                    pictureBox1 = S6.P2(pictureBox1);
                    break;
                case ("S6P3"):
                    pictureBox1 = S6.P3(pictureBox1);
                    break;
                case ("S7P1"):
                    pictureBox1 = S7.P1(pictureBox1);
                    break;
                case ("S8P1"):
                    pictureBox1 = S8.P1(pictureBox1);
                    break;
                case ("S9P1"):
                    pictureBox1 = S9.P1(pictureBox1);
                    break;
                case ("S10P1"):
                    pictureBox1 = S10.P1(pictureBox1);
                    break;
                case ("S11P1"):
                    pictureBox1 = S11.P1(pictureBox1);
                    break;
            }
        }

        MyButton[] myButtons =
        {
            new MyButton("S1P1", (s, e) => { swap(s, e); }),
            new MyButton("S1P2", (s, e) => { swap(s, e); }),
            new MyButton("S1P3", (s, e) => { swap(s, e); }),
            new MyButton("S2P1", (s, e) => { swap(s, e); }),
            new MyButton("S2P3", (s, e) => { swap(s, e); }),
            new MyButton("S2P3", (s, e) => { swap(s, e); }),
            new MyButton("S3P1", (s, e) => { swap(s, e); }),
            new MyButton("S3P2", (s, e) => { swap(s, e); }),
            new MyButton("S4P1", (s, e) => { swap(s, e); }),
            new MyButton("S5P1", (s, e) => { swap(s, e); }),
            new MyButton("S6P1", (s, e) => { swap(s, e); }),
            new MyButton("S6P2", (s, e) => { swap(s, e); }),
            new MyButton("S6P3", (s, e) => { swap(s, e); }),
            new MyButton("S7P1", (s, e) => { swap(s, e); }),
            new MyButton("S8P1", (s, e) => { swap(s, e); }),
            new MyButton("S9P1", (s, e) => { swap(s, e); }),
            new MyButton("S10P1", (s, e) => { swap(s, e); }),
            new MyButton("S11P1", (s, e) => { swap(s, e); })
        };

        public Form1()
        {
            InitializeComponent();

            
            foreach (MyButton mb in myButtons)
            {
                flowLayoutPanel1.Controls.Add(mb);
            }
            pictureBox1.Size = new Size(flowLayoutPanel2.Width, flowLayoutPanel2.Height);
            flowLayoutPanel2.Controls.Add (pictureBox1);
            WIDTH = flowLayoutPanel2.Width;
            HEIGHT = flowLayoutPanel2.Height;

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}