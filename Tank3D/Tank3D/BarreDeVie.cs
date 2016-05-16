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

    public class BarreDeVie : PlanTexturé, IActivable
    {
        public Vector3 PositionJoueur { get; set; }


        public float PourcentageVie { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
       
        
        Matrix Rotation { get; set; }


        public BarreDeVie(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente,nomTexture, intervalleMAJ)
        {
            
        }

             
        public void ModifierActivation()
        { 
        }
      
       

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ > IntervalleMAJ)
            {
                               
                CalculerNormales();
                
                CalculerMatriceMonde();
                
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }



        // CalculerNormales crée une matrice de rotation qui oriente la barre de vie toujours de 
        // façon perpendiculaire à l'angle de vue de la caméra.


        void CalculerNormales()
        {
            Vector3 VecteurUp = Vector3.Up;
            Vector3 VEntreCaméraEtAi = Position - PositionJoueur;
            Vector3 Right = Vector3.Cross(VecteurUp, VEntreCaméraEtAi);
            Vector3.Normalize(ref Right, out Right);
            Vector3 Backwards = Vector3.Cross(Right, VecteurUp);
            Vector3 Up = Vector3.Cross(Backwards, Right);
            Rotation = new Matrix(Right.X, Right.Y, Right.Z, 0, Up.X, Up.Y, Up.Z, 0, Backwards.X, Backwards.Y, Backwards.Z, 0, 0, 0, 0, 1);
        }

        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Homothétie) *
                    Rotation *
                    Matrix.CreateTranslation(Position);
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
