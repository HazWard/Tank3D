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
    public class PlanExplosion : PlanTextur�
    {
        const int TEMPS_EXPLOSION_MAX = 2;
        float Temps�coul�DepuisMAJ { get; set; }

        public PlanExplosion(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 �tendue, Vector2 charpente, string nomTexture, float intervalleMAJ)
         : base(jeu, homoth�tieInitiale, rotationInitiale, positionInitiale, �tendue, charpente,nomTexture, intervalleMAJ)
        {
        }
        public override void Update(GameTime gameTime)
        {
            
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;

            if (Temps�coul�DepuisMAJ >= TEMPS_EXPLOSION_MAX)
            {
                Game.Components.Remove(this);
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            RasterizerState JeuRasterizerState = new RasterizerState();
            RasterizerState ancienRasterizerState = EffetDeBase.GraphicsDevice.RasterizerState;
            JeuRasterizerState.CullMode = CullMode.None;
            JeuRasterizerState.FillMode = ancienRasterizerState.FillMode;
            EffetDeBase.GraphicsDevice.RasterizerState = JeuRasterizerState;
            base.Draw(gameTime);
            EffetDeBase.GraphicsDevice.RasterizerState = ancienRasterizerState;
        }
    }
}
