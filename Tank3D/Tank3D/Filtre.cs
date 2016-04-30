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
    public class Filtre : Microsoft.Xna.Framework.GameComponent
    {
        string NomImage { get; set; }
        protected ArriËrePlan Filtre…cran { get; set; }
        public bool Activation { get; set; }
        protected bool EstDansComponents { get; set; }

        public Filtre(Game game, string nomImage)
            :base(game)
        {
            Filtre…cran = new ArriËrePlan(game, nomImage);
        }
        public override void Initialize()
        {
            Game.Components.Add(Filtre…cran);
            EstDansComponents = true;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(Activation)
            {
                if (!EstDansComponents)
                {
                    Game.Components.Add(Filtre…cran);
                    EstDansComponents = true;
                }
            }
            else
            {
                Game.Components.Remove(Filtre…cran);
                EstDansComponents = false;
            }
            base.Update(gameTime);
        }
    }
}
