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
    public class BarreHUD : Microsoft.Xna.Framework.GameComponent
    {
        public ArrièrePlan FiltreÉcran { get; set; }
        public bool Activation { get; set; }
        protected bool EstDansComponents { get; set; }
        Joueur Utilisateur { get; set; }

        public BarreHUD(Game game, string nomImage)
            : base(game)
        {
            FiltreÉcran = new ArrièrePlan(game, nomImage);
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
