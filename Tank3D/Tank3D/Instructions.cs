using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    public class Instructions : Arri�rePlan
    {
        // Propri�t�s
        Rectangle Zone { get; set; }
        public Instructions(Game game, string nomImage, Rectangle zone)
            : base(game, nomImage)
        {
            Zone = zone;
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.Draw(ImageDeFond, Zone, Color.White);
            �crireInstructions();
            GestionSprites.End();
        }

        void �crireInstructions()
        {
            StreamReader texte�Lire = new StreamReader(@"..\..\..\Lorem.txt");
        }
    }
}
