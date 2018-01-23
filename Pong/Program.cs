using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pong
{
    class Program
    {
        public int Player1_score = 0;
        public int Player2_score = 0;
        public const int FIELD_X = 150; //Variable initialization
        public const int FIELD_Y = 51;
        public bool hasScored = false;

        static void Main(string[] args)
        {
            Paddle leftPaddle = new Paddle();
            leftPaddle.size = 11;
            leftPaddle.pos_y = ((FIELD_Y - 1) / 2);
            leftPaddle.pos_x = 0;
            Paddle rightPaddle = new Paddle();
            rightPaddle.size = 11;
            rightPaddle.pos_y = ((FIELD_Y - 1) / 2);
            rightPaddle.pos_x = FIELD_X;
            Program p = new Program();
            Ball ball = new Ball();
            ball.pos_x = FIELD_X / 2;
            ball.pos_y = (FIELD_Y - 1) / 2;
            ball.velocity_x = p.getRandomVelocity();
            ball.velocity_y = p.getRandomVelocity();
            Console.SetBufferSize(500, 500);

            p.drawField(ball);    //Draws the field and adds the paddles
            p.drawPaddle(leftPaddle);
            p.drawPaddle(rightPaddle);
            Console.SetCursorPosition(0, FIELD_Y + 1);
            Console.WriteLine("Player 1: 0");
            Console.WriteLine("Player 2: 0");
            Thread arrowKeysThread = new Thread(() => listenToArrowKeys(rightPaddle));
            Thread WASDThread = new Thread(() => listenToWASD(leftPaddle));
            Thread BallThread = new Thread(() => bounceBall(ball, leftPaddle, rightPaddle));
            arrowKeysThread.Start(); //Starts both input Threads
            WASDThread.Start();
            BallThread.Start();




        }

        //Draws the basic fied
        public void drawField(Ball ball)
        {
            for (int y_pos = 0; y_pos < FIELD_Y; y_pos++)
            {
                for (int x_pos = 0; x_pos < FIELD_X; x_pos++)
                {
                    if (y_pos == 0 || y_pos == FIELD_Y - 1)
                    {
                        if (x_pos == FIELD_X - 1)
                        {
                            Console.WriteLine("-");
                        }
                        else
                        {
                            Console.Write("-");
                        }
                    }
                    else
                    {
                        if (x_pos == FIELD_X - 1)
                        {
                            Console.WriteLine(" ");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                }
            }
            System.Threading.Thread.Sleep(20);
            Console.SetCursorPosition(ball.pos_x + 1, ball.pos_y);  //Adds the ball
            Console.Write("\b");
            Console.Write("@");
        }

        //Draws the Paddle
        public void drawPaddle(Paddle paddle)
        {
            for (int i = 0; i <= ((paddle.size - 1)); i++)
            {
                Console.SetCursorPosition(paddle.pos_x + 1, paddle.pos_y - ((paddle.size - 1) / 2) + i);
                Console.Write("\b");
                Console.Write("█");
            }
        }

        //Removes a paddle
        public void deletePaddle(Paddle paddle)
        {
            for (int i = 0; i < (paddle.size + 1); i++)
            {
                Console.SetCursorPosition(paddle.pos_x + 1, paddle.pos_y - ((paddle.size - 1) / 2) + i);
                Console.Write("\b");
                Console.Write(" ");
            }
        }

        //Moves the Paddle
        public void movePaddle(Paddle paddle, bool up)
        {
            if ((paddle.pos_y - ((paddle.size - 1) / 2) == 0 && up)|| (paddle.pos_y + ((paddle.size - 1) / 2) == FIELD_Y - 1 && !up)) //Prevents the paddles moving out of bounds
            {
                return;
            }
            else
            {
                deletePaddle(paddle); //Removes the paddle before updating it's position
                if (up) {
                    paddle.pos_y--;
                } else {
                    paddle.pos_y++;
                }
            }
            drawPaddle(paddle); //Redraws the paddle
            Console.SetCursorPosition(FIELD_X + 1, FIELD_Y);
        }

        //Displayes the ball moving around
        public void updateBall(Ball ball, Paddle leftPaddle, Paddle rightPaddle) {
            Console.SetCursorPosition(ball.pos_x + 1, ball.pos_y);            
            Console.Write("\b");                                           
            Console.Write(" ");                                             
            moveBall(ball, leftPaddle, rightPaddle);                          
            Console.SetCursorPosition(ball.pos_x + 1, ball.pos_y);            
            Console.Write("\b");                                            
            Console.Write("@");                                             
            Console.SetCursorPosition(FIELD_X + 1, FIELD_Y);               
        }

        //Does the code for moving the ball around
        private void moveBall(Ball ball, Paddle leftPaddle, Paddle rightPaddle)
        {
            if (ball.pos_x == 0)
            {
                Player2_score++;
                System.Threading.Thread.Sleep(1000);
                hasScored = true;
                resetField(ball, leftPaddle, rightPaddle);
                return;
            }
            if (ball.pos_x == FIELD_X)
            {
                Player1_score++;
                System.Threading.Thread.Sleep(1000);
                hasScored = true;
                resetField(ball, leftPaddle, rightPaddle);
                return;
            }
            if (isNextToPaddle(leftPaddle, rightPaddle, ball)) //CHecks if there is a paddle next to the ball to reflect it
            {
                ball.velocity_x = ball.velocity_x * -1;
            }
            if (ball.pos_y == 1 || ball.pos_y == FIELD_Y - 2)
            {
                ball.velocity_y = ball.velocity_y * -1;  
            }
            ball.pos_x += ball.velocity_x;
            ball.pos_y += ball.velocity_y;
        }

        /*
         * It checks if the ball is between the highest and the lowst point of the paddle and then checks for the X-coordinate
         * Returns "true" if the ball is next to a paddle
         */
        public bool isNextToPaddle(Paddle paddle1, Paddle paddle2, Ball ball) {
            if (paddle1.pos_y - ((paddle1.size - 1) / 2) <= ball.pos_y && paddle1.pos_y + ((paddle1.size - 1) / 2) >= ball.pos_y && ball.pos_x == paddle1.pos_x + 1)
            {
                return true;
            }
            if (paddle2.pos_y - ((paddle2.size - 1) / 2) <= ball.pos_y && paddle2.pos_y + ((paddle2.size - 1) / 2) >= ball.pos_y && ball.pos_x == paddle2.pos_x - 1)
            {
                return true;
            }
                return false;
        }

        //Resets the Ball, Paddles and the Field
        public void resetField(Ball ball, Paddle leftPaddle, Paddle rightPaddle)
        {
            Console.Clear();
            ball.pos_x = FIELD_X / 2;
            ball.pos_y = (FIELD_Y - 1) / 2;
            ball.velocity_x = getRandomVelocity();
            ball.velocity_y = getRandomVelocity();
            leftPaddle.pos_y = ((FIELD_Y - 1) / 2);
            rightPaddle.pos_y = ((FIELD_Y - 1) / 2);
            drawField(ball);
            drawPaddle(leftPaddle);
            drawPaddle(rightPaddle);
            Console.SetCursorPosition(0, FIELD_Y + 1);
            Console.WriteLine("Player 1: " + Player1_score);
            Console.WriteLine("Player 2: " + Player2_score);
            System.Threading.Thread.Sleep(1000);
            hasScored = false;
        }

        //Returns either -1 or 1
        public int getRandomVelocity()
        {
            Random random = new Random();
        Start:
            int a = random.Next(-1, 2);
          if (a == 0)
          {
               goto Start;
          }
          else
          {
              return a;
            }
        }

        //Thread responsible for Arrow Key input
        public static void listenToArrowKeys(Paddle paddle)
        {
            Program p = new Program();
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        p.movePaddle(paddle, true);
                        break;
                    case ConsoleKey.DownArrow:
                        p.movePaddle(paddle, false);
                        break;
                }
            }
        }

        //Thread responsible for WASD input
        public static void listenToWASD(Paddle paddle)
        {
            Program p = new Program();
            while (true)
            {
                switch (Console.ReadKey().Key.ToString())
                {
                    case "W":
                        p.movePaddle(paddle, true);
                        break;
                    case "S":
                        p.movePaddle(paddle, false);
                        break;
                }
            }
        }

        //Thread responsible for bouncing the ball around
        public static void bounceBall(Ball ball, Paddle leftPaddle, Paddle rightPaddle)
        {
            System.Threading.Thread.Sleep(5000);
            Program p = new Program();
            Loop:
            while (p.hasScored == false)
            {
                System.Threading.Thread.Sleep(75);
                p.updateBall(ball, leftPaddle, rightPaddle);
            }
            goto Loop;
        }
    }
}
