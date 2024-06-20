using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Rope_n_Fly
{

    public partial class GameScreen : UserControl
    {
        private List<Node> nodes;
        private List<Spring> springs;
        private List<Obstacle> buildings;
        private List<Obstacle> clouds;

        Random obstacleRandom = new Random();
        int buildingSpawnCountdown = 45;

        //Player Code
        Player player;
        SolidBrush playerBrush = new SolidBrush(Color.Crimson);
        Pen ropeBrush = new Pen(Color.Black, 3);

        float xSpeed, ySpeed;
        float playerPrevPosX, playerPrevPosY;
        int playerPosUpdateCounter = 10;

        bool leftArrowDown, rightArrowDown, upArrowDown, downArrowDown;

        //Node Variables
        public static int swingPointX;
        public static int swingPointY;

        int nodeCount = 15;

        int nodeSpawnLengthX;
        public static float nodeDistanceX;

        int nodeSpawnLengthY;
        public static float nodeDistanceY;

        bool isSwinging = false;

        int score;

        //Mouse Variables
        int mouseclicked;

        public GameScreen()
        {
            InitializeComponent();
            OnStart();

        }

        private void OnStart()
        {
            //Reference the lists so they exist
            nodes = new List<Node>();
            springs = new List<Spring>();
            buildings = new List<Obstacle>();
            clouds = new List<Obstacle>();

            //Setup starting Player Values
            float playerX = this.Width / 2 - 25 / 2;
            float playerY = this.Height / 2 - 25 / 2;

            //Creates a new ball
            xSpeed = 8;
            ySpeed = 8;

            player = new Player(playerX, playerY, Convert.ToInt16(xSpeed), Convert.ToInt16(ySpeed));

            playerPrevPosX = player.x;
            playerPrevPosY = player.y;

            scoreLabel.Text = $"Score: {score}";
        }

        private void GameScreen_MouseDown(object sender, MouseEventArgs e)
        {
            swingPointX = e.X;
            swingPointY = e.Y;

            mouseclicked++;

            //Only Generate new Nodes if you swing another web (in this case click again)
            if (mouseclicked % 2 == 0)
            {
                nodes.Clear();
                springs.Clear();
                isSwinging = false;
            }
            else
            {
                GenerateMidRangeNodes();
                isSwinging = true;
            }
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                default:
                    break;

            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
                default:
                    break;
            }
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            foreach (Obstacle obs in buildings)
            {
                g.FillRectangle(Brushes.SandyBrown, obs.x, obs.y, obs.width, obs.height);
            }
            foreach(Obstacle obs in clouds)
            {
                g.FillRectangle(Brushes.WhiteSmoke, obs.x, obs.y, obs.width, obs.height);
            }

            e.Graphics.FillEllipse(Brushes.AliceBlue, player.x, player.y, 10, 10);

            if (isSwinging)
            {
                e.Graphics.FillEllipse(Brushes.AntiqueWhite, swingPointX - 30 / 2, swingPointY - 30 / 2, 30, 30);
            }

            foreach (var spring in springs)
            {
                g.DrawLine(Pens.Black, spring.Node1.Position, spring.Node2.Position);
            }

            foreach (var node in nodes)
            {
                g.FillEllipse(Brushes.Red, node.Position.X - 5, node.Position.Y - 5, 10, 10);
            }

        }

        private void GenerateMidRangeNodes()
        {
            nodeSpawnLengthX = (int)(swingPointX - player.x);
            nodeDistanceX = nodeSpawnLengthX / nodeCount;

            nodeSpawnLengthY = (int)(player.y - swingPointY);
            nodeDistanceY = nodeSpawnLengthY / nodeCount;

            //Create Nodes
            for (int i = 0; i < nodeCount; i++)
            {
                if (i == 0)
                {
                    Node tempNode = new Node(player.x, player.y);
                    nodes.Add(tempNode);
                }
                else if (i == nodeCount)
                {
                    Node tempNode = new Node(swingPointX, swingPointY, true);
                    nodes.Add(tempNode);
                }
                else
                {
                    Node node = new Node(player.x + nodeDistanceX * i, player.y + nodeDistanceY * i * -1, i == nodeCount - 1);
                    nodes.Add(node);
                }
            }

            //Create Springs
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                springs.Add(new Spring(nodes[i], nodes[i + 1], 230f, 20));
            }
        }
        public void GenerateBuildings()
        {
            int tempRand = obstacleRandom.Next(20, 600);
            Obstacle tempObstacle = new Obstacle(this.Width, tempRand, obstacleRandom.Next(250, 450), this.Height - tempRand);
            buildings.Add(tempObstacle);
        }

        public void GenerateClouds()
        {
            int tempRand = obstacleRandom.Next(0, 200);
            Obstacle tempObstacle = new Obstacle(this.Width, tempRand, obstacleRandom.Next(50, 450), 25);
            clouds.Add(tempObstacle);
        }

        private void gameEngine_Tick(object sender, EventArgs e)
        {
            playerPosUpdateCounter--;
            buildingSpawnCountdown--;

            scoreLabel.Text = $"Score: {score}";


            score++;

            foreach (var spring in springs)
            {
                spring.ApplyForce();
            }

            foreach (var node in nodes)
            {
                node.Update(0.066f); // Update with deltaTime
            }
            if (isSwinging)
            {
                player.x = nodes[0].Position.X;
                player.y = nodes[0].Position.Y;
            }
            if (isSwinging == false)
            {
                player.AirResistance();
                player.Gravity();
            }

            //Spawn Structures

            if (buildingSpawnCountdown == 0)
            {
                GenerateClouds();
                GenerateBuildings();
                buildingSpawnCountdown = 45;
            }

            if(playerPosUpdateCounter == 0)
            {
                playerPrevPosX = player.x;
                playerPrevPosY = player.y;

                playerPosUpdateCounter = 15;
            }


            foreach (Obstacle obs in buildings)
            {
                obs.Update(player.x - playerPrevPosX);
            }
            foreach(Obstacle obs in clouds)
            {
                obs.Update(player.x - playerPrevPosX);
            }

            foreach(Obstacle obs in buildings)
            {
                if(obs.x <= -1000)
                {
                    buildings.Remove(obs);
                    break;
                }
            }
            foreach(Obstacle obs in clouds)
            {
                if(obs.x <= -1000)
                {
                    clouds.Remove(obs);
                    break;
                }
            }

            //Key Controls
            if (leftArrowDown)
            {
                player.x -= player.xSpeed;
            }
            if (rightArrowDown)
            {
                player.x += player.xSpeed;
            }
            if (downArrowDown)
            {
                player.y += player.ySpeed;
            }
            //Border Collisions with Player

            player.WallCollision(this.Width, this.Height);
            Refresh();
        }
    }
}
