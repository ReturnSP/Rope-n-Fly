using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
        //To Spawn rope you must double click
        //You can bring the nodes back if you go in the paint method and uncomment them out

        #region global variables
        //Global Variables
        int score;

        private List<Node> nodes;
        private List<Spring> springs;
        private List<Obstacle> buildings;
        private List<Obstacle> clouds;

        //Buillding Variables
        Random obstacleRandom = new Random();
        int buildingSpawnCountdown = 45;
        int cloudSpawnCountdown = 10;

        //Player Code
        Player player;
        SolidBrush playerBrush = new SolidBrush(Color.Crimson);
        Pen ropeBrush = new Pen(Color.Black, 3);

        //Player variables
        float xSpeed, ySpeed;
        float playerPrevPosX, playerPrevPosY;
        int playerPosUpdateCounter = 10;

        bool leftArrowDown, rightArrowDown, upArrowDown, downArrowDown;

        //Node Variables
        int nodeCount = 7;

        int nodeSpawnLengthX;
        public static float nodeDistanceX;

        int nodeSpawnLengthY;
        public static float nodeDistanceY;

        bool isSwinging = false;
        bool swingAllowed;

        //Mouse Variables
        int mouseclicked;
        public static int swingPointX;
        public static int swingPointY;
        public static bool canSwing;
        #endregion

        public GameScreen()
        {
            InitializeComponent();
            OnStart();

        }

        private void OnStart()
        {
            #region initialize everythings
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

            //Score
            scoreLabel.Text = $"Score: {score}";
            #endregion
        }

        /// <summary>
        /// This function is used to check if the player is swinging on a cloud or building and if they are not
        /// then don't allow them to swing
        /// </summary>
        private bool CheckSwinging(Point p, List<Obstacle> obsList)
        {
            #region checkSwinging
            foreach (Obstacle obs in obsList)
            {
                RectangleF tempRect = new RectangleF(obs.x, obs.y, obs.width, obs.height);
                if (tempRect.Contains(p.X, p.Y) && isSwinging == false)
                {
                    return false;
                }
            }
            return true;
            #endregion
        }
        private void GameScreen_MouseDown(object sender, MouseEventArgs e)
        {
            #region Mouse Stuff
            swingPointX = e.X;
            swingPointY = e.Y;

            mouseclicked++;

            //foreach (Obstacle obs in buildings)
            //{
            //    RectangleF tempRect = new RectangleF(obs.x, obs.y, obs.width, obs.height);
            //    if (tempRect.Contains(e.X, e.Y) && isSwinging == false)
            //    {
            //        GenerateMidRangeNodes();
            //        swingAllowed = true;
            //        isSwinging = true;
            //    }
            //    else
            //    {
            //        nodes.Clear();
            //        springs.Clear();
            //        isSwinging = false;
            //        swingAllowed = false;
            //    }
            //}
            //foreach (Obstacle obs in clouds)
            //{
            //    RectangleF tempRect = new RectangleF(obs.x, obs.y, obs.width, obs.height);
            //    if (tempRect.Contains(e.X, e.Y) && isSwinging == false)
            //    {
            //        GenerateMidRangeNodes();
            //        isSwinging = true;
            //        return;
            //    }
            //    else
            //    {
            //        nodes.Clear();
            //        springs.Clear();
            //        isSwinging = false;
            //        swingAllowed = false;
            //    }
            //}

            //Only Generate new Nodes if you swing another web (in this case click again)

            if (mouseclicked % 2 == 0 || CheckSwinging(new Point(e.X, e.Y), buildings) == true || CheckSwinging(new Point(e.X, e.Y), clouds))
            {
                nodes.Clear();
                springs.Clear();
                isSwinging = false;
            }
            if(CheckSwinging(new Point(e.X,e.Y), buildings) == false || CheckSwinging(new Point(e.X, e.Y), clouds) == false)
            {
                GenerateMidRangeNodes();
                isSwinging = true;
            }
            #endregion
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            #region KeyDown
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
            #endregion
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            #region KeyUp
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
            #endregion
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            #region paint
            var g = e.Graphics;

            foreach (Obstacle obs in buildings)
            {
                g.FillRectangle(Brushes.SandyBrown, obs.x, obs.y, obs.width, obs.height);
            }
            foreach (Obstacle obs in clouds)
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
                //g.FillEllipse(Brushes.Red, node.Position.X - 5, node.Position.Y - 5, 10, 10);
            }
            #endregion
        }

        private void GenerateCloseRangeNodes()
        {
            #region Create close range nodes
            int closeNodeCount = 9;
            nodeSpawnLengthX = (int)(swingPointX - player.x);
            nodeDistanceX = nodeSpawnLengthX / closeNodeCount;

            nodeSpawnLengthY = (int)(player.y - swingPointY);
            nodeDistanceY = nodeSpawnLengthY / closeNodeCount;

            //Create Nodes
            for (int i = 0; i < closeNodeCount; i++)
            {
                if (i == 0)
                {
                    Node tempNode = new Node(player.x, player.y);
                    nodes.Add(tempNode);
                }
                else if (i == closeNodeCount - 1)
                {
                    Node tempNode = new Node(swingPointX, swingPointY, true);
                    nodes.Add(tempNode);
                }
                else
                {
                    Node node = new Node(player.x + nodeDistanceX * i, player.y + nodeDistanceY * i * -1, i == closeNodeCount - 1);
                    nodes.Add(node);
                }
            }

            //Create Springs
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                springs.Add(new Spring(nodes[i], nodes[i + 1], 35f, 20));
            }
            #endregion
        }
        /// <summary>
        /// This function is used to create the nodes
        /// there is a long and short distance one because of a glitch I am trying to fix
        /// </summary>
        private void GenerateMidRangeNodes()
        {
            #region Fix Mid Range Nodes
            nodeSpawnLengthX = (int)(swingPointX - player.x);
            nodeDistanceX = nodeSpawnLengthX / nodeCount;

            nodeSpawnLengthY = (int)(player.y - swingPointY);
            nodeDistanceY = nodeSpawnLengthY / nodeCount;

            ////Create Nodes
            for (int i = 0; i < nodeCount; i++)
            {
                if (i == 0)
                {
                    Node tempNode = new Node(player.x, player.y);
                    nodes.Add(tempNode);
                }
                else if (i == nodeCount - 1)
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
                springs.Add(new Spring(nodes[i], nodes[i + 1], 40f, 20));
            }


            //// Create nodes
            //for (int i = 0; i < nodeCount; i++)
            //{
            //    nodes.Add(new Node(player.x + nodeDistanceX * i, player.y + nodeDistanceY * i * -1, i == nodeCount - 1)); // Fix only the last node
            //}

            //// Create springs
            //for (int i = 0; i < nodes.Count - 1; i++)
            //{
            //    springs.Add(new Spring(nodes[i], nodes[i + 1], 230f, 20));
            //}
            #endregion
        }
        public void GenerateBuildings()
        {
            int tempRand = obstacleRandom.Next(20, 600);
            Obstacle tempObstacle = new Obstacle(this.Width, tempRand, obstacleRandom.Next(50, 250), this.Height - tempRand);
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
            //Counters
            playerPosUpdateCounter--;
            buildingSpawnCountdown--;
            cloudSpawnCountdown--;

            //Increment and display score
            scoreLabel.Text = $"Score: {score}";
            score++;

            foreach (var spring in springs)
            {
                spring.ApplyForce();
            }

            foreach (var node in nodes)
            {
                // Update with deltaTime
                node.Update(0.086f);
            }
            //Stick the player to the rope when swinging
            if (isSwinging)
            {
                player.x = nodes[0].Position.X;
                player.y = nodes[0].Position.Y;
            }
            //Apply forces to the player when flying
            if (isSwinging == false)
            {
                player.AirResistance();
                player.Gravity();
            }

            //Spawn Structures
            if (buildingSpawnCountdown == 0)
            {
                GenerateBuildings();
                buildingSpawnCountdown = 45;
            }
            if (cloudSpawnCountdown == 0)
            {
                GenerateClouds();
                cloudSpawnCountdown = 15;
            }

            if (playerPosUpdateCounter == 0)
            {
                playerPrevPosX = player.x;
                playerPrevPosY = player.y;

                playerPosUpdateCounter = 15;
            }

            //Move the rope with the moving buildings
            foreach(Node n in nodes)
            {
                //origional position
                PointF origional = new PointF(n.Position.X, n.Position.Y);
                PointF resistance = new PointF(-5, 0);

                PointF NewPos = new PointF(n.Position.X + resistance.X, n.Position.Y + resistance.Y);
                n.Position = NewPos;

            }
            swingPointX -= 5;

            //Update buidling positions with player speed (unused for now)
            foreach (Obstacle building in buildings)
            {
                building.Update((float)Math.Sqrt((Math.Pow(player.x - playerPrevPosX, 2) + Math.Pow(player.y - playerPrevPosY, 2))));
            }
            foreach (Obstacle cloud in clouds)
            {
                cloud.Update((float)Math.Sqrt((Math.Pow(player.x - playerPrevPosX, 2) + Math.Pow(player.y - playerPrevPosY, 2))));
            }

            //Delete obstacles off the screen
            foreach (Obstacle building in buildings)
            {
                if (building.x <= -500)
                {
                    buildings.Remove(building);
                    break;
                }
            }
            foreach (Obstacle cloud in clouds)
            {
                if (cloud.x <= -500)
                {
                    clouds.Remove(cloud);
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

            //player.WallCollision(this.Width, this.Height);
            Refresh();
        }
    }
}
