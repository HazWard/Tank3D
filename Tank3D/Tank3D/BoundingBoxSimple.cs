using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    public class BoundingBoxSimple : Microsoft.Xna.Framework.GameComponent
    {
        const float MARGE_X = 55f;
        const float MARGE_Z = 200f;
        public BoundingBox BoundingBoxModèle { get; set; }
        public List<Vector3> ListePoints { get; set; }
        Vector3 Position { get; set; }
        public BoundingBoxSimple(Game game, Vector3 position)
            : base(game)
        {
            Position = position;
        }

        public override void Initialize()
        {
            CreatePoints();
            BoundingBoxModèle = BoundingBox.CreateFromPoints(ListePoints);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
        public void MettreÀJour(float déplacementEnX, float déplacementEnZ)
        {
            MovePoints(déplacementEnX, déplacementEnZ);
            BoundingBoxModèle = new BoundingBox();
            BoundingBoxModèle = BoundingBox.CreateFromPoints(ListePoints);
        }
        void CreatePoints()
        {
            ListePoints = new List<Vector3>();
            ListePoints.Add(new Vector3(Position.X - MARGE_X, Position.Y, Position.Z - MARGE_Z));
            ListePoints.Add(new Vector3(Position.X - MARGE_X, Position.Y + 100, Position.Z - MARGE_Z));
            ListePoints.Add(new Vector3(Position.X + MARGE_X, Position.Y, Position.Z - MARGE_Z));
            ListePoints.Add(new Vector3(Position.X + MARGE_X, Position.Y + 100, Position.Z - MARGE_Z));
            ListePoints.Add(new Vector3(Position.X + MARGE_X, Position.Y, Position.Z));
            ListePoints.Add(new Vector3(Position.X + MARGE_X, Position.Y + 100, Position.Z));
            ListePoints.Add(new Vector3(Position.X - MARGE_X, Position.Y, Position.Z));
            ListePoints.Add(new Vector3(Position.X - MARGE_X, Position.Y + 100, Position.Z));
        }
        void MovePoints(float déplacementEnX, float déplacementEnZ)
        {
            for (int i = 0; i < ListePoints.Count(); i++)
            {
                ListePoints[i] = new Vector3(ListePoints[i].X - déplacementEnX, ListePoints[i].Y, ListePoints[i].Z - déplacementEnZ);
            }
        }

        public bool EstEnCollision(BoundingBox cible)
        {
            return BoundingBoxModèle.Intersects(cible);
        }
    }
}
