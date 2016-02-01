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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TexteCentré : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string TexteÀAfficher { get; set; }
        string NomFont { get; set; }
        Rectangle ZoneAffichage { get; set; }
        Rectangle ZoneAffichageMarge { get; set; }
        Color CouleurTexte { get; set; }
        float Marge { get; set; }
        float Échelle { get; set; }
        Vector2 Position { get; set; }
        Vector2 Origine { get; set; }

        SpriteBatch GestionSprites { get; set; }
        SpriteFont Font { get; set; }
        RessourcesManager<SpriteFont> GestionFont { get; set; }



        public TexteCentré(Game jeu, string texteÀAfficher, string nomFont, Rectangle zoneAffichage, Color couleurTexte, float marge)
            : base(jeu)
        {
            TexteÀAfficher = texteÀAfficher;
            NomFont = nomFont;
            ZoneAffichage = zoneAffichage;
            CouleurTexte = couleurTexte;
            Marge = marge;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            float demiMarge = Marge / 2f;

            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionFont = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            Font = GestionFont.Find(NomFont);

            Vector2 dimension = Font.MeasureString(TexteÀAfficher);

            ZoneAffichageMarge = new Rectangle((int)(ZoneAffichage.X + (demiMarge * ZoneAffichage.Width)), (int)(ZoneAffichage.Y + (demiMarge * ZoneAffichage.Height)), 
                                        (int)(ZoneAffichage.Width - (Marge * ZoneAffichage.Width)), (int)(ZoneAffichage.Height - (Marge * ZoneAffichage.Height)));
            Échelle = MathHelper.Min(ZoneAffichageMarge.Width / dimension.X, ZoneAffichageMarge.Height / dimension.Y);
            Position = new Vector2(ZoneAffichageMarge.X + (ZoneAffichageMarge.Width / 2f), ZoneAffichageMarge.Y + (ZoneAffichageMarge.Height/2));
            Origine = new Vector2(dimension.X / 2,dimension.Y / 2);


            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(Font,TexteÀAfficher, Position, CouleurTexte,0,Origine,Échelle, SpriteEffects.None,1f);
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
