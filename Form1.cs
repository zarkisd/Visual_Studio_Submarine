using System;
using System.Drawing;
using System.Text;
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

        struct SHIP
        {
            public bool exist;
            public int x, y;
            public int speed;
            public int direction;
        }
        SHIP[] ship = new SHIP[SHIP_NUM];

        struct EGUN
        {
            public bool exist;
            public int x, y;
        }
        EGUN[] egun = new EGUN[EGUN_NUM];

        struct JGUN
        {
            public bool exist;
            public int x, y;
        }
        JGUN[] jgun = new JGUN[JGUN_NUM];


        const int jW = 60;  //잠수함 너비
        const int jH = 35;  //잠수함 높이
        const int sW = 80;  //적군 배 너비
        const int sH = 65;  //적군 배 높이
        const int gW = 6;   //총알 너비
        const int gH = 16;  //총알 높이
        int jX = 600; //잠수함 위치 X좌표
        int jY = 700; //잠수함 위치 Y좌표
        int score = 0; //현재 점수
        static int record_score = 0; //신기록
        SoundPlayer sndBomb;        //폭팔 소리
        Random random = new Random();

        Bitmap hJamsuham, hShip, hEgun, hJgun, hSea;
        Bitmap hArea = new Bitmap(1200, 800);

        [DllImport("User32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Size = new Size(1200, 800);
            hSea = Properties.Resources.sea;
            hJamsuham = Properties.Resources.jamsuham;
            hShip = Properties.Resources.ship;
            hEgun = Properties.Resources.egun;
            hJgun = Properties.Resources.jgun;

            StartGame();
        }
        private void StartGame()
        {
            //적군배와 총알들의 존재를 false로 초기화한다.
            for (int i = 0; i < SHIP_NUM; i++)
                ship[i].exist = false;
            for (int i = 0; i < EGUN_NUM; i++)
                egun[i].exist = false;
            for (int i = 0; i < JGUN_NUM; i++)
                jgun[i].exist = false;

            //배경음악 재생 및 폭팔 효과음 등록
            mciSendString("open \"" + "../../resource/bg.mp3" + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
            mciSendString("play MediaFile REPEAT", null, 0, IntPtr.Zero);
            sndBomb = new SoundPlayer(Properties.Resources.bomb);
            score = 0;

            //타이머를 시작한다.
            timer1.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    StartGame();
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (hArea != null)
            {
                e.Graphics.DrawImage(hArea, 0, 0);
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 배경색으로 지우지 않음
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            SHIP local_ship;
            EGUN local_egun;
            JGUN local_jgun;
            Rectangle shiprt, jamsuhamrt, egunrt, jgunrt, irt;
            int maxY = -1;
            int i, j;

            Graphics g = Graphics.FromImage(hArea);
            //g.DrawImage(Properties.Resources.sea, 0, 0);
            g.DrawImage(hSea, 0, 0);
            g.DrawImage(hJamsuham, jX - jW / 2, jY);

            if (GetKeyState((int)Keys.Left) < 0)
            {
                jX = jX - JAMSUHAM_SPEED;
                jX = Math.Max(jW / 2, jX);
            }
            if (GetKeyState((int)Keys.Right) < 0)
            {
                jX = jX + JAMSUHAM_SPEED;
                jX = Math.Min(ClientSize.Width - jW / 2, jX);
            }

            if (GetKeyState((int)Keys.Space) < 0)
            {
                for (i = 0; i < JGUN_NUM; i++)
                {
                    if (jgun[i].exist == true)
                        maxY = Math.Max(jgun[i].y, maxY);
                }
                for (i = 0; i < JGUN_NUM; i++)
                {
                    if (jgun[i].exist == false)
                        break;
                }
                if (i != JGUN_NUM && jY - maxY > JGUN_GAP)
                {
                    jgun[i].exist = true;
                    jgun[i].x = jX;
                    jgun[i].y = jY - gH;
                }
            }

            for (i = 0; i < JGUN_NUM; i++)
            {
                if (jgun[i].exist == false)
                    continue;
                if (jgun[i].y > 0)
                {
                    jgun[i].y -= JGUN_SPEED;
                    g.DrawImage(hJgun, jgun[i].x - jW / 2, jgun[i].y);
                }
                else
                {
                    jgun[i].exist = false;
                }
            }

            //적군배 생성
            if (random.Next(10) == 0)
            {
                for (i = 0; i < SHIP_NUM && ship[i].exist == true; i++) {; }
                if (i != SHIP_NUM)
                {
                    if (random.Next(2) == 1)
                    {
                        ship[i].x = sW / 2;
                        ship[i].direction = 1;
                    }
                    else
                    {
                        ship[i].x = ClientSize.Width - sW / 2;
                        ship[i].direction = -1;
                    }
                    ship[i].y = 300;
                    ship[i].speed = random.Next(4) + 3;
                    ship[i].exist = true;
                }
            }

            //적군배 이동 및 총알 발사
            for (i = 0; i < SHIP_NUM; i++)
            {
                if (ship[i].exist == false)
                    continue;
                ship[i].x += ship[i].speed * ship[i].direction;
                if (ship[i].x < 0 || ship[i].x > ClientSize.Width)
                {
                    ship[i].exist = false;
                }
                else
                {
                    g.DrawImage(hShip, ship[i].x - sW / 2, ship[i].y);
                }
                if (random.Next(30) == 0)
                {
                    for (j = 0; j < EGUN_NUM && egun[j].exist == true; j++) {; }
                    if (j != EGUN_NUM)
                    {
                        egun[j].x = ship[i].x + sW / 2;
                        egun[j].y = ship[i].y;
                        egun[j].exist = true;
                    }
                }
            }

            // 적군 총알 이동 및 그림
            for (i = 0; i < EGUN_NUM; i++)
            {
                if (egun[i].exist == false)
                    continue;
                if (egun[i].y < jY)
                {
                    egun[i].y += EGUN_SPEED;
                    g.DrawImage(hEgun, egun[i].x - 3, egun[i].y);
                }
                else
                {
                    egun[i].exist = false;
                }
            }

            // 적군과 총알의 충돌 판정
            for (i = 0; i < SHIP_NUM; i++)
            {
                if (ship[i].exist == false)
                    continue;

                local_ship = ship[i];
                int w = sW;
                int h = sH;
                shiprt = new Rectangle(local_ship.x - w / 2, local_ship.y, w, h);
                for (j = 0; j < JGUN_NUM; j++)
                {
                    if (jgun[j].exist == false) continue;
                    JGUN b = jgun[j];
                    jgunrt = new Rectangle(b.x - jW / 2, b.y, jW, jH);
                    irt = Rectangle.Intersect(shiprt, jgunrt);
                    if (irt.IsEmpty == false)
                    {
                        ship[i].exist = false;
                        egun[j].exist = false;

                        score = score + 10;
                        if (record_score < score)
                        {
                            record_score = record_score + 10;
                        }
                        sndBomb.Play();
                    }
                }
            }

            Font _font = new System.Drawing.Font(new FontFamily("휴먼둥근헤드라인"), 14, FontStyle.Bold);
            g.DrawString("Record : " + score.ToString(), _font, Brushes.DarkBlue, new PointF(10, 20));
            g.DrawString("New Record : " + record_score.ToString(), _font, Brushes.DarkBlue, new PointF(500, 20));

            // 적군 총알과 아군의 충돌 판정
            jamsuhamrt = new Rectangle(jX - jW / 2, jY, jW, jH);
            for (i = 0; i < EGUN_NUM; i++)
            {
                if (egun[i].exist == false) continue;
                egunrt = new Rectangle(egun[i].x - gW / 2, egun[i].y, gW, gH);
                irt = Rectangle.Intersect(jamsuhamrt, egunrt);
                if (irt.IsEmpty == false)
                {
                    mciSendString("stop MediaFile", null, 0, IntPtr.Zero);
                    sndBomb.Play();
                    timer1.Stop();
                }
            }

            Invalidate();
        }
    }
}
