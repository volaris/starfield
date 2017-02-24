using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.ColorUtils;
using StarfieldUtils.SoundUtils;

namespace StarfieldDrivers.Sound_Responsive
{
    [DriverType(DriverTypes.SoundResponsive)]
    public class Snakes : IStarfieldDriver
    {
        #region Private members
        private int time = 0;
        private const int MAX_SNAKE_LENGTH = 50;

        private class CoordType
        {
            public long X;
            public long Y;
            public long Z;
        }
        private class SnakeType
        {
            public List<CoordType> Pos = new List<CoordType>(); //position of snake head thru tail
            public CoordType Vel;                               //velocity vector of snake head
        }
        private List<SnakeType> snake = new List<SnakeType>();
        private Color[] snakeColor = new Color[12] { Color.FromArgb(0xFF, 0, 0),
                                                     Color.FromArgb(0, 0xFF, 0),
                                                     Color.FromArgb(0, 0, 0xFF),
                                                     Color.FromArgb(0xFF, 0xFF, 0),
                                                     Color.FromArgb(0, 0xFF, 0xFF),
                                                     Color.FromArgb(0xFF, 0, 0xFF),
                                                     Color.FromArgb(0x80, 0, 0),
                                                     Color.FromArgb(0, 0x80, 0),
                                                     Color.FromArgb(0, 0, 0x80),
                                                     Color.FromArgb(0x80, 0x80, 0),
                                                     Color.FromArgb(0, 0x80, 0x80),
                                                     Color.FromArgb(0x80, 0, 0x80)};
        CSCoreLoopbackSoundProcessor soundProcessor;

        // these values come from the starfield max values
        private long _maxX;
        private long _maxY;
        private long _maxZ;

        // user adjustable properties
        private int _numSnakes = 8;
        private int _lenSnakes = 12;
        private bool _fadeSnakes = true;
        private bool _soundResponsive = true;
        private int _snakeSpeed = 6;
        private bool _onSoundChangeDir = false;
        private bool _onSoundSpeedUp = false;
        #endregion

        #region Public Properties
        public int NumSnakes
        {
            get { return _numSnakes; }
            set { _numSnakes = value; InitSnakes(); }
        }
        public int SnakeLength
        {
            get { return _lenSnakes; }
            set { if (value < MAX_SNAKE_LENGTH && value > 0) { _lenSnakes = value; InitSnakes(); } }
        }
        public bool FadeSnakes
        {
            get { return _fadeSnakes; }
            set { _fadeSnakes = value; }
        }
        public bool OnSoundSpeedUp
        {
            get { return _soundResponsive; }
            set { _soundResponsive = value; InitSnakes();  }
        }
        public int SnakeSpeedFactor
        {
            get { return _snakeSpeed; }
            set { _snakeSpeed = value; }
        }
        public bool OnSoundSpeedUpMore
        {
            get { return _onSoundSpeedUp; }
            set { _onSoundSpeedUp = value; }
        }
        public bool OnSoundChangeDir
        {
            get { return _onSoundChangeDir; }
            set { _onSoundChangeDir = value; }
        }
        #endregion

