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

        public Vector2 GetNormale(Point coords, float rotation)
        {
            Point coordsAvant = new Point(coords.X + (int)(3 * Math.Cos(rotation)), coords.Y + (int)(3 * Math.Sin(rotation)));
            Point coordsAprès = new Point(coords.X + (int)(3 * Math.Cos(rotation)), coords.Y + (int)(3 * Math.Sin(rotation)));

            Vector3 normaleA = TerrainJeu.Normales[coordsAvant.X, coordsAvant.Y];
            Vector3 normaleB = TerrainJeu.Normales[coordsAprès.X, coordsAprès.Y];

            float angleAX = (float)Math.Atan2(normaleA.X, normaleA.Y);
            float angleAY = (float)Math.Atan2(normaleA.Z, normaleA.Y);

            float angleBX = (float)Math.Atan2(normaleB.X, normaleB.Y);
            float angleBY = (float)Math.Atan2(normaleB.Z, normaleB.Y);

            return CalculMoyenne(new Vector2(-angleAX, angleAY), new Vector2(-angleBX, angleBY));
        }

        Vector2 CalculMoyenne(Vector2 norm1, Vector2 norm2)
        {
            float moyenneX = (norm1.X + norm2.X) / 2f;
            float moyenneY = (norm1.Y + norm2.Y) / 2f;
            return new Vector2(moyenneX, moyenneY);
        }
    }
}
