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

        public Vector2 GetNormale(Point coords)
        {
            Vector3 normale = TerrainJeu.Normales[coords.X,coords.Y];

            float angleX = (float)Math.Atan2(normale.X, normale.Y);
            float angleY = (float)Math.Atan2(normale.Z, normale.Y);

            Console.WriteLine("--------------------");
            Console.WriteLine("Angle en X: {0}", angleX);
            Console.WriteLine("Angle en Y: {0}", angleY);

            return new Vector2(-angleX, angleY);
        }
    }
}
