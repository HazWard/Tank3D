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
    public class Souris : Microsoft.Xna.Framework.DrawableGameComponent
    {

        string NomTextureSouris { get; set; }
        Texture2D TextureSouris { get; set; }
        MouseState ÉtatSouris { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures {get;set;}
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        public Vector2 Position { get; private set; }
        
        public Souris(Game jeu, string nomTextureSouris, float intervalleMAJ)
            : base(jeu)
        {
            NomTextureSouris = nomTextureSouris;
            IntervalleMAJ = intervalleMAJ;
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            TextureSouris = GestionnaireDeTextures.Find(NomTextureSouris);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ÉtatSouris = Mouse.GetState();
            
            //Position = new Vector2(ÉtatSouris.X, ÉtatSouris.Y);

            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                Position = new Vector2(ÉtatSouris.X, ÉtatSouris.Y);
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.Draw(TextureSouris, Position, Color.White);
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
