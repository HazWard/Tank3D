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
    public class ObjetTournant : ObjetDeBase
    {
        float Temps…coulÈDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        public ObjetTournant(Game game, string nomModËle, float ÈchelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(game, nomModËle, ÈchelleInitiale, rotationInitiale, positionInitiale)
        {
        }

        public override void Initialize()
        {
            Temps…coulÈDepuisMAJ = 0;
            IntervalleMAJ = 1 / 60f;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Temps…coulÈDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Temps…coulÈDepuisMAJ >= IntervalleMAJ)
            {
                Rotation = new Vector3(Rotation.X, Rotation.Y + 0.02f, Rotation.Z);
                Monde = Matrix.Identity;
                Monde *= Matrix.CreateScale(…chelle);
                Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
                Monde *= Matrix.CreateTranslation(Position);
                Temps…coulÈDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
