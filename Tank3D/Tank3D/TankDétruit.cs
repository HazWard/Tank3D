﻿using System;
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
    public class TankDétruit : ObjetDeBase, IModel
    {
        int Compteur { get; set; }
        public TankDétruit(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Compteur++;
            if (Compteur > 500)
            {
                Game.Components.Remove(this);
            }
            base.Update(gameTime);
        }
    }
}
