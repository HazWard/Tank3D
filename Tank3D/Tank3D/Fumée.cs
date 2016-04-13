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
    public class Fumée : Sprite
    {
        const string NOM_TEXTURE = "Fumée";
        const float DURÉE_AFFICHAGE = 5f;
        float TempsÉcouléDepuisMAJ { get; set; }
        public Fumée(Game game, Vector2 position, float échelle)
            : base(game, NOM_TEXTURE, position, échelle) { }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsÉcouléDepuisMAJ >= DURÉE_AFFICHAGE)
            {
                Game.Components.Remove(this);
            }
            base.Update(gameTime);
        }
    }
}