        #region Event Handlers
        void soundProcessor_OnArtifactDetected(Artifact artifact)
        {
            if (_soundResponsive)
            {
                MoveSnakes(true);
            }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            if (_snakeSpeed > 0)
            {
                time++;
                time = time % _snakeSpeed;
            }

            if (time == 0 || _snakeSpeed <= 0)
            {
                // adjust snake positions if their speed is greater than zero
                if (_snakeSpeed > 0)
                {
                    MoveSnakes(false);
                }
                
                // calc starfield colors
                // Y-dir is vertical with 0 at bottom
                List<Color> starfieldColors = new List<Color>((int)_maxX * (int)_maxY * (int)_maxZ);
                for (int k=0; k < _maxX * _maxY * _maxZ; k++)
                {
                    starfieldColors.Add(new Color());
                    starfieldColors[k] = Color.Black;
                }
                for (int i = 0; i < _numSnakes; i++)
                {
                    for (int j = 0; j < _lenSnakes; j++)
                    {
                        int index = (int)(snake[i].Pos[j].X +
                                          snake[i].Pos[j].Z * _maxX +
                                          snake[i].Pos[j].Y * _maxX * _maxZ);
                        if (_fadeSnakes)
                        {
                            starfieldColors[index] = ColorUtils.GetGradientColor(snakeColor[i % snakeColor.Length], Color.FromArgb(0, 0, 0), (float)(j + 1) / _lenSnakes, true);
                        }
                        else {
                            starfieldColors[index] = snakeColor[i % snakeColor.Length];
                        }
                    }
                }

                // draw starfield
                for (int i=0; i < _maxX * _maxY * _maxZ; i++)
                {
                    int y = (int)(i / (_maxX * _maxZ));
                    int z = (int)((i % (_maxX * _maxZ)) / _maxX);
                    int x = (int)((i % (_maxX * _maxZ)) % _maxX);
                    Starfield.SetColor(x, y, z, starfieldColors[i]);
                }
            }
        }

        public void Start(StarfieldModel myStarfield)
        {
            _maxX = (long)myStarfield.NumX;
            _maxY = (long)myStarfield.NumY;
            _maxZ = (long)myStarfield.NumZ;

            InitSnakes();

            // setup sound processor
            if (_soundResponsive)
            {
                soundProcessor = new CSCoreLoopbackSoundProcessor();
                soundProcessor.ArtifactDelay = 100;
                soundProcessor.OnArtifactDetected += soundProcessor_OnArtifactDetected;
            }
        }

        public void Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Snakes";
        }
        #endregion

