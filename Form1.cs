using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices;

namespace Submarine
{
    public partial class Form1 : Form
    {
        const int SHIP_NUM = 30;
        const int EGUN_NUM = 50;
        const int JGUN_NUM = 10;
        const int EGUN_SPEED = 3;
        const int JGUN_SPEED = 7;
        const int JGUN_GAP = 40;
        const int JAMSUHAM_SPEED = 8;

        struct SHIP {

            public bool exist;
            public int x, y;
            public int speed;
            public int direction;
        } SHIP[] ship = new SHIP[SHIP_NUM];

        struct EGUN {

            public bool exist;
            public int x, y;

        }
        EGUN[] egun = new EGUN[EGUN_NUM];
        struct JGUN {

            public bool exist;
            public int x, y;
        }
        JGUN[] jgun = new JGUN[JGUN_NUM];


        const int jW = 60; // 잠수함 너비
        const int jH = 35; // 잠수함 높이
        const int sW = 80; // 적군 배 너비
        const int sH = 65; // 적군 배 높이
        const int gW = 6; // 총알 너비
        const int gH = 16; // 총알 높이
        int jX = 600; // 잠수함 위치 X좌표
        int jY = 700; // 잠수함 위치 Y좌표
        int score = 0;//현재 점수
        static int record_score = 0; //신기록
        SoundPlayer sndBomb;//폭발소리
        Random random = new Random();

        Bitmap hJamsuham, hShip, hEgun, hJgun;
        Bitmap hArea = new Bitmap(1200, 800);
        [DllImport("User32.dll")]
        private static extern short GetkeyState(int nVirtKey);
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
