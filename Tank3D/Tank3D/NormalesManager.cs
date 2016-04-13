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

    public class NormalesManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const float ANGLE_MAX = MathHelper.Pi / 16f;
        
        Terrain TerrainJeu { get; set; }
        
        public NormalesManager(Game game)
            : base(game) {}

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            TerrainJeu = Game.Services.GetService(typeof(Terrain)) as Terrain;
        }

        public Vector2 GetNormale(Point coords, Vector3 anciensAngles)
        {
            Point coordsAvant = new Point(coords.X + (int)(3 * Math.Cos(anciensAngles.Y)), coords.Y + (int)(3 * Math.Sin(anciensAngles.Y)));
            Point coordsAprès = new Point(coords.X - (int)(3 * Math.Cos(anciensAngles.Y)), coords.Y - (int)(3 * Math.Sin(anciensAngles.Y)));

            Vector3 normaleA = TerrainJeu.Normales[coordsAvant.X, coordsAvant.Y];
            Vector3 normaleB = TerrainJeu.Normales[coordsAprès.X, coordsAprès.Y];

            float angleAX = (float)Math.Atan2(normaleA.X, normaleA.Y);
            float angleAY = (float)Math.Atan2(normaleA.Z, normaleA.Y);

            float angleBX = (float)Math.Atan2(normaleB.X, normaleB.Y);
            float angleBY = (float)Math.Atan2(normaleB.Z, normaleB.Y);

            Vector2 angles = CalculMoyenne(new Vector2(angleAX, angleAY), new Vector2(angleBX, angleBY));

            return angles;
        }

        Vector2 CalculMoyenne(Vector2 norm1, Vector2 norm2)
        {
            float moyenneX = (norm1.X + norm2.X) / 2f;
            float moyenneY = (norm1.Y + norm2.Y) / 2f;
            return new Vector2(moyenneX, moyenneY);
        }

        public Vector2 GetDroites(Vector2 position, float rotation)
        {
            Vector2 pointXAvant = new Vector2(position.X + (float)(3f * Math.Cos(rotation)), position.Y + (float)(3f * Math.Sin(rotation)));
            Vector2 pointX= new Vector2(position.X + (float)(3f * Math.Cos(rotation)), position.Y + (float)(3f * Math.Sin(rotation)));
            
            
            Vector2 pointAvant = new Vector2(position.X + (float)(3f * Math.Cos(rotation)), position.Y + (float)(3f * Math.Sin(rotation)));

            return Vector2.Zero;
        }
    }
}