        private bool CollisionSoon(int curr)
        {
            // check collision with walls
            if (snake[curr].Pos[0].X + snake[curr].Vel.X >= _maxX ||
               snake[curr].Pos[0].X + snake[curr].Vel.X < 0 ||
               snake[curr].Pos[0].Y + snake[curr].Vel.Y >= _maxY ||
               snake[curr].Pos[0].Y + snake[curr].Vel.Y < 0 ||
               snake[curr].Pos[0].Z + snake[curr].Vel.Z >= _maxZ ||
               snake[curr].Pos[0].Z + snake[curr].Vel.Z < 0)
            {
                return true;
            }

            // check collision with other snakes (including tails and self)
            for (int i = 0; i < _numSnakes; i++)
            {
                for (int j = 0; j < _lenSnakes; j++)
                {
                    if (i != curr || j != 0)
                    {
                        if (snake[curr].Pos[0].X + snake[curr].Vel.X == snake[i].Pos[j].X &&
                           snake[curr].Pos[0].Y + snake[curr].Vel.Y == snake[i].Pos[j].Y &&
                           snake[curr].Pos[0].Z + snake[curr].Vel.Z == snake[i].Pos[j].Z)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void MoveSnakes(bool onSound)
        {
            const uint MAX_ITERS = 100; //max allowed iterations to find a new direction for a snake
            uint iter;
            Random rndDir = new Random();

            for (int i = 0; i < _numSnakes; i++)
            {
                int velFactor = 1;
                if (_onSoundSpeedUp && onSound)
                {
                    velFactor = 2;
                }

                // move more than once if just changed direction (due to sound)
                for (int velIter = 0; velIter < velFactor; velIter++)
                {
                    if ((_onSoundChangeDir && onSound && velIter == 0) || CollisionSoon(i))
                    {
                        // start moving this snake in a different direction
                        iter = 0;
                        do
                        {
                            iter++;

                            // clear the current velocity vectory
                            snake[i].Vel.X = 0;
                            snake[i].Vel.Y = 0;
                            snake[i].Vel.Z = 0;

                            // choose a new direction at random
                            switch (rndDir.Next(0, 6))
                            {
                                case 0:
                                    snake[i].Vel.X = 1;
                                    break;
                                case 1:
                                    snake[i].Vel.X = -1;
                                    break;
                                case 2:
                                    snake[i].Vel.Y = 1;
                                    break;
                                case 3:
                                    snake[i].Vel.Y = -1;
                                    break;
                                case 4:
                                    snake[i].Vel.Z = 1;
                                    break;
                                default:
                                    snake[i].Vel.Z = -1;
                                    break;
                            }
                        } while (iter < MAX_ITERS && CollisionSoon(i));

                        if (iter >= MAX_ITERS)
                        {
                            // if unable to find a new direction, freeze the snake in place
                            snake[i].Vel.X = 0;
                            snake[i].Vel.Y = 0;
                            snake[i].Vel.Z = 0;
                        }
                    }

                    // if we found a valid direction, move this snake
                    if (snake[i].Vel.X != 0 || snake[i].Vel.Y != 0 || snake[i].Vel.Z != 0)
                    {
                        // move the head in the direction of the velocity vector
                        snake[i].Pos[0].X = Math.Min(Math.Max(snake[i].Pos[0].X + snake[i].Vel.X, 0), _maxX - 1);
                        snake[i].Pos[0].Y = Math.Min(Math.Max(snake[i].Pos[0].Y + snake[i].Vel.Y, 0), _maxY - 1);
                        snake[i].Pos[0].Z = Math.Min(Math.Max(snake[i].Pos[0].Z + snake[i].Vel.Z, 0), _maxZ - 1);

                        // move the rest of the snake following the head
                        for (int j = _lenSnakes - 1; j > 0; j--)
                        {
                            snake[i].Pos[j].X = snake[i].Pos[j - 1].X;
                            snake[i].Pos[j].Y = snake[i].Pos[j - 1].Y;
                            snake[i].Pos[j].Z = snake[i].Pos[j - 1].Z;
                        }
                    }
                }
            }
        }

        private void InitSnakes()
        {
            const int MAX_ITERS = 1000; //max iterations to find an unoccupied position for a snake
            int iter;
            Random rndNum = new Random();
            bool occupied;

            // initialize snakes
            snake.Clear();
            snake.Capacity = _numSnakes;
            for (int i = 0; i < _numSnakes; i++)
            {
                snake.Add(new SnakeType());

                // initialize snake positions
                snake[i].Pos.Clear();
                snake[i].Pos.Capacity = _lenSnakes;
                for (int k = 0; k < _lenSnakes; k++)
                {
                    snake[i].Pos.Add(new CoordType());
                }

                // find a random starting position for each snake
                iter = 0;
                do
                {
                    // find a new position for the snake
                    iter++;
                    snake[i].Pos[0].X = rndNum.Next((int)(0), (int)(_maxX) - (int)(1));
                    snake[i].Pos[0].Y = rndNum.Next(0, (int)(_maxY) - 1);
                    snake[i].Pos[0].Z = rndNum.Next(0, (int)(_maxZ) - 1);

                    // check that each snake is not on top of another snake
                    occupied = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (snake[i].Pos[0].X == snake[j].Pos[0].X &&
                           snake[i].Pos[0].Y == snake[j].Pos[0].Y &&
                           snake[i].Pos[0].Z == snake[j].Pos[0].Z)
                        {
                            occupied = true;
                            break;
                        }
                    }
                } while (occupied && iter < MAX_ITERS);

                // determine the start vector for each snake at random
                // 6 possible directions: +X, -X, +Y, -Y, +Z, -Z
                snake[i].Vel = new CoordType();
                snake[i].Vel.X = 0;
                snake[i].Vel.Y = 0;
                snake[i].Vel.Z = 0;
                switch (rndNum.Next(0, 5))
                {
                    case 0:
                        snake[i].Vel.X = 1;
                        break;
                    case 1:
                        snake[i].Vel.X = -1;
                        break;
                    case 2:
                        snake[i].Vel.Y = 1;
                        break;
                    case 3:
                        snake[i].Vel.Y = -1;
                        break;
                    case 4:
                        snake[i].Vel.Z = 1;
                        break;
                    default:
                        snake[i].Vel.Z = -1;
                        break;
                }
            }
        }
    }
}
