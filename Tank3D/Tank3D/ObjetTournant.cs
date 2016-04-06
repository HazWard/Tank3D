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
        float Temps�coul�DepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        public ObjetTournant(Game game, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(game, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
        }

        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;
            IntervalleMAJ = 1 / 60f;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Temps�coul�DepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                Rotation = new Vector3(Rotation.X, Rotation.Y + 0.02f, Rotation.Z);
                Monde = Matrix.Identity;
                Monde *= Matrix.CreateScale(�chelle);
                Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
                Monde *= Matrix.CreateTranslation(Position);
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
