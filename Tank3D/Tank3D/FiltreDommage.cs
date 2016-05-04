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
    public class FiltreDommage : Filtre
    {
        const string NOM_TEXTURE_DOMMAGE = "Dommage";
        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        int Alpha { get; set; }
        public FiltreDommage(Game game, float intervalleMAJ)
            : base(game, NOM_TEXTURE_DOMMAGE)
        {
            IntervalleMAJ = intervalleMAJ;
            Alpha = 255;
        }

        public override void Update(GameTime gameTime)
        {
            Temps�coul�DepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                R�duireAlpha();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        void R�duireAlpha()
        {
            if (Filtre�cran.Couleur.A <= 0 || Utilisateur.EstMort)
            {
                Activation = false;
                Game.Components.Remove(this);
                EstDansComponents = false;
            }
            else
            {
                Activation = true;
                Filtre�cran.Couleur = new Color(Filtre�cran.Couleur.R, Filtre�cran.Couleur.G, Filtre�cran.Couleur.B, Alpha -= 3);
            }
        }
    }
}
