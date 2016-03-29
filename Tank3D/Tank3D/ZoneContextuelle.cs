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
    public class ZoneContextuelle : ArrièrePlan
    {
        // Propriétés
        Rectangle Zone { get; set; }
        string TypeMenu { get; set; }
        public ZoneContextuelle(Game game, string nomImage, string typeMenu, Rectangle zone)
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
            AfficherContenu();
            GestionSprites.End();
        }

        void AfficherContenu()
        {
            switch(TypeMenu)
            {
                case "Instructions":
                    ÉcrireInstructions();
                    break;

                case "Options":
                    ÉcrireOptions();
                    break;
            }
        }

        void ÉcrireOptions()
        {
            // Afficher un tank avec une texture changeante
            // Choix du nombre d'ennemi
            // Difficulté (peut-être)
        }

        void ÉcrireInstructions()
        {
            // Afficher les instructions
        }
    }
}
